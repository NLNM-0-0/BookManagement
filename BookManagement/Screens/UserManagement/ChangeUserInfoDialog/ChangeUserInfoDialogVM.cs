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

namespace BookManagement
{
    public class ChangeUserInfoDialogVM : BaseViewModel
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
        public ICommand SaveUserCommand { get; set; }
        public ICommand ResetEditCommand { get; set; }
        #endregion

        #region Properties
        private NHANVIEN initUser;
        private NHANVIEN user;
        public NHANVIEN User
        {
            get => user;
            set
            {
                user = value;
                OnPropertyChanged();
            }
        }
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
        public ChangeUserInfoDialogVM(NHANVIEN editedUser)
        {
            Task.Run(async () => {
                MainViewModel.SetLoading(true);
                initUser = editedUser;
                await Load(editedUser.MaNhanVien);
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    UserGroups = new ObservableCollection<NHOMNGUOIDUNG>(UserGroups);
                    SelectedUserGroup = UserGroups.Single(p=>p.MaNhomNguoiDung == User.MaNhomNguoiDung);
                }));

                ResetEditCommand = new RelayCommandWithNoParameter(() =>
                {
                    User.MaNhomNguoiDung = initUser.MaNhomNguoiDung;
                    User.GioiTinh = initUser.GioiTinh;
                    User.NgaySinh = initUser.NgaySinh;
                    User.DienThoai = initUser.DienThoai;
                    User.TenNhanVien = initUser.TenNhanVien;
                    User.DiaChi = initUser.DiaChi;
                    SelectedUserGroup = UserGroups.Single(p => p.MaNhomNguoiDung == User.MaNhomNguoiDung);
                    OnPropertyChanged(nameof(User));
                });

                SaveUserCommand = new RelayCommand<object>((_) => {
                    return !String.IsNullOrEmpty(User.DienThoai) &&
                    !String.IsNullOrEmpty(User.TenNhanVien) &&
                    !String.IsNullOrEmpty(User.UserName) &&
                    !String.IsNullOrEmpty(User.DiaChi) &&
                    User.NgaySinh != null;
                }
                , async (_) =>
                {
                    MainViewModel.SetLoading(true);
                    User.MaNhomNguoiDung = SelectedUserGroup.MaNhomNguoiDung;
                    await userRepo.Update(User);
                    MainViewModel.SetLoading(false);
                    DialogHost.CloseDialogCommand.Execute(null, null);
                    OnClosedDialog();
                });
                MainViewModel.SetLoading(false);
            });

        }
        #endregion

        #region Command Define
        public async Task Load(String idUser)
        {
            UserGroups = new ObservableCollection<NHOMNGUOIDUNG>(await userGroupRepo.GetListAsync(p=>p.MaNhomNguoiDung!=AppEnum.Admin));
            User = await userRepo.GetSingleAsync(p => p.MaNhanVien == idUser);
        }
        #endregion
    }
}
