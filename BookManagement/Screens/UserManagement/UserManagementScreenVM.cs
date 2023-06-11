using BookManagement.Models;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BookManagement
{
    public class UserManagementScreenVM : BaseViewModel
    {
        #region Access property
        public bool IsAllowChangePassword =>
            AccountStore.instance.CurrentAccount.NHOMNGUOIDUNG.CHUCNANGs.Any(p => p.MaChucNang == AppEnum.ThayDoiMatKhau);
        public bool IsAllowSearchUser =>
            AccountStore.instance.CurrentAccount.NHOMNGUOIDUNG.CHUCNANGs.Any(p => p.MaChucNang == AppEnum.TraCuuNhanVien);
        public bool IsAllowAddUser =>
            AccountStore.instance.CurrentAccount.NHOMNGUOIDUNG.CHUCNANGs.Any(p => p.MaChucNang == AppEnum.ThemNguoiDung);
        #endregion

        #region DataRepository
        public GenericDataRepository<NHANVIEN> userRepo;
        #endregion

        #region Commands
        public ICommand OpenAddUserDialogCommand { get; set; }
        public ICommand RemoveUserCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand CloseSearchCommand { get; set; }
        public ICommand ResetPasswordCommand { get; set; }
        public ICommand SavePasswordCommand { get; set; }
        #endregion

        #region Properties
        private int selectedPage;
        public int SelectedPage
        {
            get => selectedPage;
            set
            {
                selectedPage = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<NHANVIEN> _filteredUsers;
        public ObservableCollection<NHANVIEN> FilteredUsers
        {
            get { return _filteredUsers; }
            set { _filteredUsers = value; OnPropertyChanged(); }
        }

        private ObservableCollection<NHANVIEN> usersToSearch;

        private ObservableCollection<NHANVIEN> bannedUsers;
        private ObservableCollection<NHANVIEN> notBannedUsers;


        private string _searchText;

        public string SearchText
        {
            get { return _searchText; }
            set { _searchText = value; OnPropertyChanged(); }
        }

        private string _searchBy;

        public string SearchBy
        {
            get { return _searchBy; }
            set { _searchBy = value; OnPropertyChanged(); }
        }

        private string _lastSearchText = "";
        private string _lastSearchOption;
        public List<string> SearchByOptions { get; set; }

        public NHANVIEN NewUser { get; set; }

        private bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (value)
                {
                    StatusText = "Đang hoạt động";
                    FilteredUsers = usersToSearch = notBannedUsers;
                    RemoveOrUnBanned = "Chặn";
                }
                else
                {
                    StatusText = "Không hoạt động";
                    FilteredUsers = usersToSearch = bannedUsers;
                    RemoveOrUnBanned = "Bỏ chặn";
                }

                _isChecked = value;
            }
        }

        private string _statusText;
        public string StatusText
        {
            get => _statusText;
            set { _statusText = value; OnPropertyChanged(); }
        }

        private string _removeOrUnBanned;
        public string RemoveOrUnBanned
        {
            get => _removeOrUnBanned;
            set { _removeOrUnBanned = value; OnPropertyChanged(); }
        }

        public bool IsShop { get; set; }
        private bool _isShop
        {
            get => _isShop;
            set { _isShop = value; OnPropertyChanged(); }
        }
        public string CurrentPassword { get; set; }
        private string newPassword;
        public string NewPassword
        {
            get => newPassword;
            set
            {
                newPassword = value;
                ConfirmNewPassword = null;
                ConfirmNewPassword = "";
            }
        }
        public string ConfirmNewPassword { get; set; }
        #endregion

        #region Constructor
        public UserManagementScreenVM()
        {
            Task.Run(async () =>
            {
                MainViewModel.SetLoading(true);
                userRepo = new GenericDataRepository<NHANVIEN>();
                SearchByOptions = new List<string> { "ID", "Tên", "SĐT", "Địa chỉ", "Username" };
                if (!IsAllowSearchUser && !IsAllowAddUser)
                {
                    SelectedPage = 1;
                }
                else
                {
                    SelectedPage = 0;
                }
                SearchBy = "ID";
                IsChecked = true;
                if (IsAllowSearchUser)
                {
                    await Load();
                }
                else
                {
                    notBannedUsers = new ObservableCollection<NHANVIEN>();
                    bannedUsers = new ObservableCollection<NHANVIEN>();
                    FilteredUsers = usersToSearch = notBannedUsers;
                    App.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        FilteredUsers = new ObservableCollection<NHANVIEN>(FilteredUsers);
                    }));
                }
                RemoveUserCommand = new RelayCommand<object>(p => p != null, async (p) => await RemoveUser(p));
                SearchCommand = new RelayCommandWithNoParameter(async () => await Search());
                CloseSearchCommand = new RelayCommandWithNoParameter(CloseSearch);
                OpenAddUserDialogCommand = new RelayCommandWithNoParameter(async () => await OpenAddUserDialog());
                SavePasswordCommand = new RelayCommand<object>(p =>
                {
                    return !string.IsNullOrEmpty(CurrentPassword) &&
                    !string.IsNullOrEmpty(NewPassword) &&
                    !string.IsNullOrEmpty(ConfirmNewPassword) &&
                    ConfirmNewPassword == NewPassword;
                }, async p =>
                {
                    var user = AccountStore.instance.CurrentAccount;
                    var dl = new ConfirmDialog()
                    {
                        ContentString = "Mật khẩu bạn nhập đã sai. Xin hãy nhập lại!",
                        Header = "Oops",
                    };
                    if (CurrentPassword != user.Password)
                    {
                        await DialogHost.Show(dl, "Main");
                        return;
                    }
                    dl.ContentString = "Mật khẩu đã được thay đổi!";
                    dl.Header = "Thông báo";

                    user.Password = NewPassword;
                    await userRepo.Update(user);
                    await DialogHost.Show(dl, "Main");
                    CurrentPassword = "";
                    NewPassword = "";
                    ConfirmNewPassword = "";
                });

                ResetPasswordCommand = new RelayCommand<NHANVIEN>(p => { return true; }, async p =>
                {
                    ConfirmAccountDialog confirmAccountDialog = new ConfirmAccountDialog();
                    ConfirmAccountDialogVM confirmAccountDialogVM = new ConfirmAccountDialogVM(AccountStore.instance.CurrentAccount, p);
                    confirmAccountDialog.DataContext = confirmAccountDialogVM;
                    await DialogHost.Show(confirmAccountDialog, "Main");
                });
                MainViewModel.SetLoading(false);
            });
        }
        #endregion 

        #region Command Methods
        private async Task Load()
        {
            NewUser = new NHANVIEN();
            IsChecked = true;
            RemoveOrUnBanned = "Chặn";
            SearchBy = SearchByOptions[1];

            notBannedUsers = new ObservableCollection<NHANVIEN>(await userRepo.GetListAsync(
                user => user.isActive == true &&
                user.MaNhanVien != AccountStore.instance.CurrentAccount.MaNhanVien)
            );
            bannedUsers = new ObservableCollection<NHANVIEN>(await userRepo.GetListAsync(
                user => user.isActive == false &&
                user.MaNhanVien != AccountStore.instance.CurrentAccount.MaNhanVien)
            );
            FilteredUsers = usersToSearch = notBannedUsers;

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                FilteredUsers = new ObservableCollection<NHANVIEN>(FilteredUsers);
            }));
        }

        public async Task RemoveUser(object obj)
        {
            MainViewModel.SetLoading(true);

            var removeUser = obj as NHANVIEN;
            if (removeUser == null)
                return;

            if(removeUser.isActive == true)
            {
                removeUser.isActive = false;
                notBannedUsers.Remove(removeUser);
                bannedUsers.Insert(0, removeUser);
                FilteredUsers = usersToSearch = notBannedUsers;
            }    
            else
            {
                removeUser.isActive = true;
                notBannedUsers.Insert(0, removeUser);
                bannedUsers.Remove(removeUser);
                FilteredUsers = usersToSearch = bannedUsers;
            }

            await userRepo.Update(removeUser);
            MainViewModel.SetLoading(false);
        }

        public async Task Search()
        {
            MainViewModel.SetLoading(true);
            await Task.Run(() =>
            {
                if (string.IsNullOrEmpty(SearchBy))
                    FilteredUsers = usersToSearch;

                if (string.IsNullOrEmpty(_lastSearchText) && string.IsNullOrEmpty(SearchText) ||
                    (string.Equals(_lastSearchText, SearchText) && _lastSearchOption == SearchBy))
                {
                    FilteredUsers = usersToSearch;
                }

                if (string.IsNullOrEmpty(SearchText) || usersToSearch.Count <= 0 || usersToSearch == null)
                {
                    FilteredUsers = usersToSearch;
                    return;
                }

                if(SearchBy == "ID")
                {
                    _lastSearchOption = "ID";
                    FilteredUsers = new ObservableCollection<NHANVIEN>(usersToSearch.Where(br => br.MaNhanVien.ToLower().Contains(SearchText.ToLower())));
                }    
                else if (SearchBy == "Tên")
                {
                    _lastSearchOption = "Tên";
                    FilteredUsers = new ObservableCollection<NHANVIEN>(usersToSearch.Where(br => Helpers.convertToUnSign3(br.TenNhanVien).ToLower().Contains(Helpers.convertToUnSign3(SearchText).ToLower())));
                }
                else if (SearchBy == "SĐT")
                {
                    _lastSearchOption = "SĐT";
                    FilteredUsers = new ObservableCollection<NHANVIEN>(usersToSearch.Where(br => br.DienThoai.ToLower().Contains(SearchText.ToLower())));
                }
                else if (SearchBy == "Địa chỉ")
                {
                    _lastSearchOption = "Địa chỉ";
                    FilteredUsers = new ObservableCollection<NHANVIEN>(usersToSearch.Where(br => Helpers.convertToUnSign3(br.DiaChi).ToLower().Contains(Helpers.convertToUnSign3(SearchText).ToLower())));
                }
                else if (SearchBy == "Username")
                {
                    _lastSearchOption = "Username";
                    FilteredUsers = new ObservableCollection<NHANVIEN>(usersToSearch.Where(br => br.UserName.ToLower().Contains(SearchText.ToLower())));
                }
            });
            MainViewModel.SetLoading(false);
        }

        public async Task OpenAddUserDialog()
        {
            MainViewModel.SetLoading(true);
            AdminAddUserDialog addUserDialog = new AdminAddUserDialog();
            AdminAddUserDialogVM addUserDialogViewModel = new AdminAddUserDialogVM();
            if(IsAllowSearchUser)
            {
                addUserDialogViewModel.ClosedDialog += AddUserDialogViewModel_ClosedDialog;
            }   
            addUserDialog.DataContext = addUserDialogViewModel;
            MainViewModel.SetLoading(false);
            await DialogHost.Show(addUserDialog, "Main");
        }
        private void AddUserDialogViewModel_ClosedDialog()
        {
            _ = Load();
        }

        public void CloseSearch()
        {
            SearchText = string.Empty;
        }
        #endregion
    }
}
