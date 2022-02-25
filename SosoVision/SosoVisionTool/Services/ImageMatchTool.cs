﻿using Newtonsoft.Json;
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

namespace SosoVisionTool.Services
{
    public class ImageMatchTool : ToolBase
    {
        public ImageMatchTool()
        {
            ToolDesStr = "模板匹配";
            ToolTip = "模板匹配";
            ToolIcon = "\xe729";
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
            return JsonConvert.DeserializeObject<ImageMatchToolViewModel>(File.ReadAllText(file));
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
                ToolWindow = new ImageMatchToolWindw() { Title = _tree.Header.ToString() };
                DataContext = new ImageMatchToolViewModel(ToolInVision);
                ToolWindow.DataContext = DataContext;
            }
            else if (ToolWindow == null)
            {
                ToolWindow = new ImageMatchToolWindw() { Title = _tree.Header.ToString() };
                if (DataContext is ImageMatchToolViewModel)
                {
                    ToolWindow.DataContext = DataContext;
                }
                else
                {
                    DataContext = new ImageMatchToolViewModel(ToolInVision);
                    ToolWindow.DataContext = DataContext;
                }
            }


            base.UIElement_OnPreviewMouseDoubleClick(sender, e);
        }
    }
}
