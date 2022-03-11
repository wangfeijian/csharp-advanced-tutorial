using System;
using System.Collections.Generic;
using Basler.Pylon;
using Prism.Ioc;
using System.Windows;
using SosoVisionCommonTool.Log;
using HalconDotNet;

namespace ImageCapture
{
    /// <summary>
    /// Basler相机管理类
    /// </summary>
    public class BaslerManger : SingleInstanceTemplate<BaslerManger>
    {
        private Dictionary<string, ICameraInfo> _dictDeviceInfo = new Dictionary<string, ICameraInfo>();

        /// <summary>
        /// 相机初始化
        /// </summary>
        public void Init()
        {
            _dictDeviceInfo.Clear();

            try
            {
                List<ICameraInfo> allCameras = CameraFinder.Enumerate();

                foreach (ICameraInfo cameraInfo in allCameras)
                {
                    _dictDeviceInfo.Add(cameraInfo[CameraInfoKey.DeviceIpAddress], cameraInfo);
                }
            }
            catch (Exception ex)
            {
                ContainerLocator.Container.Resolve<ISosoLogManager>().ShowLogError(ex.Message);
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
        public int Count => _dictDeviceInfo.Count;

        /// <summary>
        /// 获取相机信息
        /// </summary>
        /// <param name="strName"></param>
        /// <returns></returns>
        public ICameraInfo GetDeviceInfo(string strName)
        {
            if (Count == 0)
            {
                Init();
            }

            if (_dictDeviceInfo.ContainsKey(strName))
            {
                return _dictDeviceInfo[strName];
            }

            return null;
        }
    }

    /// <summary>
    /// Basler相机图像采集类
    /// </summary>
    public class CaptureBasler : CaptureBase
    {
        private bool _isGrab;
        private bool _isOpened;
        private Camera _camera;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="strCaptureName"></param>
        /// <param name="isColorImage"></param>
        public CaptureBasler(string strCaptureName, bool isColorImage) : base(strCaptureName, isColorImage)
        {
            Open();
        }

        /// <summary>
        /// 打开相机
        /// </summary>
        /// <returns></returns>
        public override bool Open()
        {
            lock (Lock)
            {
                if (!IsOpen())
                {
                    try
                    {
                        ICameraInfo stDevInfo = BaslerManger.GetInstance().GetDeviceInfo(CaptureName);

                        _camera = new Camera(stDevInfo);

                        _camera.StreamGrabber.ImageGrabbed += OnImageGrabbed;

                        _camera.Close();

                        _camera.Open();

                        _isOpened = _camera.IsOpen;

                        if (_isOpened)
                        {

                            _camera.Parameters[PLCamera.PixelFormat].TrySetValue(PLCamera.PixelFormat.BGRA8Packed);

                            //设置触发模式
                            _camera.Parameters[PLCamera.TriggerMode].TrySetValue(PLCamera.TriggerMode.On);

                            //设置软件触发
                            _camera.Parameters[PLCamera.TriggerSource].TrySetValue(PLCamera.TriggerSource.Software);

                            _camera.Parameters[PLCamera.AcquisitionMode].SetValue(PLCamera.AcquisitionMode.Continuous);

                            // todo
                            _camera.StreamGrabber.Start(GrabStrategy.OneByOne, GrabLoop.ProvidedByStreamGrabber);
                        }

                    }
                    catch (Exception ex)
                    {
                        ContainerLocator.Container.Resolve<ISosoLogManager>().ShowLogError($"{CaptureName}打开失败！" + ex.Message);

                        MessageBox.Show($"相机{CaptureName}打开失败！" + ex.Message);
                        Close();

                        return false;
                    }
                }

                return _isOpened;
            }
        }

        /// <summary>
        /// 图片采集
        /// </summary>
        private void ImageGrab()
        {
            HObject image;
            _camera.StreamGrabber.Start();
            _camera.ExecuteSoftwareTrigger();
            IGrabResult grabResult = _camera.StreamGrabber.RetrieveResult(5000, TimeoutHandling.ThrowException);
            _camera.StreamGrabber.Stop();
            using (grabResult)
            {
                // Image grabbed successfully?
                if (grabResult.GrabSucceeded)
                {
                    HOperatorSet.GenImage1(out image, "byte", grabResult.Width, grabResult.Height, grabResult.PixelDataPointer);
                    OnHandlerImage(new[] { image });
                }
                else
                {
                    ContainerLocator.Container.Resolve<ISosoLogManager>().ShowLogError($"Error: {grabResult.ErrorCode} {grabResult.ErrorDescription}");
                }
            }
        }

        private void OnImageGrabbed(Object sender, ImageGrabbedEventArgs e)
        {
            HObject image;
            try
            {
                // Get the grab result.
                IGrabResult grabResult = e.GrabResult;

                // Check if the image can be displayed.
                if (grabResult.IsValid)
                {
                    HOperatorSet.GenImage1(out image, "byte", grabResult.Width, grabResult.Height, grabResult.PixelDataPointer);
                    OnHandlerImage(new[] { image });
                }
                else
                {
                    ContainerLocator.Container.Resolve<ISosoLogManager>().ShowLogError($"Error: {grabResult.ErrorCode} {grabResult.ErrorDescription}");
                }

            }
            catch (Exception ex)
            {
                ContainerLocator.Container.Resolve<ISosoLogManager>().ShowLogError(ex.Message);
            }
            finally
            {
                // Dispose the grab result if needed for returning it to the grab loop.
                e.DisposeGrabResultIfClone();
            }
        }

        /// <summary>
        /// 相机是否打开
        /// </summary>
        /// <returns></returns>
        public override bool IsOpen()
        {
            return _camera != null && _isOpened;
        }

        public override bool Close()
        {
            if (_camera != null)
            {
                AutoResetEventParam.Set();

                _camera.Close();

                _camera.Dispose();

                _camera = null;

            }

            BaslerManger.GetInstance().DeInit();

            return true;
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

                            // todo
                            _camera.ExecuteSoftwareTrigger();
                            //ImageGrab();
                        }

                        if (!WaitOne(TimeOut))
                        {
                            ContainerLocator.Container.Resolve<ISosoLogManager>().ShowLogError($"相机{CaptureName}采集失败" + " - timeout");

                            return 0;
                        }

                        HObject image = GetImage();

                        return image == null ? 0 : 1;
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(e.ToString());

                        ContainerLocator.Container.Resolve<ISosoLogManager>().ShowLogError($"相机{CaptureName}采集失败" + " - " + e.Message);

                        return 0;
                    }

                }
                return 0;
            }
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
                        _camera.ExecuteSoftwareTrigger();
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

