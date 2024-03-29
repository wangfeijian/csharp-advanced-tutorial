﻿using Newtonsoft.Json;
using SosoVisionTool.ViewModels;
using SosoVisionTool.Views;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;

namespace SosoVisionTool.Services.Tools
{
    public class ImageAcquisitionTool : ToolBase
    {
        public ImageAcquisitionTool()
        {
            ToolDesStr = "图像采集";
            ToolTip = "图像采集";
            ToolIcon = "\xe967";
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
            return JsonConvert.DeserializeObject<ImageAcquisitionToolViewModel>(File.ReadAllText(file));
        }
        public override void UIElement_OnPreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem _tree = sender as TreeViewItem;
            if (_tree == null)
            {
                return;
            }

            if (ToolWindow == null && DataContext == null)
            {
                ToolWindow = new ImageAcquisitionToolWindow() { Title = _tree.Header.ToString() };
                DataContext = new ImageAcquisitionToolViewModel(ToolInVision, CameraId);
                ToolWindow.DataContext = DataContext;
            }
            else if (ToolWindow == null)
            {
                ToolWindow = new ImageAcquisitionToolWindow() { Title = _tree.Header.ToString() };
                if (DataContext is ImageAcquisitionToolViewModel)
                {
                    ToolWindow.DataContext = DataContext;
                }
                else
                {
                    DataContext = new ImageAcquisitionToolViewModel(ToolInVision, CameraId);
                    ToolWindow.DataContext = DataContext;
                }
            }


            base.UIElement_OnPreviewMouseDoubleClick(sender, e);
        }
    }
}
