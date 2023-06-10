using System;
using System.Globalization;
using System.Windows.Data;

namespace BookManagement { 
    public class DecimalToMoneyWithoutCurrencyConverter : IValueConverter
    {
        public static DecimalToMoneyWithoutCurrencyConverter Instance => new DecimalToMoneyWithoutCurrencyConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            decimal money = (decimal)value;
            if (money == 0)
            {
                return "0";
            }
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
            return money.ToString("#,###", cul.NumberFormat);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
