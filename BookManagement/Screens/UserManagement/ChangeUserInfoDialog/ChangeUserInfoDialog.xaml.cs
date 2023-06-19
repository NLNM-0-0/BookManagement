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
    /// Interaction logic for ChangeUserInfoDialog.xaml
    /// </summary>
    public partial class ChangeUserInfoDialog : UserControl
    {
        public ChangeUserInfoDialog()
        {
            InitializeComponent();
            genderComboBox.ItemsSource = new List<String>() { "Nữ", "Nam", "Khác" };
        }
    }
}
