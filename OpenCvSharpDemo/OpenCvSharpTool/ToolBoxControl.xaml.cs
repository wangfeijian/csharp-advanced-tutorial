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
        private bool _isDown;
        private bool _isDragging;
        private Point _startPoint;
        private UIElement _realDragSource;
        private UIElement _dummyDragSource = new UIElement();

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

        private void ToolStackPanel_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source != ToolStackPanel)
            {
                _isDown = true;
                _startPoint = e.GetPosition(ToolStackPanel);
            }
        }

        private void ToolStackPanel_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDown = false;
            _isDragging = false;
            _realDragSource?.ReleaseMouseCapture();
        }

        private void ToolStackPanel_OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
           bool posInTargetX = Math.Abs(e.GetPosition(ToolStackPanel).X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance;
           bool posInTargetY = Math.Abs(e.GetPosition(ToolStackPanel).Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance;
            if (_isDown)
            {
                if (!_isDragging && (posInTargetX || posInTargetY))
                {
                    _isDragging = true;
                    _realDragSource = e.Source as UIElement;
                    _realDragSource.CaptureMouse();
                    DragDrop.DoDragDrop(_dummyDragSource, new DataObject("UIElement", e.Source, true),DragDropEffects.Move|DragDropEffects.Copy);
                }
            }
        }

        private void ToolStackPanel_OnDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("UIElement"))
            {
                e.Effects = DragDropEffects.Move;
            }
        }

        private void ToolStackPanel_OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("UIElement"))
            {
                UIElement dropTarget = e.Source as UIElement;
                int dropTargetIndex = -1, i = 0;
                foreach (UIElement uiElement in ToolStackPanel.Children)
                {
                    if (uiElement.Equals(dropTarget))
                    {
                        dropTargetIndex = i;
                        break;
                    }

                    i++;
                }

                if (dropTargetIndex!=-1)
                {
                    ToolStackPanel.Children.Remove(_realDragSource);
                    ToolStackPanel.Children.Insert(dropTargetIndex,_realDragSource);
                }

                _isDown = false;
                _isDragging = false;
                _realDragSource.ReleaseMouseCapture();
            }
        }
    }
}
