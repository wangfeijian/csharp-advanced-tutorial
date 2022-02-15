using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace SosoVision.Extensions
{
    public class ProcedureParam : BindableBase
    {
        /// <summary>
        /// 删除该行
        /// </summary>
        private bool _delete;

        public bool Delete
        {
            get { return _delete; }
            set { _delete = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 流程名称
        /// </summary>
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 流程命令
        /// </summary>
        private string _command;

        public string Command
        {
            get { return _command; }
            set { _command = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 流程ID
        /// </summary>
        private int _id;

        public int Id
        {
            get { return _id; }
            set { _id = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 相机ID
        /// </summary>
        private int _cameraId;

        public int CameraId
        {
            get { return _cameraId; }
            set { _cameraId = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 显示ID
        /// </summary>
        private int _showId;

        public int ShowId
        {
            get { return _showId; }
            set { _showId = value; RaisePropertyChanged(); }
        }
        /// <summary>
        /// 曝光时间
        /// </summary>
        private double _exposureTIme;

        public double ExposureTIme
        {
            get { return _exposureTIme; }
            set { _exposureTIme = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 亮度
        /// </summary>
        private int _brightness;

        public int Brightness
        {
            get { return _brightness; }
            set { _brightness = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 对比度
        /// </summary>
        private int _contrast;

        public int Contrast
        {
            get { return _contrast; }
            set { _contrast = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 外部触发
        /// </summary>
        private bool _trigger;

        public bool Trigger
        {
            get { return _trigger; }
            set { _trigger = value; }
        }

        /// <summary>
        /// 重试次数
        /// </summary>
        private int _redoCount;

        public int RedoCount
        {
            get { return _redoCount; }
            set { _redoCount = value; RaisePropertyChanged(); }
        }

    }
}
