using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SosoVisionCommonTool.Log
{
    public interface ISosoLogManager
    {
        void ShowLogInfo(string msg);

        void ShowLogError(string msg);

        void ShowLogWarning(string msg);
    }
}
