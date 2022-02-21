using Newtonsoft.Json;
using SosoVisionTool.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace SosoVisionTool.Services
{
    public class ImageBlobTool : ToolBase
    {
        private TreeViewItem _tree;

        public ImageBlobTool()
        {
            ToolDesStr = "Blob分析";
            ToolTip = "Blob分析";
            ToolIcon = "\xe729";
        }

        public override void UIElement_OnPreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _tree = sender as TreeViewItem;
            string fileName = $"config/Vision/{ToolInVision}/ToolsData/{_tree.Header}.json";
            if (_tree == null)
            {
                return;
            }

            if (ToolWindow == null && File.Exists(fileName))
            {
                ToolWindow = new ImageBlobWindow() { ToolTreeViewItem = _tree, BlobTool = this ,Title=_tree.Header.ToString()};
                var viewModel = JsonConvert.DeserializeObject<object>(File.ReadAllText(fileName));
                ToolWindow.DataContext = viewModel;
            }
            else if (ToolWindow == null)
                ToolWindow = new ImageBlobWindow() { ToolTreeViewItem = _tree, BlobTool = this ,Title=_tree.Header.ToString()};


            base.UIElement_OnPreviewMouseDoubleClick(sender, e);
        }
    }
}
