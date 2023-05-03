using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BookManagement
{
    
    public class ImportBookManagementScreenVM : BaseViewModel
    {
        #region Commands
        public ICommand OnChangeScreen { get; set; }
        #endregion
        public ImportBookManagementScreenVM()
        {
            OnChangeScreen = new RelayCommandWithNoParameter(() =>
            {
                NavigateProvider.ImportBookPage().Navigate();
            });
        }
        
    }
}
