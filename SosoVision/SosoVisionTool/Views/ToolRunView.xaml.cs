using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
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

        // Using a DependencyProperty as the backing store for VisionStep.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VisionStepProperty =
            DependencyProperty.Register(nameof(VisionStep), typeof(string), typeof(ToolRunView));

        /// <summary>
        /// 相机ID
        /// </summary>
        public string CameraID
        {
            get { return (string)GetValue(CameraIDProperty); }
            set { SetValue(CameraIDProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CameraID.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CameraIDProperty =
            DependencyProperty.Register(nameof(CameraID), typeof(string), typeof(ToolRunView));

        public ToolRunView()
        {
            InitializeComponent();
        }

        private void ButtonRun_OnClick(object sender, RoutedEventArgs e)
        {
            ButtonRun.IsEnabled = false;

            int time;
            int.TryParse(TimeText.Text, out time);

            if (time < 20)
            {
                time = 20;
            }


            if (CheckBoxContinue.IsChecked == true)
            {
                while (CheckBoxContinue.IsChecked == true)
                {
                    DispatcherHelper.Delay(time);
                    foreach (var child in ToolTreeView.Items)
                    {
                        TreeViewItem tree = child as TreeViewItem;
                        ToolBase tool = tree.Tag as ToolBase;
                        tree.Background = Brushes.Transparent;
                        bool result = false;
                        tool?.Run(tool, ref result);
                        if (!result)
                        {
                            tree.Background = Brushes.Red;
                            ButtonRun.IsEnabled = true;
                            return;
                        }
                    }
                }
                ButtonRun.IsEnabled = true;
            }
            else
            {
                Run();

                ButtonRun.IsEnabled = true;
            }
        }

        public void Run()
        {
            foreach (var child in ToolTreeView.Items)
            {
                TreeViewItem tree = child as TreeViewItem;
                ToolBase tool = tree.Tag as ToolBase;
                tree.Background = Brushes.Transparent;
                bool result = false;
                tool?.Run(tool, ref result);
                if (!result)
                {
                    tree.Background = Brushes.Red;
                    ButtonRun.IsEnabled = true;
                    return;
                }
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

                    TreeViewItem tree = tempToolBase.CreateTreeView(head);
                    tempToolBase.ToolInVision = VisionStep;
                    tempToolBase.CameraId = CameraID;

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
            tool.UIElement_OnPreviewMouseDoubleClick(sender, e);
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
                string fileName = $"config/Vision/{VisionStep}/Tools/{tree.Header}.json";
                string newDirName = $"config/Vision/{VisionStep}/Tools/backup/";
                string newFileName = $"config/Vision/{VisionStep}/Tools/backup/{tree.Header}.json";
                if (!Directory.Exists(newDirName))
                {
                    Directory.CreateDirectory(newDirName);
                }

                if (File.Exists(fileName))
                {
                    if (File.Exists(newFileName))
                    {
                        File.Delete(newFileName);
                    }
                    File.Move(fileName, newFileName);
                }

                ToolTreeView.Items.Remove(ToolTreeView.SelectedItem);
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (ToolTreeView.Items.Count <= 0)
            {
                MessageBox.Show("流程中没有工具，无法保存！");
                return;
            }

            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "请选择文件夹路径";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string foldPath = dialog.SelectedPath;

                var tree = ToolTreeView.SelectedItem as TreeViewItem;

                foreach (var item in ToolTreeView.Items)
                {
                    var saveTree = item as TreeViewItem;
                    string fileName = saveTree.Header.ToString() + ".json";
                    Type t = saveTree.Tag.GetType();

                    File.WriteAllText($"{foldPath}/{fileName}", JsonConvert.SerializeObject(t));
                }
            }
            MessageBox.Show("保存成功！！");
        }

        private void ButtonLoad_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("加载后将删除当前流程中所有的工具且不可恢复，是否加载！！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
            {
                System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
                dialog.Description = "请选择文件夹路径";

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string foldPath = dialog.SelectedPath;

                    DirectoryInfo theFolder = new DirectoryInfo(foldPath);
                    FileInfo[] dirInfo = theFolder.GetFiles();

                    if (dirInfo.Length <= 0)
                    {
                        MessageBox.Show("文件夹中没有备份程序！");
                        return;
                    }

                    //遍历文件夹，检查加载的文件夹中保存的配置是否正确
                    foreach (FileInfo file in dirInfo)
                    {
                        if (file.Extension != ".json")
                        {
                            MessageBox.Show("请确定此文件夹是否为软件备份的视觉流程文件夹！");
                            return;
                        }

                        Type t = JsonConvert.DeserializeObject<Type>(File.ReadAllText(file.FullName));
                        if (t == null)
                        {
                            MessageBox.Show("请确定此文件夹是否为软件备份的视觉流程文件夹！");
                            return;
                        }
                    }

                    ToolTreeView.Items.Clear();

                    Array.Sort(dirInfo, (x, y) => { return x.LastWriteTime.CompareTo(y.LastWriteTime); });
                    foreach (var fileInfo in dirInfo)
                    {
                        string head = fileInfo.Name.Substring(0, fileInfo.Name.Length - 5);
                        Type t = JsonConvert.DeserializeObject<Type>(File.ReadAllText(fileInfo.FullName));
                        var tag = Activator.CreateInstance(t) as ToolBase;
                        var tempTree = tag.CreateTreeView(head);
                        tag.ToolInVision = VisionStep;
                        tag.CameraId = CameraID;
                        tempTree.Tag = tag;
                        tempTree.PreviewMouseDoubleClick += Tree_PreviewMouseDoubleClick;
                        ToolTreeView.Items.Add(tempTree);
                    }
                    MessageBox.Show("保存成功");
                }
            }
        }
    }
}
