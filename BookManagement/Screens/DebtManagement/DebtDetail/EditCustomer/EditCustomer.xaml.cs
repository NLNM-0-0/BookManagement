using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace BookManagement
{
    /// <summary>
    /// Interaction logic for EditCustomer.xaml
    /// </summary>
    public partial class EditCustomer : UserControl
    {
        public EditCustomer()
        {
            InitializeComponent();
            genderComboBox.ItemsSource = new List<String>() { "Nữ", "Nam", "Khác" };
        }
    }
}