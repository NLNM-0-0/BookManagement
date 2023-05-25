using BookManagement.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BookManagement
{
    public class DecimalToMoneyConveter : IValueConverter
    {
        public static DecimalToMoneyConveter Instance => new DecimalToMoneyConveter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            decimal money = (decimal)value;
            if(money == 0)
            {
                return "0 đ";
            }    
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
            return $"{money.ToString("#,###", cul.NumberFormat)} đ";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
