using BookManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookManagement
{
    public class DebtCollectPdfVM : BaseViewModel
    {
        private PHIEUTHUNO debtCollect;
        public PHIEUTHUNO DebtCollect
        {
            get => debtCollect;
            set
            {
                debtCollect = value;
                OnPropertyChanged();
            }
        }
        public DebtCollectPdfVM(PHIEUTHUNO debtCollect) 
        {
            DebtCollect = debtCollect;
        }
    }
}
