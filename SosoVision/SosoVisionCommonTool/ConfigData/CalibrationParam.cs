using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SosoVisionCommonTool.ConfigData
{
    public class CalibrationParam: BindableBase
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
        /// 标定ID
        /// </summary>
        private int _calibId;

        public int CalibId
        {
            get { return _calibId; }
            set { _calibId = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 标定名称
        /// </summary>
        private string _calibName;

        public string CalibName
        {
            get { return _calibName; }
            set { _calibName = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 标定类型
        /// </summary>
        private string _calibType;

        public string CalibType
        {
            get { return _calibType; }
            set { _calibType = value; RaisePropertyChanged(); }
        }
    }
}
