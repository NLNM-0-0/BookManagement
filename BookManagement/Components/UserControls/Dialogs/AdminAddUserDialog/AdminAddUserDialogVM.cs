using BookManagement.Models;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;

namespace BookManagement
{
    public class AdminAddUserDialogVM : BaseViewModel
    {
        #region DataRepository
        private readonly GenericDataRepository<NHANVIEN> userRepo = new GenericDataRepository<NHANVIEN>();
        private readonly GenericDataRepository<NHOMNGUOIDUNG> userGroupRepo = new GenericDataRepository<NHOMNGUOIDUNG>();
        #endregion

        #region Action
        public event Action ClosedDialog;
        private void OnClosedDialog()
        {
            ClosedDialog?.Invoke();
        }
        #endregion

        #region Commands
        public ICommand AddUserCommand { get; set; }
        #endregion

        #region Properties
        public NHANVIEN NewUser { get; set; } = new NHANVIEN();
        private ObservableCollection<NHOMNGUOIDUNG> userGroups;
        public ObservableCollection<NHOMNGUOIDUNG> UserGroups
        {
            get => userGroups;
            set
            {
                userGroups = value;
                OnPropertyChanged();
            }
        }
        private NHOMNGUOIDUNG selectedUserGroup;
        public NHOMNGUOIDUNG SelectedUserGroup
        {
            get => selectedUserGroup;
            set
            {
                selectedUserGroup = value;
                OnPropertyChanged();
            }
        }    
        private System.Windows.Controls.UserControl PreviousItem;
        #endregion

        #region Constructor
        public AdminAddUserDialogVM() {
            Task.Run(async() => {
                MainViewModel.SetLoading(true);
                NewUser.GioiTinh = "Nữ";
                NewUser.DienThoai = "";
                NewUser.TenNhanVien = "";
                NewUser.UserName = "";
                await Load();
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    UserGroups = new ObservableCollection<NHOMNGUOIDUNG>(UserGroups);
                    SelectedUserGroup = UserGroups[0]??null;
                }));
                
                AddUserCommand = new RelayCommand<object>((_) => {
                    return !String.IsNullOrEmpty(NewUser.DienThoai) &&
                    !String.IsNullOrEmpty(NewUser.TenNhanVien) &&
                    !String.IsNullOrEmpty(NewUser.UserName) &&
                    !String.IsNullOrEmpty(NewUser.DiaChi) &&
                    NewUser.NgaySinh!=null;
                }
                , async (_) =>
                {
                    PreviousItem = MainViewModel.UpdateDialog("Main");

                    MainViewModel.SetLoading(true);

                    var check = await userRepo.GetSingleAsync(d => d.UserName == NewUser.UserName);
                    if (check != null)
                    {
                        var dl = new ConfirmDialog()
                        {
                            Header = "Oops",
                            ContentString = "Đã tồn tại tài khoản trong hệ thống.",
                            CM = new RelayCommandWithNoParameter(() =>
                            {
                                DialogHost.CloseDialogCommand.Execute(null, null);
                                if (PreviousItem != null)
                                {
                                    DialogHost.Show(PreviousItem, "Main");
                                }
                            })
                        };
                        MainViewModel.SetLoading(false);
                        await DialogHost.Show(dl, "Main");
                        return;
                    }

                    NewUser.TenNhanVien = NewUser.TenNhanVien.Trim();
                    NewUser.DiaChi = NewUser.DiaChi.Trim();
                    NewUser.MaNhanVien = await GenerateId.Gen(typeof(NHANVIEN), "MaNhanVien");
                    NewUser.MaNhomNguoiDung = SelectedUserGroup.MaNhomNguoiDung;
                    NewUser.isActive = true;
                    NewUser.NgayVaoLam = DateTime.Now;
                    byte[] temp = ASCIIEncoding.ASCII.GetBytes("BUUK123");
                    byte[] hasData = new MD5CryptoServiceProvider().ComputeHash(temp);

                    string hasPass = "";

                    foreach (byte item in hasData)
                    {
                        hasPass += item;
                    }
                    NewUser.Password = hasPass;
                    NewUser.DiaChi = NewUser.DiaChi ?? "";

                    await userRepo.Add(NewUser);

                    MainViewModel.SetLoading(false);
                    DialogHost.CloseDialogCommand.Execute(null, null);
                    OnClosedDialog();
                });
                MainViewModel.SetLoading(false);
            });
            
        }
        #endregion

        #region Command Define
        public async Task Load()
        {
            UserGroups = new ObservableCollection<NHOMNGUOIDUNG>(await userGroupRepo.GetAllAsync());
        }
        #endregion
    }
}
