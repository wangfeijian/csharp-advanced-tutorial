using HalconDotNet;
using System.Collections.Generic;

namespace SosoVisionTool.Services
{
    public class AllVisionRunData
    {
        /// <summary>
        /// 流程运行后图片
        /// </summary>
        public Dictionary<string, HObject> VisionRunImage { get; set; }
        /// <summary>
        /// 流程运行后区域
        /// </summary>
        public Dictionary<string, HObject> VisionRunRegion { get; set; }
        /// <summary>
        /// 流程运行后得到的Double结果
        /// </summary>
        public Dictionary<string, double> VisionRunDoubleValue { get; set; }
        /// <summary>
        /// 流程运行后得到的int结果
        /// </summary>
        public Dictionary<string, int> VisionRunIntValue { get; set; }

        /// <summary>
        /// 流程运行后得到的string结果
        /// </summary>
        public Dictionary<string, string> VisionRunStringValue { get; set; }
    }

    public class DataKeyAndType
    {
        public string DataType { get; set; }

        public string DataKey { get; set; }
    }

    public class ScriptInputData
    {
        public string DataName { get; set; }
        public string DataType { get; set; }
        public string DataKey { get; set; }
    }

    public class ScriptOutputData
    {
        public string DataName { get; set; }
        public string DataType { get; set; }
        public object Value { get; set; }
    }
}
