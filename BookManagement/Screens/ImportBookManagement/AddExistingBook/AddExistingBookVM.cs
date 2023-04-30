using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BookManagement
{
    
    public class AddExistingBookVM : BaseViewModel
    {
        #region Commands
        public ICommand OnAddNewBoook { get; set; }
        private System.Windows.Controls.UserControl PreviousItem;
        #endregion
        public AddExistingBookVM()
        {
            OnAddNewBoook = new RelayCommandWithNoParameter(async () => 
            {
                PreviousItem = MainViewModel.UpdateDialog("Main");
                MainViewModel.SetLoading(true);
                AddNewBook addNewBookDialog = new AddNewBook();
                addNewBookDialog.DataContext = new AddNewBookVM()
                {
                    CloseDialogCommand = new RelayCommandWithNoParameter(() =>
                    {
                        DialogHost.CloseDialogCommand.Execute(null, null);
                        if (PreviousItem != null)
                        {
                            DialogHost.Show(PreviousItem, "Main");
                        }
                    })
                };
                MainViewModel.SetLoading(false);
                await DialogHost.Show(addNewBookDialog, "Main");
            });
        }
    }
}
