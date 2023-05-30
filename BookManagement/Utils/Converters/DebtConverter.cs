
using BookManagement.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace BookManagement
{
    internal class DebtConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var item = (CHITIETBAOCAOCONGNO)value;
            if (item != null)
            {
                var result = item.NoDau + item.PhatSinh - item.NoCuoi;
                decimal money = (decimal)result;
                if (money == 0)
                {
                    return "0 đ";
                }
                CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
                return $"{money.ToString("#,###", cul.NumberFormat)} đ";
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
