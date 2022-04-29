using MvCamCtrl.NET;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using HalconDotNet;
using System.Windows;

namespace ImageCapture
{
    /// <summary>
    /// 海康相机管理类
    /// </summary>
    public class HikMgr : SingleInstanceTemplate<HikMgr>
    {
        private Dictionary<string, MyCamera.MV_CC_DEVICE_INFO> m_dictDeviceInfo = new Dictionary<string, MyCamera.MV_CC_DEVICE_INFO>();

        /// <summary>
        /// 相机初始化
        /// </summary>
        public void Init()
        {
            m_dictDeviceInfo.Clear();

            MyCamera.MV_CC_DEVICE_INFO_LIST stDevList = new MyCamera.MV_CC_DEVICE_INFO_LIST();
            int nRet = MyCamera.MV_CC_EnumDevices_NET(MyCamera.MV_GIGE_DEVICE | MyCamera.MV_USB_DEVICE, ref stDevList);
            if (MyCamera.MV_OK != nRet)
            {
                return;
            }

            //遍历相机
            for (Int32 i = 0; i < stDevList.nDeviceNum; i++)
            {
                MyCamera.MV_CC_DEVICE_INFO stDevInfo;                            // 通用设备信息
                stDevInfo = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(stDevList.pDeviceInfo[i], typeof(MyCamera.MV_CC_DEVICE_INFO));

                string strDeviceName = string.Empty;
                if (MyCamera.MV_GIGE_DEVICE == stDevInfo.nTLayerType)
                {
                    MyCamera.MV_GIGE_DEVICE_INFO stGigEDeviceInfo = (MyCamera.MV_GIGE_DEVICE_INFO)MyCamera.ByteToStruct(stDevInfo.SpecialInfo.stGigEInfo, typeof(MyCamera.MV_GIGE_DEVICE_INFO));
                    uint nIp1 = ((stGigEDeviceInfo.nCurrentIp & 0xff000000) >> 24);
                    uint nIp2 = ((stGigEDeviceInfo.nCurrentIp & 0x00ff0000) >> 16);
                    uint nIp3 = ((stGigEDeviceInfo.nCurrentIp & 0x0000ff00) >> 8);
                    uint nIp4 = (stGigEDeviceInfo.nCurrentIp & 0x000000ff);
                    //Console.WriteLine("\n" + i.ToString() + ": [GigE] User Define Name : " + stGigEDeviceInfo.chUserDefinedName);
                    strDeviceName = nIp1 + "." + nIp2 + "." + nIp3 + "." + nIp4;
                }
                else if (MyCamera.MV_USB_DEVICE == stDevInfo.nTLayerType)
                {
                    MyCamera.MV_USB3_DEVICE_INFO stUsb3DeviceInfo = (MyCamera.MV_USB3_DEVICE_INFO)MyCamera.ByteToStruct(stDevInfo.SpecialInfo.stUsb3VInfo, typeof(MyCamera.MV_USB3_DEVICE_INFO));
                    //Console.WriteLine("\n" + i.ToString() + ": [U3V] User Define Name : " + stUsb3DeviceInfo.chUserDefinedName);
                    //Console.WriteLine("\n Serial Number : " + stUsb3DeviceInfo.chSerialNumber);
                    //Console.WriteLine("\n Device Number : " + stUsb3DeviceInfo.nDeviceNumber);
                    strDeviceName = stUsb3DeviceInfo.chUserDefinedName;
                }

                m_dictDeviceInfo.Add(strDeviceName, stDevInfo);
            }
        }

        /// <summary>
        /// 相机资源释放
        /// </summary>
        public void DeInit()
        {

        }

        /// <summary>
        /// 相机数量
        /// </summary>
        public int Count
        {
            get
            {
                return m_dictDeviceInfo.Count;
            }
        }

        /// <summary>
        /// 获取相机
        /// </summary>
        /// <param name="strName"></param>
        /// <returns></returns>
        public MyCamera.MV_CC_DEVICE_INFO GetDeviceInfo(string strName)
        {
            if (Count == 0)
            {
                Init();
            }

            if (m_dictDeviceInfo.ContainsKey(strName))
            {
                return m_dictDeviceInfo[strName];
            }

            return new MyCamera.MV_CC_DEVICE_INFO();
        }

    }


