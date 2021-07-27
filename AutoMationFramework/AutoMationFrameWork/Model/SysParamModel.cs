using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMationFrameWork.Model
{
    /// <summary>
    /// 系统参数类型
    /// </summary>
    public class ParameterInfo
    {
        /// <summary>
        /// 键值
        /// </summary>
        public string KeyValue { get; set; }
        /// <summary>
        /// 当前值
        /// </summary>
        public string CurrentValue { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// 参数描述
        /// </summary>
        public string ParamDesc { get; set; }
        /// <summary>
        /// 翻译
        /// </summary>
        public string EnglishDesc { get; set; }
        /// <summary>
        /// 最小值
        /// </summary>
        public string MinValue { get; set; }
        /// <summary>
        /// 最大值
        /// </summary>
        public string MaxValue { get; set; }
        /// <summary>
        /// 权限
        /// </summary>
        public string Authority { get; set; }
    }

    /// <summary>
    /// 系统参数信息
    /// </summary>
    public class Parameters
    {
        private List<ParameterInfo> _parameterInfos;
        /// <summary>
        /// 参数集合
        /// </summary>
        public List<ParameterInfo> ParameterInfos
        {
            get { return _parameterInfos; }
            set { _parameterInfos = value; }
        }

    }
}
