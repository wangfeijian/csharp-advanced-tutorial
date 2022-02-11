using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SosoVision.Extensions
{
    /// <summary>
    /// 程序序列化
    /// </summary>
    public class SerializationData
    {
        public ObservableCollection<string> VisionTitle { get; set; }
        public Dictionary<string, object> VisionView { get; set; }
    }
}
