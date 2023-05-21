using BookManagement.Models;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        private System.Windows.Controls.UserControl PreviousItem;
        #endregion

        #region Constructor
        public AdminAddUserDialogVM() {
            NewUser.GioiTinh = "Nữ";
            NewUser.DienThoai = "";
            NewUser.TenNhanVien = "";
            NewUser.UserName = "";

            AddUserCommand = new RelayCommand<object>((_)=>{
                return NewUser.DienThoai.Length > 0 &&
                NewUser.TenNhanVien.Length > 0 &&
                NewUser.UserName.Length > 0;
            }
            , async(_) =>
            {
                PreviousItem = MainViewModel.UpdateDialog("Main");

                MainViewModel.SetLoading(true);

                var check = await userRepo.GetSingleAsync(d => d.UserName == NewUser.UserName);
                if (check != null)
                {
                    var dl = new ConfirmDialog()
                    {
                        Header = "Oops",
                        ContentString = "This username is already exists",
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

                NewUser.MaNhanVien = await GenerateId.Gen(typeof(NHANVIEN), "MaNhanVien");
                NewUser.MaNhomNguoiDung = AppEnum.Staff;
                NewUser.isActive = true;    
                NewUser.NgayVaoLam = DateTime.Now;
                NewUser.Password = "";
                NewUser.DiaChi = NewUser.DiaChi ?? "";

                await userRepo.Add(NewUser);

                MainViewModel.SetLoading(false);
                DialogHost.CloseDialogCommand.Execute(null, null);
                OnClosedDialog();
            });
        }
        #endregion
    }
}