    /// <summary>
    /// 海康相机采集类
    /// </summary>
    public class CaptureHik : CaptureBase
    {
        private bool _isGrab;
        private bool _isOpened;
        private MyCamera _camera;
        private MyCamera.cbOutputExdelegate _imageCallBack;

        public CaptureHik(string strCaptureName, bool isColorImage) : base(strCaptureName, isColorImage)
        {
            Open();
        }

        /// <summary>
        /// 关闭相机
        /// </summary>
        /// <returns></returns>
        public override bool Close()
        {
            if (_camera != null)
            {
                AutoResetEventParam.Set();

                int nRet = _camera.MV_CC_StopGrabbing_NET();

                // ch:关闭设备 | en:Close device
                nRet = _camera.MV_CC_CloseDevice_NET();

                // ch:销毁设备 | en:Destroy device
                nRet = _camera.MV_CC_DestroyDevice_NET();

                _camera = null;

            }

            HikMgr.GetInstance().DeInit();

            return true;
        }

        public override int Grab()
        {
            lock (Lock)
            {
                if (!_isGrab)
                {
                    _isGrab = true;

                }

                try
                {

                    if (_isGrab)
                    {
                        ClearBuffer();

                        // todo
                        _camera.MV_CC_SetCommandValue_NET("TriggerSoftware");
                        //ImageGrab();

                        if (!WaitOne(5000))
                        {
                            return 0;
                        }

                        HObject image = GetImage();

                        return image == null ? 0 : 1;

                    }
                }
                catch (Exception e)

                {
                    System.Diagnostics.Debug.WriteLine(e.ToString());


                    return 0;
                }
            }
            return 1;
        }

        private void OnImageGrabbed(IntPtr pData, ref MyCamera.MV_FRAME_OUT_INFO_EX pFrameInfo, IntPtr pUser)
        {
            HObject image = null;
            int nWidth = pFrameInfo.nWidth;
            int nHeight = pFrameInfo.nHeight;

            IntPtr pImageBuffer = pData;

            try
            {
                HOperatorSet.GenImage1(out image, "byte", nWidth, nHeight, pImageBuffer);
                OnHandlerImage(new[] { image });

            }
            catch (Exception ex)
            {
            }
        }

        public override bool IsOpen()
        {
            return _camera != null && _isOpened;
        }

