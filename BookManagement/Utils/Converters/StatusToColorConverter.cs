using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace BookManagement
{
    internal class StatusToColorConverter : IValueConverter
    {
        public static StatusToColorConverter Instance => new StatusToColorConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var item = (bool)value;
            if (item == true)
            {
                return (System.Windows.Media.Brush)new SolidColorBrush(System.Windows.Media.Color.FromRgb(42, 169, 82));
            }
            return (System.Windows.Media.Brush)new SolidColorBrush(System.Windows.Media.Color.FromRgb(219, 48, 34));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
}
