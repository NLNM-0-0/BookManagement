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
using BookManagement.Models;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;

namespace BookManagement {
    public class DrawerVM : BaseViewModel {
        private readonly AccountStore _accountStore;
        private readonly NavigationStore _navigationStore;

        public NHANVIEN CurrentUser => _accountStore?.CurrentAccount;
        public ObservableCollection<ButtonItem> ButtonItems => ButtonCreate();

        #region Props
        private int selectedIndex = -1;
        public int SelectedIndex {
            get { return selectedIndex; }
            set {
                if(selectedIndex != value) {
                    selectedIndex = value;
                    OnPropertyChanged();
                }
            }
        }
        private int prevSelected { get; set; } = -1;

        private bool canReload;
        public bool CanReload {
            get { return canReload; }
            set { canReload = value; OnPropertyChanged(); }
        }
        #endregion

        #region Command
        public ICommand OnChangeScreen { get; set; }
        public ICommand OnButtonClick { get; }
        public ICommand OnThemeMouseOver { get; set; }
        public ICommand OnThemeMouseLeave { get; set; }
        #endregion

        public DrawerVM() {
            _accountStore = AccountStore.instance;
            _accountStore.AccountChanged += OnAccountChange;

            _navigationStore = NavigationStore.instance;
            _navigationStore.CurrentVMChanged += OnScreenChange;

            CanReload = true;
            #region Command define

            OnChangeScreen = new RelayCommand<object>((p) => {
                if(CanReload) return true;
                if(!CanReload) {
                    CanReload = true;
                    return true;
                }
                return false;
            },
            (p) => {
                if (DialogHost.IsDialogOpen("Main"))
                    DialogHost.Close("Main");
                ChangeScreen();
                prevSelected = SelectedIndex;

            });

            OnButtonClick = new RelayCommand<object>(p => {
                if((int)p != SelectedIndex) {
                    SelectedIndex = (int)p;
                    return true;
                }
                return false;
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

            //Theme popup Handle
            var timer = new DispatcherTimer();
            OnThemeMouseOver = new RelayCommand<object>(p => {
                return true;
            }, p => {
                ThemePopUp themePopUp = (ThemePopUp)p;
                timer.Stop();
                timer.Interval = TimeSpan.FromMilliseconds(200);
                timer.Tick += delegate {
                    themePopUp.Visibility = Visibility.Visible;
                };
                timer.Start();
            });

            OnThemeMouseLeave = new RelayCommand<object>(p => {
                return true;
            }, p => {
                ThemePopUp themePopUp = (ThemePopUp)p;
                timer.Stop();
                timer.Interval = TimeSpan.FromMilliseconds(300);
                timer.Tick += delegate {
                    if (themePopUp.IsMouseOver == false)
                        themePopUp.Visibility = Visibility.Collapsed;
                };
                timer.Start();
            });
            #endregion
        }

        #region Outside change handle
        private void OnScreenChange() {
            if(NavigationStore.instance.CurrentViewModel == null) return;
            Type type = NavigationStore.instance.CurrentViewModel.GetType();
            switch(type.Name) {
                case "SaleBookScreenVM":
                    SelectedIndex = 0;
                    break;
                case "BookManagementScreenVM":
                    SelectedIndex = 1;
                    break;
                case "ImportBookManagementScreenVM":
                    SelectedIndex = 2;
                    break;
                case "BillManagementScreenVM":
                    SelectedIndex = 3;
                    break;
                case "DebtManagementScreenVM":
                    SelectedIndex = 4;
                    break;
                case "HomeScreenVM":
                    SelectedIndex = 5;
                    break;
                case "SettingScreenVM":
                    SelectedIndex = 6;
                    break;
                case "StaffManagementVM":
                    SelectedIndex = 7;
                    break;
                case "NoAccessScreenVM":
                    SelectedIndex = 8;
                    break;
            }
        }
        #endregion

        void ChangeIndex(INavigationService nav, object o = null) {
            var _navStore = NavigationStore.instance;
            if(_navStore.currentViewModel == null || nav.GetViewModel() != _navStore.CurrentViewModel.GetType()) {
                if(o != null)
                    nav.Navigate(o);
                else nav.Navigate();
            }
        }
        void ChangeScreen()
        {
            if (SelectedIndex == 0)
            {
                ChangeIndex(NavigateProvider.SaleBookScreen());
                
            }
            else if (SelectedIndex == 1)
            {
                ChangeIndex(NavigateProvider.BookManagementScreen());
            }
            else if (SelectedIndex == 2)
            {
                ChangeIndex(NavigateProvider.ImportBookManagementScreen());
            }
            else if (SelectedIndex == 3)
            {
                ChangeIndex(NavigateProvider.BillManagementScreen());
            }
            else if (SelectedIndex == 4)
            {
                ChangeIndex(NavigateProvider.DebtManagementScreen());
            }
            else if (SelectedIndex == 5)
            {
                ChangeIndex(NavigateProvider.HomeScreen());
            }
            else if (SelectedIndex == 6)
            {
                ChangeIndex(NavigateProvider.SettingScreen());
            }
            else if (selectedIndex == 7)
            {
                ChangeIndex(NavigateProvider.StaffManagementScreen());
            }
            else if (selectedIndex == 8)
            {
                ChangeIndex(NavigateProvider.NoAccessScreen());
            }
        }
        private void OnAccountChange()
        {
            OnPropertyChanged(nameof(CurrentUser));
            OnPropertyChanged(nameof(ButtonItems));
            changeDefaultSelectedIndex();
            ChangeScreen();
        }
        
        private void changeDefaultSelectedIndex()
        {
            int i = 0;
            for (; i < ButtonItems.Count - 1; i++)
            {
                if (ButtonItems[i].IsNotCollapsed)
                {
                    break;
                }
            }
            SelectedIndex = prevSelected = i;
        }
        public override void Dispose()
        {
            _accountStore.AccountChanged -= OnAccountChange;
            base.Dispose();
        }

        private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs) {
            SelectedIndex = prevSelected;
        }

        private ObservableCollection<ButtonItem> ButtonCreate() {
            ObservableCollection<ButtonItem> buttonItems = new ObservableCollection<ButtonItem>();
            if (CurrentUser == null)
            {
                return buttonItems;
            }
            else
            {
                List<CHUCNANG> accessList = CurrentUser.NHOMNGUOIDUNG.CHUCNANGs.ToList();
                buttonItems.Add(new ButtonItem("CreditCardSettings", "Bán sách", 0, 
                    isExistAccess(accessList, AppEnum.LapHoaDonBanSach)));
                buttonItems.Add(new ButtonItem("Book", "Sách", 1, 
                    isExistAccess(accessList, AppEnum.TraCuuSach)));
                buttonItems.Add(new ButtonItem("Import", "Nhập sách", 2, 
                    isExistAccess(accessList, AppEnum.LapPhieuNhapSach) ||
                      isExistAccess(accessList, AppEnum.TraCuuPhieuNhapSach)));
                buttonItems.Add(new ButtonItem("PageLayoutFooter", "Hóa đơn", 3, 
                    isExistAccess(accessList, AppEnum.TraCuuHoaDonBanSach)));
                buttonItems.Add(new ButtonItem("CashRefund", "Khách hàng", 4, 
                    isExistAccess(accessList, AppEnum.TraCuukhachHang)));
                buttonItems.Add(new ButtonItem("CalendarText", "Báo cáo", 5,
                    isExistAccess(accessList, AppEnum.LapBaoCaoThang)));
                buttonItems.Add(new ButtonItem("Cog", "Cài đặt", 6, 
                    isExistAccess(accessList, AppEnum.ThayDoiQuiDinh) || 
                    isExistAccess(accessList, AppEnum.PhanQuyen)));
                buttonItems.Add(new ButtonItem("Account", "Tài khoản", 7, 
                    isExistAccess(accessList, AppEnum.ThayDoiMatKhau) ||
                    isExistAccess(accessList, AppEnum.TraCuuNhanVien)));
                buttonItems.Add(new ButtonItem("Exclamation", "NoAccess", 8, false));
                return buttonItems;
            }    
        }

        private bool isExistAccess(List<CHUCNANG> accessList, String  access)
        {
            if(accessList.Where(a => a.MaChucNang.Equals(access)).Count() > 0)
            {
                return true;
            }
            return false;
        }
    }

}
public class ButtonItem {
    public string Icon { get; set; }
    public string Text { get; set; }
    public int Index { get; set; }
    public bool IsNotCollapsed { get; set; }
    public ButtonItem(string icon, string text, int index, bool isNotCollapsed) {
        Icon=icon;
        Text=text;
        Index=index;
        IsNotCollapsed = isNotCollapsed;
    }
}
