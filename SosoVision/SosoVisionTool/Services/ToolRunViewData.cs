using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SosoVisionTool.Services
{
    public class ToolRunViewData
    {
        /// <summary>
        /// 工具运行后图片
        /// </summary>
        public Dictionary<string, HObject> ToolOutputImage { get; set; }
        /// <summary>
        /// 工具运行后区域
        /// </summary>
        public Dictionary<string, HObject> ToolOutputRegion { get; set; }
        /// <summary>
        /// 工具运行后得到的Double结果
        /// </summary>
        public Dictionary<string, double> ToolOutputDoubleValue { get; set; }
        /// <summary>
        /// 工具运行后得到的int结果
        /// </summary>
        public Dictionary<string, double> ToolOutputIntValue { get; set; }
    }
}
