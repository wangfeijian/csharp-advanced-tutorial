using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenCvSharpTool
{
    /// <summary>
    /// 工具基类，任何工具都需要实现该类
    /// </summary>
    public abstract class ToolBase
    {
        /// <summary>
        /// 输入参数
        /// </summary>
        public List<object> InputParams;

        /// <summary>
        /// 输出参数
        /// </summary>
        public List<object> OutputParams;

        /// <summary>
        /// 工具运行
        /// </summary>
        public abstract void Run();
    }
}
