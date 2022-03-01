using Newtonsoft.Json;
using SosoVisionTool.ViewModels;
using SosoVisionTool.Views;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;

namespace SosoVisionTool.Services.Tools
{
    public class InputParamSetTool : ToolBase
    {
        public InputParamSetTool()
        {
            ToolDesStr = "输入参数";
            ToolTip = "输入参数";
            ToolIcon = "\xe60c";
        }
        public override TreeViewItem CreateTreeView(string name)
        {
            ToolItem = new TreeViewItem { Header = name };
            ToolInputItem = new TreeViewItem { Header = AddInOutTreeViewItem(true) };
            ToolItem.Items.Add(ToolInputItem);
            return ToolItem;
        }

        public override object GetDataContext(string file)
        {
            return JsonConvert.DeserializeObject<InputParamSetToolViewModel>(File.ReadAllText(file));
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
                ToolWindow = new InputParamSetToolWindow() { Title = _tree.Header.ToString() };
                DataContext = new InputParamSetToolViewModel(ToolInVision);
                ToolWindow.DataContext = DataContext;
            }
            else if (ToolWindow == null)
            {
                ToolWindow = new InputParamSetToolWindow() { Title = _tree.Header.ToString() };
                if (DataContext is InputParamSetToolViewModel)
                {
                    ToolWindow.DataContext = DataContext;
                }
                else
                {
                    DataContext = new InputParamSetToolViewModel(ToolInVision);
                    ToolWindow.DataContext = DataContext;
                }
            }


            base.UIElement_OnPreviewMouseDoubleClick(sender, e);
        }
    }
}
