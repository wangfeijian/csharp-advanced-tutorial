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


            Task.Run(() =>
           {
               while (true)
               {
                   DispatcherHelper.Delay(10);
                   try
                   {
                       Dispatcher.Invoke(delegate
                       {
                           TreeViewItem tree = ToolTreeView.Items[ToolTreeView.Items.Count - 1] as TreeViewItem;
                           ToolBase tool = tree.Tag as ToolBase;

                           if (tool != null && tool.OutputParams.ContainsKey("OutputImage"))
                           {
                               OutputBitmap = tool.OutputParams["OutputImage"] as WriteableBitmap;
                           }
                       });
                   }
                   catch (Exception exception)
                   {
                       Console.WriteLine(exception);
                       throw;
                   }

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
                    DispatcherHelper.Delay(1000);
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
            TreeViewItem treeView = (TreeViewItem)e.Data.GetData(typeof(TreeViewItem));

           TreeViewItem targetTree = e.Source as TreeViewItem;
            int index = ToolTreeView.Items.IndexOf(targetTree);
            if (index > ToolTreeView.Items.Count-1||index<0)
            {
                return;
            }
            ToolTreeView.Items.Remove(treeView);
            ToolTreeView.Items.Insert(index,treeView);
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (ToolTreeView.SelectedItem==null)
            {
                MessageBox.Show("请先选择工具");
                return;
            }

            ToolTreeView.Items.Remove(ToolTreeView.SelectedItem);
        }
    }
}
