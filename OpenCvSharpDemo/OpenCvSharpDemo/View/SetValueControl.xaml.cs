using System;
using System.Collections.Generic;
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
using OpenCvSharpDemo.ViewModel;

namespace OpenCvSharpDemo.View
{
    /// <summary>
    /// SetValueControl.xaml 的交互逻辑
    /// </summary>
    public partial class SetValueControl : UserControl
    {
        private bool _isMouseLeftButtonDown;
        public SetValueControl()
        {
            InitializeComponent();
        }

        public static readonly RoutedEvent ValueChangeEvent = EventManager.RegisterRoutedEvent(nameof(ValueChangeEvent), RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventArgs<Object>), typeof(SetValueControl));

        /// <summary>
        /// 处理各种路由事件的方法 
        /// </summary>
        public event RoutedEventHandler ValueChange
        {
            //将路由事件添加路由事件处理程序
            add { AddHandler(ValueChangeEvent, value); }
            //从路由事件处理程序中移除路由事件
            remove { RemoveHandler(ValueChangeEvent, value); }
        }

        public string ParamDescribe
        {
            get { return (string)GetValue(ParamDescribeProperty); }
            set { SetValue(ParamDescribeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ParamDescribe.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ParamDescribeProperty =
            DependencyProperty.Register(nameof(ParamDescribe), typeof(string), typeof(SetValueControl));

        public int ParamValue
        {
            get { return (int)GetValue(ParamValueProperty); }
            set { SetValue(ParamValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ParamValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ParamValueProperty =
            DependencyProperty.Register(nameof(ParamValue), typeof(int), typeof(SetValueControl));


        private void SubButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (ParamValue == 0)
            {
                return;
            }

            ParamValue--;
        }

        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            ParamValue++;
        }

        private void SubButton_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _isMouseLeftButtonDown = true;
                while (_isMouseLeftButtonDown)
                {
                    SubButton_OnClick(null, null);
                    MainViewModel.DispatcherHelper.Delay(20);
                }
            }
        }

        private void OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isMouseLeftButtonDown = false;
        }

        private void AddButton_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _isMouseLeftButtonDown = true;
                while (_isMouseLeftButtonDown)
                {
                    AddButton_OnClick(null, null);
                    MainViewModel.DispatcherHelper.Delay(50);
                }
            }
        }

        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            ValueChanged(sender,e);
        }

        private void ValueChanged(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            int temp = 0;
            int.TryParse(textBox.Text, out temp);

            if (temp!=0)
            {
                ParamValue = temp;
            }

            //定义传递参数
            RoutedEventArgs args2 = new RoutedEventArgs(ValueChangeEvent, this);
            //引用自定义路由事件
            this.RaiseEvent(args2);
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            _isMouseLeftButtonDown = false;
        }
    }
}
