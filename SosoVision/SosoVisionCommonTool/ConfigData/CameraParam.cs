using Prism.Mvvm;
using System.Collections.Generic;

namespace SosoVisionCommonTool.ConfigData
{
    public class CameraParam: BindableBase
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
        /// 相机ID
        /// </summary>
        private int _cameraId;

        public int CameraId
        {
            get { return _cameraId; }
            set { _cameraId = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 相机名称
        /// </summary>
        private string _cameraName;

        public string CameraName
        {
            get { return _cameraName; }
            set { _cameraName = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 相机IP
        /// </summary>
        private string _cameraIP;

        public string CameraIP
        {
            get { return _cameraIP; }
            set { _cameraIP = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 相机品牌
        /// </summary>
        private string _cameraBand;

        public string CameraBand
        {
            get { return _cameraBand; }
            set { _cameraBand = value; RaisePropertyChanged(); }
        }
    }
}
