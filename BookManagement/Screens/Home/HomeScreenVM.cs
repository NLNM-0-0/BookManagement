using DocumentFormat.OpenXml.InkML;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;

namespace BookManagement
{
    internal class HomeScreenVM : BaseViewModel
    {
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }
        public ChartValues<double> Values1 { get; set; }
        public ChartValues<double> Values2 { get; set; }
        public HomeScreenVM()
        {
            Values1 = new ChartValues<double> { 3, 4, 6, 3, 2, 6 };
            Values2 = new ChartValues<double> { 5, 3, 5, 7, 3, 9 };

            Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May" };
            YFormatter = value => value.ToString("C");
        }
    }
}
