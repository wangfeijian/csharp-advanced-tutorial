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
        public string Result { get; set; }
        public string ShowMessage { get; set; }
    }
}
