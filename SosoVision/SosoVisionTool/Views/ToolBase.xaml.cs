using MaterialDesignThemes.Wpf;
using SosoVisionTool.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for ToolBase.xaml
    /// </summary>
    public abstract partial class ToolBase : UserControl
    {
        public TreeViewItem ToolItem { get; set; }
        public TreeViewItem ToolInputItem { get; set; }
        public TreeViewItem ToolOutputItem { get; set; }

        /// <summary>
        /// 工具在哪个视觉流程中
        /// </summary>
        public string ToolInVision { get; set; }
        /// <summary>
        /// 相机ID
        /// </summary>
        public string CameraId { get; set; }
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

        public virtual void UIElement_OnPreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TextBlock tempBlock = e.OriginalSource as TextBlock;
            if (tempBlock != null)
            {
                if (tempBlock.Text.Contains(ToolDesStr))
                {
                    ToolWindow?.ShowDialog();
                }
            }
        }

        /// <summary>
        /// 工具所绑定的窗体
        /// </summary>
        public Window ToolWindow { get; set; }

        /// <summary>
        /// 工具运行
        /// </summary>
        public void Run(ToolBase tool, ref bool result)
        {
            var viewModel = DataContext as IToolBaseViewModel;
            viewModel?.Run(tool, ref result);
        }

        /// <summary>
        /// 创建输入输出参数
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public abstract TreeViewItem CreateTreeView(string name);

        public abstract object GetDataContext(string file);

        /// <summary>
        /// 添加输入输出
        /// </summary>
        /// <param name="item"></param>
        public void AddInputOutputTree(TreeViewItem item, bool isInput)
        {
            if (isInput)
            {
                foreach (var input in ToolInputItem.Items)
                {
                    var inItem = input as TreeViewItem;
                    if (inItem.Header == item.Header)
                    {

                        inItem.ToolTip = item.ToolTip;
                        return;
                    }
                }
                ToolInputItem.Items.Add(item);
            }
            else
            {
                foreach (var output in ToolOutputItem.Items)
                {
                    var outItem = output as TreeViewItem;
                    if (outItem.Header == item.Header)
                    {

                        outItem.ToolTip = item.ToolTip;
                        return;
                    }
                }
                ToolOutputItem.Items.Add(item);
            }
        }

        public object AddInOutTreeViewItem(bool isIn)
        {
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;
            PackIcon packIcon = new PackIcon();
            packIcon.Kind = isIn ? PackIconKind.ArrowRight : PackIconKind.ArrowLeft;
            packIcon.VerticalAlignment = VerticalAlignment.Center;
            TextBlock textBlock = new TextBlock();
            textBlock.Margin = new Thickness(10, 0, 0, 0);
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            textBlock.Text = isIn ? "输入" : "输出";
            stackPanel.Children.Add(packIcon);
            stackPanel.Children.Add(textBlock);
            return stackPanel;
        }
    }
}
