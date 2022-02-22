using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;
using Prism.Events;

namespace SosoVisionTool.Tools
{
    public class MessageEvent : PubSubEvent<LogStruct>
    {
    }

    public class HObjectEvent : PubSubEvent<HObjectParams>
    {

    }
}
