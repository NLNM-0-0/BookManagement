using BookManagement.Models;
using System.Globalization;

namespace BookManagement
{
    public class BillInforPdfViewModel : BaseViewModel
    {
        private HOADON bill;
        public HOADON Bill
        {
            get => bill;
            set
            {
                bill = value;
                OnPropertyChanged();
            }
        }
        public string CustomerName
        {
            get
            {
                return Bill.KHACHHANG == null ? "" : Bill.KHACHHANG.TenKhachHang;
            }
        }
        public string CustomerPhone
        {
            get
            {
                return Bill.KHACHHANG == null ? "" : Bill.KHACHHANG.DienThoai;
            }
        }
        public string CustomerAddress
        {
            get
            {
                return Bill.KHACHHANG == null ? "" : Bill.KHACHHANG.DiaChi;
            }
        }
        public string Paid
        {
            get
            {
                decimal money = (decimal)(Bill.TongTien - Bill.SoTienNo);
                if (money == 0)
                {
                    return "0 đ";
                }
                CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
                return $"{money.ToString("#,###", cul.NumberFormat)} đ";
            }
        }
        public BillInforPdfViewModel(HOADON bill) 
        {
            Bill = bill;
        }
    }
}
