

using BookManagement.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace BookManagement
{
    internal class BillDetailViewModel: BaseViewModel
    {
        private GenericDataRepository<HOADON> billRepo;
        private GenericDataRepository<CHITIETHOADON> ctRepo;
        private string billId;
        public HOADON HoaDon { get; set; }
        public ObservableCollection<CHITIETHOADON> ListBillDetail { get; set; }
        public KHACHHANG KhachHang { get; set; }
        public BillDetailViewModel(string billId = "hd001")
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
            List<CHITIETHOADON> list = new List<CHITIETHOADON>(
                await ctRepo.GetListAsync(c => c.MaHoaDon == billId, c => c.SACH, c => c.SACH.DAUSACH));

            ListBillDetail = new ObservableCollection<CHITIETHOADON>(list);

            KhachHang = HoaDon.KHACHHANG;

        }
    }
}