                    ContainerLocator.Container.Resolve<ISosoLogManager>().ShowLogError($"相机{CaptureName}采集失败" + " - " + e.Message);

                    return 0;
                }
            }
            return 1;
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
                Exposure = exposure;

                IFloatParameter p;
                if (_camera.Parameters.Contains(PLCamera.ExposureTimeAbs))
                {
                    p = _camera.Parameters[PLCamera.ExposureTimeAbs];
                }
                else
                {
                    p = _camera.Parameters[PLCamera.ExposureTime];
                }

                exposure *= 1000;

                if (exposure > p.GetMaximum())
                {
                    exposure = p.GetMaximum();
                }
                else if (exposure < p.GetMinimum())
                {
                    exposure = p.GetMinimum();
                }

                if (p.GetValue() != exposure)
                {
                    p.SetValue(exposure);
                }
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
                try
                {
                    Contrast = contrast;

                    IIntegerParameter p;
                    p = _camera.Parameters[PLCamera.GainRaw];

                    double maxValue = p.GetMaximum();
                    double minValue = p.GetMinimum();
                    contrast *= maxValue;

                    if (contrast <= minValue)
                    {
                        contrast = minValue;
                    }

                    int v = (int)contrast;

                    if (p.GetValue() != v)
                    {
                        p.SetValue(v);
                    }
                }
                catch (Exception)
                {
                    Contrast = -1;
                }

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
                if (bEnable)
                {
                    _camera.Parameters[PLCamera.TriggerSource].TrySetValue(PLCamera.TriggerSource.Line1);
                }
                else
                {
                    _camera.Parameters[PLCamera.TriggerSource].TrySetValue(PLCamera.TriggerSource.Software);
                }

                ExTrigger = bEnable;
            }
        }
    }
}
