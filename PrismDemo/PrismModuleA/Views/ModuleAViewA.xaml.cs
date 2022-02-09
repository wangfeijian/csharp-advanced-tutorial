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
using Prism.Events;
using PrismModuleA.Events;

namespace PrismModuleA.Views
{
    /// <summary>
    /// ViewA.xaml 的交互逻辑
    /// </summary>
    public partial class ModuleAViewA : UserControl
    {
        public ModuleAViewA(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            eventAggregator.GetEvent<MessageEvent>().Subscribe(arg => { MessageBox.Show($"接收到消息：{arg}"); });
        }
    }
}
