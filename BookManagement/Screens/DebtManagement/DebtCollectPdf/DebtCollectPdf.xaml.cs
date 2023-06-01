using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BookManagement
{
    /// <summary>
    /// Interaction logic for DebtCollectPdf.xaml
    /// </summary>
    public partial class DebtCollectPdf : UserControl
    {
        public DebtCollectPdf()
        {
            InitializeComponent();
        }
        public void Print()
        {
            if (DataContext != null && DataContext.GetType() == typeof(DebtCollectPdfVM))
            {
                try
                {
                    this.IsEnabled = false;
                    PrintDialog printDialog = new PrintDialog();
                    if (printDialog.ShowDialog() == true)
                    {
                        printDialog.PrintVisual(grid, $"debt_{(DataContext as DebtCollectPdfVM).DebtCollect.MaPhieuThu}");
                    }
                }
                finally
                {
                    this.IsEnabled = true;
                }
            }
        }
    }
}
