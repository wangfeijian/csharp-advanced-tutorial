using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Basler.Pylon;
using log4net;
using log4net.Repository.Hierarchy;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Media;
using PixelFormat = System.Windows.Media.PixelFormat;
using System.Windows;

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
                LogManager.GetLogger("vision").Error(ex.Message);
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
                        }

                    }
                    catch (Exception ex)
                    {
                        LogManager.GetLogger("vision").Error($"{CaptureName}打开失败！" + ex.Message);

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
            Bitmap image;
            _camera.StreamGrabber.Start();
            _camera.ExecuteSoftwareTrigger();
            IGrabResult grabResult = _camera.StreamGrabber.RetrieveResult(5000, TimeoutHandling.ThrowException);
            _camera.StreamGrabber.Stop();
            using (grabResult)
            {
                // Image grabbed successfully?
                if (grabResult.GrabSucceeded)
                {

                    PixelDataConverter converter = new PixelDataConverter();

                    Bitmap bitmap = new Bitmap(grabResult.Width, grabResult.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                    // Lock the bits of the bitmap.
                    BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
                    // Place the pointer to the buffer of the bitmap.
                    converter.OutputPixelFormat = PixelType.BGRA8packed;
                    IntPtr ptrBmp = bmpData.Scan0;
                    converter.Convert(ptrBmp, bmpData.Stride * bitmap.Height, grabResult);
                    bitmap.UnlockBits(bmpData);


                    image = bitmap;

                    OnHandlerImage(new[] { image });
                }
                else
                {
                    LogManager.GetLogger("vision").Error($"Error: {grabResult.ErrorCode} {grabResult.ErrorDescription}");
                }
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

                            ImageGrab();
                        }

                        if (!WaitOne(TimeOut))
                        {
                            LogManager.GetLogger("vision").Error($"相机{CaptureName}采集失败" + " - timeout");

                            return 0;
                        }

                        Bitmap image = GetImage();

                        return image == null ? 0 : 1;
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(e.ToString());

                        LogManager.GetLogger("vision").Error($"相机{CaptureName}采集失败" + " - " + e.Message, e);

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

                if (_isGrab)
                {
                    ClearBuffer();

                    ImageGrab();

                    if (!WaitOne(5000))
                    {
                        return 0;
                    }

                    Bitmap image = GetImage();

                    return image == null ? 0 : 1;

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
    }
}
