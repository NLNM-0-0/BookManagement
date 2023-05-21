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
    internal class StatusToTextConverter : IValueConverter
    {
        public static StatusToTextConverter Instance => new StatusToTextConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var item = (bool)value;
            if (item == true)
            {
                return "Đang hoạt động";
            }
            return "Không hoạt động";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
