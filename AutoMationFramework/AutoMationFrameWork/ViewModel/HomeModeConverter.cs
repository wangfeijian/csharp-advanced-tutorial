using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
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
}
