using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BookManagement
{
    public class AddNewBookVM : BaseViewModel 
    { 
        #region
        public ICommand CloseDialogCommand { get; set; }
        #endregion
        
        public AddNewBookVM()
        {

        }
    }
}
