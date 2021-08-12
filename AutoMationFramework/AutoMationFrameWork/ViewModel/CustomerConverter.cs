using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;
using MotionIO;

namespace AutoMationFrameWork.ViewModel
{
    public class HomeModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int result = 0;
            if (value != null && int.TryParse(value.ToString(), out result))
            {
              return (HomeMode)result;
            }

            return (HomeMode)result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
           int result = (int) value;
            return result;
        }
    }

    public class BoolValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool result;
            if (value != null&&bool.TryParse(value.ToString(),out result))
            {
                return !result;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = (bool) value;
            return !result;
        }
    }
    public class TreeViewLineConverter : IMultiValueConverter
    {


        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double height = (double)values[0];

            TreeViewItem item = values[2] as TreeViewItem;
            ItemsControl ic = ItemsControl.ItemsControlFromItemContainer(item);
            bool isLastOne = ic.ItemContainerGenerator.IndexFromContainer(item) == ic.Items.Count - 1;

            Rectangle rectangle = values[3] as Rectangle;
            if (isLastOne)
            {
                rectangle.VerticalAlignment = VerticalAlignment.Top;
                return 9.0;
            }
            else
            {
                rectangle.VerticalAlignment = VerticalAlignment.Stretch;
                return double.NaN;
            }


        }



        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