        public override bool Open()
        {
            lock (Lock)
            {
                if (!IsOpen())
                {
                    try
                    {
                        MyCamera.MV_CC_DEVICE_INFO stDevInfo = HikMgr.GetInstance().GetDeviceInfo(CaptureName);

                        _camera = new MyCamera();

                        int nRet = _camera.MV_CC_CreateDevice_NET(ref stDevInfo);
                        if (MyCamera.MV_OK != nRet)
                        {
                            _isOpened = false;
                            MessageBox.Show("相机打开失败");
                            return false;
                        }

                        // ch:打开设备 | en:Open device
                        nRet = _camera.MV_CC_OpenDevice_NET();
                        if (MyCamera.MV_OK != nRet)
                        {
                            _isOpened = false;
                            MessageBox.Show("相机打开失败");
                            return false;
                        }

                        _isOpened = true;

                        // ch:探测网络最佳包大小(只对GigE相机有效) | en:Detection network optimal package size(It only works for the GigE camera)
                        if (stDevInfo.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                        {
                            int nPacketSize = _camera.MV_CC_GetOptimalPacketSize_NET();
                            if (nPacketSize > 0)
                            {
                                nRet = _camera.MV_CC_SetIntValue_NET("GevSCPSPacketSize", (uint)nPacketSize);
                                if (nRet != MyCamera.MV_OK)
                                {
                                    MessageBox.Show("相机打开失败");
                                }
                            }
                            else
                            {
                                MessageBox.Show("相机打开失败");
                            }
                        }

                        // ch:设置采集连续模式 | en:Set Continues Aquisition Mode
                        nRet = _camera.MV_CC_SetEnumValue_NET("AcquisitionMode", (uint)MyCamera.MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_CONTINUOUS);

                        //设置触发模式
                        nRet = _camera.MV_CC_SetEnumValue_NET("TriggerMode", (uint)MyCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON);

                        //设置软件触发
                        nRet = _camera.MV_CC_SetEnumValue_NET("TriggerSource", (uint)MyCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_SOFTWARE);

                        // ch:注册回调函数 | en:Register image callback
                        _imageCallBack = new MyCamera.cbOutputExdelegate(OnImageGrabbed);
                        nRet = _camera.MV_CC_RegisterImageCallBackEx_NET(_imageCallBack, IntPtr.Zero);

                        // ch:开启抓图 || en: start grab image
                        nRet = _camera.MV_CC_StartGrabbing_NET();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("相机打开失败" + ex.Message);
                        Close();

                        return false;
                    }
                }

                return _isOpened;
            }
        }

        public override int Snap()
        {
            lock (Lock)
            {
                if (_isGrab)
                {
                    StopGrab();
                }

                if (!IsOpen())
                {
                    Open();
                }

                if (IsOpen())
                {
                    try
                    {
                        if (!ExTrigger)
                        {
                            ClearBuffer();

                            _camera.MV_CC_SetCommandValue_NET("TriggerSoftware");
                        }

                        if (!WaitOne(TimeOut))
                        {

                            return 0;
                        }

                        HObject image = GetImage();

                        return image == null ? 0 : 1;
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(e.ToString());


                        return 0;
                    }

                }
                return 0;
            }
        }

        public override bool StopGrab()
        {
            if (_isGrab)
            {
                _isGrab = false;

                AutoResetEventParam.Set();

                ClearBuffer(false);
            }

            return true;
        }

        /// <summary>
        /// 设置曝光
        /// </summary>
        /// <param name="exposure"></param>
        public override void SetExposure(double exposure)
        {
            if (IsOpen() && Exposure != exposure)
            {
                MyCamera.MVCC_FLOATVALUE pValue = new MyCamera.MVCC_FLOATVALUE();
                _camera.MV_CC_GetExposureTime_NET(ref pValue);

                float fexposure = (float)(exposure * 1000);


                if (fexposure > pValue.fMax)
                {
                    fexposure = pValue.fMax;
                }
                else if (fexposure < pValue.fMin)
                {
                    fexposure = pValue.fMin;
                }

                if (fexposure != pValue.fCurValue)
                {
                    _camera.MV_CC_SetExposureTime_NET(fexposure);
                }

                Exposure = exposure;
            }

        }

        /// <summary>
        /// 设置亮度
        /// </summary>
        /// <param name="bright"></param>
        public override void SetBrightness(double bright)
        {
            if (IsOpen() && Bright != bright)
            {
                MyCamera.MVCC_INTVALUE pValue = new MyCamera.MVCC_INTVALUE();
                _camera.MV_CC_GetBrightness_NET(ref pValue);

                uint ff = (uint)(bright * pValue.nMax);

                if (ff > pValue.nMax)
                {
                    ff = pValue.nMax;
                }
                else if (ff < pValue.nMin)
                {
                    ff = pValue.nMin;
                }

                if (ff != pValue.nCurValue)
                {
                    _camera.MV_CC_SetBrightness_NET(ff);
                }

                Bright = bright;
            }
        }

        /// <summary>
        /// 设置对比度
        /// </summary>
        /// <param name="contrast"></param>
        public override void SetConstract(double contrast)
        {
            if (IsOpen() && Contrast != contrast)
            {
                MyCamera.MVCC_FLOATVALUE pValue = new MyCamera.MVCC_FLOATVALUE();
                _camera.MV_CC_GetGain_NET(ref pValue);

                float ff = (float)contrast * pValue.fMax;

                if (ff > pValue.fMax)
                {
                    ff = pValue.fMax;
                }
                else if (ff < pValue.fMin)
                {
                    ff = pValue.fMin;
                }

                if (ff != pValue.fCurValue)
                {
                    _camera.MV_CC_SetGain_NET(ff);
                }

                Contrast = contrast;
            }

        }

        /// <summary>
        /// 设置硬件触发
        /// </summary>
        /// <param name="bEnable"></param>
        public override void SetTrigger(bool bEnable)
        {
            if (IsOpen() && ExTrigger != bEnable)
            {
                int nRet;

                if (bEnable)
                {
                    nRet = _camera.MV_CC_SetEnumValue_NET("TriggerSource", (uint)MyCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_LINE0);
                }
                else
                {
                    nRet = _camera.MV_CC_SetEnumValue_NET("TriggerSource", (uint)MyCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_SOFTWARE);
                }

                ExTrigger = bEnable;
            }
        }
    }
}
