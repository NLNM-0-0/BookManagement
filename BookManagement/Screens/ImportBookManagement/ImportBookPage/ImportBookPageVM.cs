using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BookManagement
{
    public class ImportBookPageVM : BaseViewModel
    {
        #region Commands
        public ICommand OnAddBoook { get; set; }
        public ICommand OnDoneImport { get; set; }
        public ICommand OnChangeSearchBy { get; set; }
        #endregion

        #region Props
        private string searchBy;
        public string SearchBy
        {
            get { return searchBy; }
            set
            {
                searchBy = value;
                OnPropertyChanged();
            }
        }
        private string searchByValue;
        public string SearchByValue
        {
            get { return searchByValue; }
            set
            {
                searchByValue = value;
                OnPropertyChanged();
            }
        }
        #endregion
        public ImportBookPageVM()
        {
            
            SearchBy = "Id";
            #region Define Commands
            OnAddBoook = new RelayCommandWithNoParameter(async () =>
            {
                MainViewModel.SetLoading(true);
                AddExistingBook addExistingBook = new AddExistingBook();
                addExistingBook.DataContext = new AddExistingBookVM();
                MainViewModel.SetLoading(false);
                await DialogHost.Show(addExistingBook, "Main");
            });
            OnDoneImport = new RelayCommandWithNoParameter(() =>
            {
                NavigateProvider.ImportBookManagementScreen().Navigate();
            });
            #endregion

        }
    }
}
