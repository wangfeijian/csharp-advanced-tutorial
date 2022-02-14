using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace SosoVision.Extensions
{
    public class ProcedureParam
    {
        /// <summary>
        /// 流程名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 流程命令
        /// </summary>
        public string Command { get; set; }
        /// <summary>
        /// 流程ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 相机ID
        /// </summary>
        public int CameraId { get; set; }
        /// <summary>
        /// 显示ID
        /// </summary>
        public int ShowId { get; set; }
        /// <summary>
        /// 曝光时间
        /// </summary>
        public double ExposureTIme { get; set; }
        /// <summary>
        /// 亮度
        /// </summary>
        public int Brightness { get; set; }
        /// <summary>
        /// 对比度
        /// </summary>
        public int Contrast { get; set; }
        /// <summary>
        /// 外部触发
        /// </summary>
        public bool Trigger { get; set; }
        /// <summary>
        /// 重试次数
        /// </summary>
        public int RedoCount { get; set; }
    }
}
