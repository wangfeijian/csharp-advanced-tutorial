using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SosoVisionTool.Views
{
    /// <summary>
    /// Interaction logic for ToolControlBox.xaml
    /// </summary>
    public partial class ToolControlBoxView : UserControl
    {
        private bool _isDown;
        private bool _isDragging;
        private Point _startPoint;
        private UIElement _realDragSource;
        private UIElement _dummyDragSource = new UIElement();

        public ToolControlBoxView()
        {
            InitializeComponent();
        }

        private void ToolControl_Loaded(object sender, RoutedEventArgs e)
        {
            var item = typeof(ToolControlBoxView).Assembly.GetTypes().Where(t => typeof(ToolBase).IsAssignableFrom(t))
               .Where(t => !t.IsAbstract && t.IsClass).Select(t => (ToolBase)Activator.CreateInstance(t));
            foreach (var toolBase in item)
            {
                StackPanelToolList.Children.Add(toolBase);
            }
        }

        private void ToolStackPanel_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source != StackPanelToolList)
            {
                _isDown = true;
                _startPoint = e.GetPosition(StackPanelToolList);
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
            bool posInTargetX = Math.Abs(e.GetPosition(StackPanelToolList).X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance;
            bool posInTargetY = Math.Abs(e.GetPosition(StackPanelToolList).Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance;
            if (_isDown)
            {
                if (!_isDragging && (posInTargetX || posInTargetY))
                {
                    _isDragging = true;
                    _realDragSource = e.Source as UIElement;
                    _realDragSource.CaptureMouse();
                    DragDrop.DoDragDrop(_dummyDragSource, new DataObject("UIElement", e.Source, true), DragDropEffects.Move | DragDropEffects.Copy);
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
                foreach (UIElement uiElement in StackPanelToolList.Children)
                {
                    if (uiElement.Equals(dropTarget))
                    {
                        dropTargetIndex = i;
                        break;
                    }

                    i++;
                }

                if (dropTargetIndex != -1)
                {
                    StackPanelToolList.Children.Remove(_realDragSource);
                    StackPanelToolList.Children.Insert(dropTargetIndex, _realDragSource);
                }

                _isDown = false;
                _isDragging = false;
                _realDragSource.ReleaseMouseCapture();
            }
        }
    }
}
