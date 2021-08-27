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

namespace OpenCvSharpTool
{
    /// <summary>
    /// ToolBoxControl.xaml 的交互逻辑
    /// </summary>
    public partial class ToolBoxControl : UserControl
    {
        public ToolBoxControl()
        {
            InitializeComponent();
        }

        private void ToolBoxControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            //获取此程序下所有程序集并实例化实现某个基类或接口的类
            //var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            //foreach (var assembly in assemblies)
            //{
            //    var item = assembly.GetTypes().Where(t => typeof(ToolBase).IsAssignableFrom(t))
            //     .Where(t => !t.IsAbstract && t.IsClass).Select(t => (ToolBase)Activator.CreateInstance(t));
            //    foreach (var toolBase in item)
            //    {
            //        ToolStackPanel.Children.Add(toolBase);
            //    }
            //}

            var item = typeof(ToolBoxControl).Assembly.GetTypes().Where(t => typeof(ToolBase).IsAssignableFrom(t))
                .Where(t => !t.IsAbstract && t.IsClass).Select(t => (ToolBase)Activator.CreateInstance(t));
            foreach (var toolBase in item)
            {
                ToolStackPanel.Children.Add(toolBase);
            }
        }
    }
}
