using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace OpenCvSharpTool
{
    /// <summary>
    /// ToolRunBox.xaml 的交互逻辑
    /// </summary>
    public partial class ToolRunBox : UserControl
    {
        public static Dictionary<string, object> OutputObjects = new Dictionary<string, object>();
        public ToolRunBox()
        {
            InitializeComponent();
        }

        public WriteableBitmap OutputBitmap
        {
            get { return (WriteableBitmap)GetValue(OutputBitmapProperty); }
            set { SetValue(OutputBitmapProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OutputBitmap.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OutputBitmapProperty =
            DependencyProperty.Register(nameof(OutputBitmap), typeof(WriteableBitmap), typeof(ToolRunBox));


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

                       if (tool != null && tool.OutputParams.ContainsKey("OutputImage"))
                       {
                           OutputBitmap = tool.OutputParams["OutputImage"] as WriteableBitmap;
                       }
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
                    TreeViewItem tree = CreateTreeView(tempToolBase.ToolDesStr);

                    tempToolBase.ContextMenu = null;
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
            tool.UIElement_OnPreviewMouseLeftButtonDown(sender, e);
        }

        private TreeViewItem CreateTreeView(string name)
        {
            TreeViewItem nameItem = new TreeViewItem { Header = name };
            TreeViewItem inputItem = new TreeViewItem { Header = "输入" };
            TreeViewItem outputItem = new TreeViewItem { Header = "输出" };
            nameItem.Items.Add(inputItem);
            nameItem.Items.Add(outputItem);
            return nameItem;
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
                ToolTreeView.Items.Remove(ToolTreeView.SelectedItem);
            }
        }
    }
}
