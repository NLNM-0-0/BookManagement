using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;

namespace BookManagement {
    public class DrawerVM : BaseViewModel {
        private readonly NavigationStore _navigationStore;

        public ObservableCollection<ButtonItem> ButtonItems => NormalButtonCreate();

        #region Props
        private int selectedIndex = 0;
        public int SelectedIndex {
            get { return selectedIndex; }
            set {
                if(selectedIndex != value) {
                    selectedIndex = value;
                    OnPropertyChanged();
                }
            }
        }
        private int prevSelected { get; set; } = 0;

        private bool canReload;
        public bool CanReload {
            get { return canReload; }
            set { canReload = value; OnPropertyChanged(); }
        }
        #endregion

        #region Command
        public ICommand OnChangeScreen { get; set; }
        public ICommand OnButtonClick { get; }
        #endregion

        public DrawerVM() {
            _navigationStore = NavigationStore.instance;
            _navigationStore.CurrentVMChanged += OnScreenChange;

            CanReload = true;

            #region Command define

            OnChangeScreen = new RelayCommand<object>((p) => {
                if(CanReload) return true;
                if(!CanReload && SelectedIndex != 4) {
                    CanReload = true;
                    return true;
                }
                return false;
            },
            (p) => {
                if(DialogHost.IsDialogOpen("Main"))
                    DialogHost.Close("Main");
                if(SelectedIndex == 0) {
                    ChangeIndex(NavigateProvider.HomeScreen());
                }
                else if(SelectedIndex == 1) {
                    ChangeIndex(NavigateProvider.SaleBookScreen());
                }
                else if(SelectedIndex == 2) {
                    ChangeIndex(NavigateProvider.BookManagementScreen());
                }
                else if(SelectedIndex == 3) {
                    ChangeIndex(NavigateProvider.ImportBookManagementScreen());
                }
                else if(SelectedIndex == 4) {
                    ChangeIndex(NavigateProvider.BillManagementScreen());
                }
                else if(SelectedIndex == 5) {
                    ChangeIndex(NavigateProvider.DebtManagementScreen());
                }
                else if (SelectedIndex == 6)
                {
                    ChangeIndex(NavigateProvider.SettingScreen());
                }
                else {
                    //Unknown
                }
                prevSelected = SelectedIndex;

            });

            OnButtonClick = new RelayCommand<object>(p => {
                if((int)p != SelectedIndex) {
                    SelectedIndex = (int)p;
                    return false;
                }
                return true;
            }, p => {
                NavigationStore.instance.CurrentViewModel = null;
                SelectedIndex = -1;
                SelectedIndex = (int)p;
                var stackScreen = NavigationStore.instance.stackScreen;

                if(stackScreen.Count > 1 &&
                    stackScreen[stackScreen.Count - 1].Item1.GetType().Equals(
                    stackScreen[stackScreen.Count - 2].Item1.GetType()) ) {
                    stackScreen.RemoveAt(stackScreen.Count - 1);
                }
            });
            #endregion
        }

        #region Outside change handle
        private void OnScreenChange() {
            if(NavigationStore.instance.CurrentViewModel == null) return;
            Type type = NavigationStore.instance.CurrentViewModel.GetType();
            switch(type.Name) {
                case "HomeScreenVM":
                    SelectedIndex = 0;
                    break;
                case "SaleBookScreenVM":
                    SelectedIndex = 1;
                    break;
                case "BookManagementScreenVM":
                    SelectedIndex = 2;
                    break;
                case "ImportBookManagementScreenVM":
                    SelectedIndex = 3;
                    break;
                case "BillManagementScreenVM":
                    SelectedIndex = 4;
                    break;
                case "DebtManagementScreenVM":
                    SelectedIndex = 5;
                    break;
                case "SettingScreenVM":
                    SelectedIndex = 6;
                    break;
            }
        }
        #endregion

        void ChangeIndex(INavigationService nav, object o = null) {
            var _navStore = NavigationStore.instance;
            if(!Internet.IsConnected) {
                _navStore.stackScreen.Add(new Tuple<INavigationService, object>(nav, o));
                return;
            }
            if(_navStore.currentViewModel == null || nav.GetViewModel() != _navStore.CurrentViewModel.GetType()) {
                if(o != null)
                    nav.Navigate(o);
                else nav.Navigate();
            }
        }

        private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs) {
            SelectedIndex = prevSelected;
        }

        private ObservableCollection<ButtonItem> NormalButtonCreate() {
            return new ObservableCollection<ButtonItem> {
                    new ButtonItem("Home", "Home", 0),
                    new ButtonItem("CreditCardSettings", "Sale", 1),
                    new ButtonItem("Book", "Book", 2),
                    new ButtonItem("Import", "Import", 3),
                    new ButtonItem("PageLayoutFooter", "Bill", 4),
                    new ButtonItem("CashRefund", "Debt", 5),
                    new ButtonItem("Cog", "Setting", 6),
                };
        }
    }

}
public class ButtonItem {
    public string Icon { get; set; }
    public string Text { get; set; }
    public int Index { get; set; }
    public ButtonItem(string icon, string text, int index) {
        Icon=icon;
        Text=text;
        Index=index;
    }
}
