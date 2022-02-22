using SosoVisionTool.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SosoVisionTool.ViewModels
{
    public interface IToolBaseViewModel
    {
        void Run(ToolBase tool, ref bool result);
    }
}
