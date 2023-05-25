using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BookManagement
{
    public class NumberAndUnitPriceToTotalPriceConverter : IMultiValueConverter
    {
        public static NumberAndUnitPriceToTotalPriceConverter Instance => new NumberAndUnitPriceToTotalPriceConverter();
        object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if(values.Length != 2 || values == null) return null;
            decimal unitPrice = (decimal)values[0];
            int number = (int)values[1];
            decimal money = unitPrice * number;
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
            return $"{money.ToString("#,###", cul.NumberFormat)} đ";
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
