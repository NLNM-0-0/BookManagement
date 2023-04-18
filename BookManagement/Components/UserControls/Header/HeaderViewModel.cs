using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThreadTimer = System.Threading.Timer;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;

namespace BookManagement
{
    public class HeaderViewModel : BaseViewModel
    {
        #region Commands
        public ICommand SignOutCommand { get; set; }
        public ICommand OnBack { get; set; }
        #endregion

        public HeaderViewModel()
        {
            MainViewModel.SetLoading(true);

            SignOutCommand = new ImmediateCommand<object>(async o => {
                NavigateProvider.LoginScreenHandle(true);
            });

            OnBack = new RelayCommand<object>(p => NavigationStore.instance.stackScreen.Count > 1, p =>
            {
                NavigateProvider.Back();
            });

            MainViewModel.SetLoading(false);
        }
    }
}
