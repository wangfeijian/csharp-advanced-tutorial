using System;
using System.CodeDom;
using System.Collections.Concurrent;
using System.Drawing;
using System.Threading;
using System.Windows.Media.Imaging;

namespace ImageCapture
{
    /// <summary>
    /// 图片采集的基类
    /// </summary>
    public abstract class CaptureBase
    {
        /// <summary>
        /// 处理外部触发委托
        /// </summary>
        /// <param name="strCmd"></param>
        /// <param name="paramList"></param>
        /// <param name="image"></param>
        public delegate void ProcessExTriggerHandler(string strCmd, string[] paramList, Bitmap[] image);

        /// <summary>
        ///  处理外部触发事件
        /// </summary>
        public event ProcessExTriggerHandler OnProcessExTriggerHandlerEvent;

        /// <summary>
        /// 自动事件
        /// </summary>
        protected AutoResetEvent AutoResetEventParam = new AutoResetEvent(false);

        /// <summary>
        /// 图片队列的最大长度
        /// </summary>
        public const int BufferLength = 10;

        /// <summary>
        /// 互锁
        /// </summary>
        protected readonly object Lock = new object();

        /// <summary>
        /// 图片队列
        /// </summary>
        protected ConcurrentQueue<Bitmap> Images = new ConcurrentQueue<Bitmap>();

        /// <summary>
        /// 采集名称
        /// </summary>
        private string _captureName;

        /// <summary>
        /// 采集名称
        /// </summary>
        public string CaptureName
        {
            get { return _captureName; }
            set { _captureName = value; }
        }
        private double _exposure;

        /// <summary>
        /// 是否为彩色图片
        /// </summary>
        private bool _isColorImage;

        /// <summary>
        /// 是否为彩色图片
        /// </summary>
        public bool IsColorImage
        {
            get { return _isColorImage; }
            set { _isColorImage = value; }
        }

        /// <summary>
        /// 曝光
        /// </summary>
        public double Exposure => _exposure;

        private double _bright = -1;

        /// <summary>
        /// 亮度
        /// </summary>
        public double Bright => _bright;

        private double _contrast = -1;

        /// <summary>
        /// 对比度
        /// </summary>
        public double Contrast => _contrast;

        private double _timeOut;

        /// <summary>
        /// 超时时间，单位（ms）
        /// </summary>
        public double TimeOut => _timeOut;

        private bool _exTrigger;

        /// <summary>
        /// 外部触发
        /// </summary>
        public bool ExTrigger => _exTrigger;

        /// <summary>
        /// 检测命令
        /// </summary>
        public string InspectCmd { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public CaptureBase(string strCaptureName, bool isColorImage)
        {
            _captureName = strCaptureName;
            _isColorImage = isColorImage;
        }

        /// <summary>
        /// 获取当前采集的图片
        /// </summary>
        /// <param name="isRemove">是否删除图片</param>
        /// <returns></returns>
        public Bitmap GetImage(bool isRemove = false)
        {
            Bitmap image;

            bool getImageOk = isRemove ? Images.TryDequeue(out image) : Images.TryPeek(out image);

            if (getImageOk)
            {
                return image;
            }

            return null;
        }

        /// <summary>
        /// 获取图像列表
        /// </summary>
        /// <returns></returns>
        public Bitmap[] GetImages()
        {
            Bitmap[] images = Images.ToArray();

            ClearImages();

            return images;
        }

        /// <summary>
        /// 清除队列中的图片
        /// </summary>
        private void ClearImages()
        {
            Bitmap image;

            while (Images.Count > 0)
            {
                Images.TryDequeue(out image);
            }
        }

        /// <summary>
        /// 打开采集工具
        /// </summary>
        /// <returns></returns>
        public abstract bool Open();

        /// <summary>
        /// 判断采集工具是否打开
        /// </summary>
        /// <returns></returns>
        public abstract bool IsOpen();

        /// <summary>
        /// 关闭采集工具
        /// </summary>
        /// <returns></returns>
        public abstract bool Close();

        /// <summary>
        /// 释放相机资源
        /// </summary>
        public virtual void DeInit()
        {
            ClearImages();
        }

        /// <summary>
        /// 软触发一次同步采集
        /// </summary>
        /// <returns></returns>
        public abstract int Snap();

        /// <summary>
        /// 设置曝光
        /// </summary>
        /// <param name="exposure"></param>
        public virtual void SetExposure(double exposure)
        {
            _exposure = exposure;
        }

        /// <summary>
        /// 设置亮度
        /// </summary>
        /// <param name="bright"></param>
        public virtual void SetBrightness(double bright)
        {
            _bright = bright;
        }

        /// <summary>
        /// 设置对比度
        /// </summary>
        /// <param name="contrast"></param>
        public virtual void SetConstract(double contrast)
        {
            _contrast = contrast;
        }

        /// <summary>
        /// 设置超时时间
        /// </summary>
        /// <param name="timeOut"></param>
        public virtual void SetTimeOut(double timeOut)
        {
            _timeOut = timeOut;
        }

        /// <summary>
        /// 设置硬触发
        /// </summary>
        /// <param name="isTrigger"></param>
        public virtual void SetTrigger(bool isTrigger)
        {
            _exTrigger = isTrigger;
        }

        /// <summary>
        /// 清空缓存
        /// </summary>
        /// <param name="isResetEvent"></param>
        public void ClearBuffer(bool isResetEvent = true)
        {
            if (isResetEvent)
            {
                AutoResetEventParam.Reset();
            }

            ClearImages();
        }

        /// <summary>
        /// 触发一次异步采集
        /// </summary>
        /// <returns></returns>
        public abstract int Grab();

        /// <summary>
        /// 停止异步采集
        /// </summary>
        /// <returns></returns>
        public abstract bool StopGrab();

        /// <summary>
        /// 处理图像
        /// </summary>
        /// <param name="image"></param>
        protected void OnHandlerImage(Bitmap[] image)
        {
            foreach (var item in image)
            {
                while (Images.Count > BufferLength)
                {
                    Bitmap temp;
                    Images.TryDequeue(out temp);
                }

                Images.Enqueue(item);
            }

            AutoResetEventParam.Set();

            if (OnProcessExTriggerHandlerEvent != null && _exTrigger && !string.IsNullOrEmpty(InspectCmd))
            {
                OnProcessExTriggerHandlerEvent(InspectCmd, null, image);
            }

            GC.Collect();
        }

        /// <summary>
        /// 等待触发
        /// </summary>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        protected bool WaitOne(double timeOut)
        {
            if (Images.Count > 0)
            {
                AutoResetEventParam.Reset();

                return true;
            }

            return AutoResetEventParam.WaitOne((int)timeOut);
        }
    }
}
