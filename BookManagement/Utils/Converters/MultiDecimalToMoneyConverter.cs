using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace BookManagement
{
    internal class MultiDecimalToMoneyConverter : IMultiValueConverter
    {
        public static MultiDecimalToMoneyConverter Instance => new MultiDecimalToMoneyConverter();
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 2 || values[0] == null || values[1] == null || values[1] == DependencyProperty.UnsetValue) return null;
            decimal value1 = Decimal.Parse(values[0].ToString());
            decimal value2 = Decimal.Parse(values[1].ToString());
            decimal money = value1 * value2;
            if (money == 0)
            {
                return "0 đ";
            }
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
            return $"{money.ToString("#,###", cul.NumberFormat)} đ";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
