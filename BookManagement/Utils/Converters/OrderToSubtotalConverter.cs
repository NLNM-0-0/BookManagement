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
    internal class OrderToSubtotalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var item = (CHITIETHOADON)value;
            if (item != null)
            {
                var result = item.SoLuong * item.DonGia;
                if (result == 0)
                {
                    return "0 đ";
                }
                CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
                return $"{result.ToString("#,###", cul.NumberFormat)} đ";
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
