using BookManagement.Models;
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
    internal class AuthorsToTextConverter : IValueConverter
    {
        public static AuthorsToTextConverter Instance => new AuthorsToTextConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            ICollection<TACGIA> authors = (ICollection<TACGIA>)value;
            List<string> authorNames = authors.Select(author => author.TenTacGia).ToList();
            string namesString = string.Join(", ", authorNames);
            return namesString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
