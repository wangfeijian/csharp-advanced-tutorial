using Newtonsoft.Json;
using SosoVisionTool.ViewModels;
using SosoVisionTool.Views;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;

namespace SosoVisionTool.Services.Tools
{
    public class FindLineTool : ToolBase
    {
        public FindLineTool()
        {
            ToolDesStr = "找线工具";
            ToolTip = "找线工具";
            ToolIcon = "\xe60a";
        }
        public override TreeViewItem CreateTreeView(string name)
        {
            ToolItem = new TreeViewItem { Header = name };
            ToolInputItem = new TreeViewItem { Header = AddInOutTreeViewItem(true) };
            ToolItem.Items.Add(ToolInputItem);
            ToolOutputItem = new TreeViewItem { Header = AddInOutTreeViewItem(false) };
            ToolItem.Items.Add(ToolOutputItem);
            return ToolItem;
        }

        public override object GetDataContext(string file)
        {
             return JsonConvert.DeserializeObject<FindLineToolViewModel>(File.ReadAllText(file));
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
                ToolWindow = new FindLineToolWindow() { Title = _tree.Header.ToString() };
                DataContext = new FindLineToolViewModel(ToolInVision);
                ToolWindow.DataContext = DataContext;
            }
            else if (ToolWindow == null)
            {
                ToolWindow = new FindLineToolWindow() { Title = _tree.Header.ToString() };
                if (DataContext is FindLineToolViewModel)
                {
                    ToolWindow.DataContext = DataContext;
                }
                else
                {
                    DataContext = new FindLineToolViewModel(ToolInVision);
                    ToolWindow.DataContext = DataContext;
                }
            }


            base.UIElement_OnPreviewMouseDoubleClick(sender, e);
        }
    }
}
