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
    }
}
