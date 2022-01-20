using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;

namespace OpenCvSharpTool
{
    /// <summary>
    /// ToolBase.xaml 的交互逻辑
    /// </summary>
    public abstract partial class ToolBase : UserControl
    {
        private int i;
        /// <summary>
        /// 输入参数
        /// </summary>
        public Dictionary<string, object> InputParams;

        /// <summary>
        /// 输出参数
        /// </summary>
        public Dictionary<string, object> OutputParams;

        /// <summary>
        /// 工具所绑定的窗体
        /// </summary>
        public Window ToolWindow { get; set; }

        /// <summary>
        /// 工具运行
        /// </summary>
        public virtual void Run()
        {

        }

        /// <summary>
        /// 工具描述
        /// </summary>
        public string ToolDesStr
        {
            get { return (string)GetValue(ToolDesStrProperty); }
            set { SetValue(ToolDesStrProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToolDesStr.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToolDesStrProperty =
            DependencyProperty.Register(nameof(ToolDesStr), typeof(string), typeof(ToolBase));

        /// <summary>
        /// 图标内容
        /// 根据iconfont所生成，需要定义指定的iconfont字体
        /// </summary>
        public string ToolIcon
        {
            get { return (string)GetValue(ToolIconProperty); }
            set { SetValue(ToolIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToolIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToolIconProperty =
            DependencyProperty.Register(nameof(ToolIcon), typeof(string), typeof(ToolBase));


        public ToolBase()
        {
            InitializeComponent();
        }

        public virtual void UIElement_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock tempBlock = e.OriginalSource as TextBlock;
            if (tempBlock != null)
            {
                if (tempBlock.Text == ToolDesStr)
                {
                    ToolWindow?.ShowDialog();
                }
            }
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            StackPanel stack = Parent as StackPanel;
            GroupBox groupBox = stack.Parent as GroupBox;
            ToolBoxControl tool = groupBox.Parent as ToolBoxControl;
            Panel panel = tool.Parent as Panel;
            ToolRunBox toolRun = panel.FindName("ToolRunBoxItem") as ToolRunBox;
            TreeView toolTreeView = toolRun.FindName("ToolTreeView") as TreeView;

            TreeViewItem tree = CreateTreeView(ToolDesStr);

            ToolBase toolBase = Activator.CreateInstance(GetType()) as ToolBase;
            toolBase.ContextMenu = null;
            tree.Tag = toolBase;
            tree.PreviewMouseDoubleClick += Tree_PreviewMouseDoubleClick;
            toolTreeView.Items.Add(tree);
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
    }
}
