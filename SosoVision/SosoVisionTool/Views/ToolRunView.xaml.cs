using MaterialDesignThemes.Wpf;
using SosoVisionTool.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SosoVisionTool.Views
{
    /// <summary>
    /// Interaction logic for ToolRunView.xaml
    /// </summary>
    public partial class ToolRunView : UserControl
    {
        /// <summary>
        /// 工具描述
        /// </summary>
        public string VisionStep
        {
            get { return (string)GetValue(VisionStepProperty); }
            set { SetValue(VisionStepProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToolDesStr.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VisionStepProperty =
            DependencyProperty.Register(nameof(VisionStep), typeof(string), typeof(ToolRunView));

        public ToolRunView()
        {
            InitializeComponent();
        }

        private void ButtonRun_OnClick(object sender, RoutedEventArgs e)
        {
            CancellationTokenSource clt = new CancellationTokenSource();
            CancellationToken cls = clt.Token;
            ButtonRun.IsEnabled = false;

            int time;
            int.TryParse(TimeText.Text, out time);

            if (time < 20)
            {
                time = 20;
            }

            Task.Run(() =>
           {
               while (true)
               {
                   DispatcherHelper.Delay(10);
                   Dispatcher.Invoke(delegate
                   {
                       TreeViewItem tree = ToolTreeView.Items[ToolTreeView.Items.Count - 1] as TreeViewItem;
                       ToolBase tool = tree.Tag as ToolBase;

                       //if (tool != null && tool.OutputParams.ContainsKey("OutputImage"))
                       //{
                       //    OutputBitmap = tool.OutputParams["OutputImage"] as WriteableBitmap;
                       //}
                   });

                   if (cls.IsCancellationRequested)
                   {
                       return;
                   }
               }
           }, cls);

            if (CheckBoxContinue.IsChecked == true)
            {
                while (CheckBoxContinue.IsChecked == true)
                {
                    DispatcherHelper.Delay(time);
                    foreach (var child in ToolTreeView.Items)
                    {
                        TreeViewItem tree = child as TreeViewItem;
                        ToolBase tool = tree.Tag as ToolBase;
                        tool?.Run();
                    }
                }
                clt.Cancel();
                ButtonRun.IsEnabled = true;
            }
            else
            {
                foreach (var child in ToolTreeView.Items)
                {
                    TreeViewItem tree = child as TreeViewItem;
                    ToolBase tool = tree.Tag as ToolBase;
                    tool?.Run();
                }

                clt.Cancel();
                ButtonRun.IsEnabled = true;
            }
        }

        private void ToolTreeView_OnMouseMove(object sender, MouseEventArgs e)
        {
            TreeView tree = sender as TreeView;
            TreeViewItem treItem = tree.SelectedItem as TreeViewItem;

            if (treItem != null && treItem.Parent != tree) return;

            if (treItem != null && e.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(treItem, treItem, DragDropEffects.All);
            }
        }

        private void ToolTreeView_OnDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TreeView)))
            {
                e.Effects = DragDropEffects.All;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void ToolTreeView_OnDrop(object sender, DragEventArgs e)
        {
            TreeViewItem targetTree = e.Source as TreeViewItem;

            if (targetTree != null)
            {
                TreeViewItem treeView = (TreeViewItem)e.Data.GetData(typeof(TreeViewItem));

                if (treeView != null)
                {
                    int index = ToolTreeView.Items.IndexOf(targetTree);
                    if (index > ToolTreeView.Items.Count - 1 || index < 0)
                    {
                        return;
                    }
                    ToolTreeView.Items.Remove(treeView);
                    ToolTreeView.Items.Insert(index, treeView);
                }
            }
            else
            {
                ToolBase tempToolBase = e.Data.GetData("UIElement") as ToolBase;
                if (tempToolBase != null)
                {
                    List<string> tempListStr = new List<string>();

                    foreach (var item in ToolTreeView.Items)
                    {
                        var tempItemView = item as TreeViewItem;
                        if (tempItemView.Header.ToString().Contains(tempToolBase.ToolDesStr))
                            tempListStr.Add(tempItemView.Header.ToString());
                    }

                    string head = tempListStr.Count > 0 ? tempToolBase.ToolDesStr + tempListStr.Count : tempToolBase.ToolDesStr;
                    if (head.Contains("图像采集") && head.Length > 4)
                    {
                        MessageBox.Show("一个视觉处理流程中只能包含一个图像采集源", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    TreeViewItem tree = TreeViewDataAccess.CreateTreeView(head);

                    tree.Tag = tempToolBase;
                    tree.PreviewMouseDoubleClick += Tree_PreviewMouseDoubleClick;
                    ToolTreeView.Items.Add(tree);
                }
            }
        }

        private void Tree_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = sender as TreeViewItem;
            ToolBase tool = treeViewItem.Tag as ToolBase;
            // tool.UIElement_OnPreviewMouseLeftButtonDown(sender, e);
        }



        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (ToolTreeView.SelectedItem == null)
            {
                MessageBox.Show("请先选择工具");
                return;
            }

            if (MessageBox.Show("是否删除该工具，删除后将无法恢复！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
            {
                var tree = ToolTreeView.SelectedItem as TreeViewItem;
                string name = VisionStep.Substring(5,VisionStep.Length - 5);
                string fileName = $"config/Vision/{name}/Tools/{tree.Header}.json";
                string newDirName = $"config/Vision/{name}/Tools/backup/";
                string newFileName = $"config/Vision/{name}/Tools/backup/{tree.Header}.json";
                if (!Directory.Exists(newDirName))
                {
                    Directory.CreateDirectory(newDirName);
                }

                if (File.Exists(fileName))
                {
                    File.Move(fileName, newFileName);
                }

                ToolTreeView.Items.Remove(ToolTreeView.SelectedItem);
            }
        }
    }
}
