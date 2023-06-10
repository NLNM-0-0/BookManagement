using BookManagement.Models;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BookManagement.Screens.SaleBook.AddCustomer
{
    public class AddCustomerVM : BaseViewModel
    {
        #region Action
        public event Action<KHACHHANG> ClosedDialog;
        #endregion

        #region Commands
        public ICommand AddCustomerCommand { get; set; }
        public ICommand CloseDialogCommand { get; set; }
        #endregion

        #region Properties
        public KHACHHANG NewCustomer { get; set; } = new KHACHHANG(); 
        private System.Windows.Controls.UserControl PreviousItem;
        #endregion

        #region Constructor
        public AddCustomerVM(List<KHACHHANG> customers)
        {
            Task.Run(() => {
                MainViewModel.SetLoading(true);
                NewCustomer.GioiTinh = "Nữ";
                NewCustomer.DienThoai = "";
                NewCustomer.TenKhachHang = "";
                NewCustomer.Email = "";

                AddCustomerCommand = new RelayCommand<object>((_) => {
                    return !String.IsNullOrEmpty(NewCustomer.DienThoai) &&
                    !String.IsNullOrEmpty(NewCustomer.TenKhachHang) &&
                    !String.IsNullOrEmpty(NewCustomer.Email) &&
                    !String.IsNullOrEmpty(NewCustomer.DiaChi) &&
                    NewCustomer.NgaySinh != null;
                }
                , async (_) =>
                {
                    PreviousItem = MainViewModel.UpdateDialog("Main");

                    MainViewModel.SetLoading(true);

                    KHACHHANG check = null;
                    try
                    {
                        check = customers.Single(d => d.DienThoai == NewCustomer.DienThoai);
                    }
                    catch
                    {
                        //Không có khách hàng trùng số điện thoại hoặc là có 2 khách hàng có sđt trùng
                    } 
                    
                    if (check != null)
                    {
                        var dl = new ConfirmDialog()
                        {
                            Header = "Oops",
                            ContentString = "Đã tồn tại khách hàng có số điện thoại này trong hệ thống.",
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
                    NewCustomer.TenKhachHang = NewCustomer.TenKhachHang.Trim();
                    NewCustomer.Email = NewCustomer.Email.Trim();
                    NewCustomer.DiaChi = NewCustomer.DiaChi.Trim();
                    NewCustomer.MaKhachHang = "";
                    NewCustomer.TienNo = 0;
                    MainViewModel.SetLoading(false);
                    CloseDialogCommand.Execute(null);
                    ClosedDialog?.Invoke(NewCustomer);
                });
                MainViewModel.SetLoading(false);
            });

        }
        #endregion
    }
}
