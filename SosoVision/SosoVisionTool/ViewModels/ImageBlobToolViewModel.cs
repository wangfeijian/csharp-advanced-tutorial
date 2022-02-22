using System;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SosoVisionTool.Views;
using System.Windows.Controls;

namespace SosoVisionTool.ViewModels
{
    public class ImageBlobToolViewModel : BindableBase, IToolBaseViewModel
    {
        public ImageBlobToolViewModel()
        {

        }

        public void Run(ToolBase tool, ref bool result)
        {
            TreeViewItem temp = new TreeViewItem { Header = "input", ToolTip = "input" };
            tool.AddInputOutputTree(temp, true);
            temp = new TreeViewItem { Header = "output", ToolTip = "output" };
            tool.AddInputOutputTree(temp, false);
            result = false;
        }
    }
}
