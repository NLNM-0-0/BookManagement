

using BookManagement.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BookManagement
{
    internal class BillDetailViewModel: BaseViewModel
    {
        private GenericDataRepository<HOADON> billRepo;
        private GenericDataRepository<CHITIETHOADON> ctRepo;
        private string billId;
        public HOADON HoaDon { get; set; }
        public ObservableCollection<CHITIETHOADON> ListBillDetail { get; set; }
        private KHACHHANG khachHang;
        public KHACHHANG KhachHang
        {
            get => khachHang;
            set { 
                khachHang = value; 
                OnPropertyChanged(); 
            }
        }
        private decimal paid;
        public decimal Paid
        {
            get => paid;
            set
            {
                paid = value;
                OnPropertyChanged();
            }
        }
        public BillDetailViewModel(string billId)
        {
            billRepo = new GenericDataRepository<HOADON>();
            ctRepo = new GenericDataRepository<CHITIETHOADON>();
            this.billId = billId;
            Task.Run(async () =>
            {
                MainViewModel.SetLoading(true);
                await Load();
            }).ContinueWith((first) =>
            {
                MainViewModel.SetLoading(false);

            });
        }

        private async Task Load()
        {
            HoaDon = await billRepo.GetSingleAsync(b => b.MaHoaDon.Equals(billId), b => b.KHACHHANG);
            List<CHITIETHOADON> list = new List<CHITIETHOADON>((
                await ctRepo.GetListAsync(
                    c => c.MaHoaDon == billId, 
                    c => c.SACH, 
                    c => c.SACH.DAUSACH)).OrderBy(p=>p.SACH.DAUSACH.TenSach).ThenBy(p=>p.SACH.NhaXuatBan));

            ListBillDetail = new ObservableCollection<CHITIETHOADON>(list);

            KhachHang = HoaDon.KHACHHANG;

            Paid = HoaDon.TongTien - HoaDon.SoTienNo;
        }
    }
}
