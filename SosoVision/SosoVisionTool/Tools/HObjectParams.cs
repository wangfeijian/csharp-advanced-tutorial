using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SosoVisionTool.Tools
{
    public class HObjectParams
    {
        public string VisionStep { get; set; }
        public HObject Image { get; set; }
        public string ImageKey { get; set; }
        public HObject Region { get; set; }
        public string RegionKey { get; set; }
    }
}
