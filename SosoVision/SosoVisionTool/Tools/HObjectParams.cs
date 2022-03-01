using HalconDotNet;

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
