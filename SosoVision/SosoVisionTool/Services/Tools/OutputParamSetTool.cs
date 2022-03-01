using Newtonsoft.Json;
using SosoVisionTool.ViewModels;
using SosoVisionTool.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace SosoVisionTool.Services.Tools
{
    public class OutParamSetTool : ToolBase
    {
        public OutParamSetTool()
        {
            ToolDesStr = "输出参数";
            ToolTip = "输出参数";
            ToolIcon = "\xe60a";
        }
        public override TreeViewItem CreateTreeView(string name)
        {
            ToolItem = new TreeViewItem { Header = name };
            ToolOutputItem = new TreeViewItem { Header = AddInOutTreeViewItem(false) };
            ToolItem.Items.Add(ToolOutputItem);
            return ToolItem;
        }

        public override object GetDataContext(string file)
        {
            return JsonConvert.DeserializeObject<OutputParamSetToolViewModel>(File.ReadAllText(file));
        }

        public override void UIElement_OnPreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem _tree = sender as TreeViewItem;
            string fileName = $"config/Vision/{ToolInVision}/ToolsData/{_tree.Header}.json";
            if (_tree == null)
            {
                return;
            }

            if (ToolWindow == null && DataContext == null)
            {
                ToolWindow = new OutputParamSetToolWindow() { Title = _tree.Header.ToString() };
                DataContext = new OutputParamSetToolViewModel(ToolInVision);
                ToolWindow.DataContext = DataContext;
            }
            else if (ToolWindow == null)
            {
                ToolWindow = new OutputParamSetToolWindow() { Title = _tree.Header.ToString() };
                if (DataContext is OutputParamSetToolViewModel)
                {
                    ToolWindow.DataContext = DataContext;
                }
                else
                {
                    DataContext = new OutputParamSetToolViewModel(ToolInVision);
                    ToolWindow.DataContext = DataContext;
                }
            }


            base.UIElement_OnPreviewMouseDoubleClick(sender, e);
        }
    }
}
