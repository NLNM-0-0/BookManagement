using BookManagement.Models;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BookManagement
{
    internal class EditCustomerVM : BaseViewModel
    {
        private KHACHHANG oldKhachHang;
        private GenericDataRepository<KHACHHANG> khRepo;
        private List<KHACHHANG> allKH;
        public string TenKhachHang { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string DiaChi { get; set; }
        private string dienThoai;
        public string DienThoai
        {
            get => dienThoai;
            set
            {
                dienThoai = value;
                if (PhoneWarnVisibility)
                {
                    PhoneWarnVisibility = false;
                }
            }
        }
        public string Email { get; set; }
        public string GioiTinh { get; set; }
        public bool PhoneWarnVisibility { get; set; }
        public ICommand SaveCustomerCommand { get; set; }
        public ICommand ResetCommand { get; set; }
        public EditCustomerVM(KHACHHANG kHACHHANG)
        {
            khRepo = new GenericDataRepository<KHACHHANG>();
            oldKhachHang = kHACHHANG;
            TenKhachHang = oldKhachHang.TenKhachHang;
            NgaySinh = oldKhachHang.NgaySinh;
            DiaChi = oldKhachHang.DiaChi;
            DienThoai = oldKhachHang.DienThoai;
            Email = oldKhachHang.Email;
            GioiTinh = oldKhachHang.GioiTinh;
            PhoneWarnVisibility = false;
            SaveCustomerCommand = new RelayCommandWithNoParameter(SaveNewCustomer);
            ResetCommand = new RelayCommandWithNoParameter(() =>
            {
                TenKhachHang = oldKhachHang.TenKhachHang;
                TenKhachHang = oldKhachHang.TenKhachHang;
                NgaySinh = oldKhachHang.NgaySinh;
                DiaChi = oldKhachHang.DiaChi;
                DienThoai = oldKhachHang.DienThoai;
                Email = oldKhachHang.Email;
                GioiTinh = oldKhachHang.GioiTinh;
            });
        }
        private async void SaveNewCustomer()
        {
            var list = new List<KHACHHANG>();
            try
            {
                if (allKH == null)
                {
                    allKH = new List<KHACHHANG>( await khRepo.GetAllAsync());
                }
                list = allKH.Where(kh => kh.DienThoai == DienThoai.Trim() && kh.DienThoai != oldKhachHang.DienThoai).ToList();
            }
            catch
            {
                //Không có khách hàng trùng số điện thoại hoặc là có 2 khách hàng có sđt trùng
            }

            if (list.Count>0)
            {
                PhoneWarnVisibility = true;
                return;
            }
            PhoneWarnVisibility = false;
            oldKhachHang.TenKhachHang = TenKhachHang.Trim();
            oldKhachHang.Email = Email.Trim();
            oldKhachHang.DiaChi = DiaChi.Trim();

            oldKhachHang.NgaySinh = NgaySinh;
            oldKhachHang.DienThoai = DienThoai.Trim();
            oldKhachHang.GioiTinh = GioiTinh;
            DialogHost.CloseDialogCommand.Execute(null, null);
            MainViewModel.IsLoading = true;
            await khRepo.Update(oldKhachHang);
            MainViewModel.IsLoading = false;
        }
    }
}
