using BookManagement.Models;
using DocumentFormat.OpenXml.Drawing.Charts;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BookManagement
{
    public class BillManagementScreenVM : BaseViewModel
    {
        #region GenericDataRepository
        GenericDataRepository<HOADON> billRepo = new GenericDataRepository<HOADON>();
        #endregion

        #region Commands
        public ICommand SearchCommand { get; set; }
        public ICommand ResetCommand { get; set; }
        public ICommand ShowBillCommand { get; set; }
        public ICommand PrintBillCommand { get; set; }
        #endregion

        #region Properties
        public string SearchId { get; set; }
        public string SearchCustomerName { get; set; }
        public string SearchStaffName { get; set; }
        public DateTime? SearchDate { get; set; }
        public string SearchMinPrice { get; set; }
        public string SearchMaxPrice { get; set; }

        private ObservableCollection<HOADON> filterBills;
        public ObservableCollection<HOADON> FilterBills
        {
            get => filterBills;
            set
            {
                filterBills = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<HOADON> allBills;
        public ObservableCollection<HOADON> AllBills
        {
            get => allBills;
            set
            {
                allBills = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructor
        public BillManagementScreenVM()
        {
            Task.Run(async () =>
            {
                MainViewModel.IsLoading = true;
                await Load();
            }).ContinueWith(task =>
            {
                SearchCommand = new RelayCommandWithNoParameter(() =>
                {
                    MainViewModel.SetLoading(true);
                    Search();
                    MainViewModel.SetLoading(false);
                });
                ResetCommand = new RelayCommandWithNoParameter(() =>
                {
                    SearchId = "";
                    SearchDate = null;
                    SearchMinPrice = "";
                    SearchMaxPrice = "";
                    SearchCustomerName = "";
                    SearchStaffName = "";
                    FilterBills = new ObservableCollection<HOADON>(AllBills);
                });
                ShowBillCommand = new RelayCommand<HOADON>(p => p != null, p =>
                {
                    NavigateProvider.BillDetailScreen().Navigate(p.MaHoaDon);
                });
                PrintBillCommand = new RelayCommand<HOADON>(p => p != null, p =>
                {
                    BillInfoPdf pdf = new BillInfoPdf();
                    pdf.DataContext = new BillInforPdfViewModel(p);
                    try
                    {
                        pdf.Print();
                    }
                    catch  
                    {
                        var dl = new ConfirmDialog()
                        {
                            Header = "Oops",
                            ContentString = "Đã có lỗi xảy ra. Xin hãy kiểm tra lại bạn đã đóng file hay chưa."
                        };
                        DialogHost.Show(dl, "Main");
                    }
                });
                MainViewModel.IsLoading = false;
            });
        }
        #endregion

        public async Task Load()
        {
            AllBills = new ObservableCollection<HOADON>((
                await billRepo.GetAllAsync(
                    p => p.NHANVIEN, 
                    p => p.KHACHHANG, 
                    p => p.CHITIETHOADONs,
                    p => p.CHITIETHOADONs.Select(c => c.SACH.DAUSACH)
                )
            ).OrderByDescending(p=>p.NgayLapHoaDon));
            FilterBills = new ObservableCollection<HOADON>(AllBills);
        }

        public void Search()
        {
            ICollection<HOADON> temp = AllBills.Where(p => {
                bool checkId = String.IsNullOrEmpty(SearchId) || p.MaHoaDon.ToLower().Contains(SearchId.Trim().ToLower());
                if (!checkId)
                {
                    return false;
                }
                bool checkCustomerName = String.IsNullOrEmpty(SearchCustomerName)?true: (p.KHACHHANG !=null && Helpers.convertToUnSign3(p.KHACHHANG.TenKhachHang).ToLower().Contains(Helpers.convertToUnSign3(SearchCustomerName.Trim()).ToLower()));
                if (!checkCustomerName)
                {
                    return false;
                }
                bool checkStaffName = String.IsNullOrEmpty(SearchStaffName) || Helpers.convertToUnSign3(p.NHANVIEN.TenNhanVien.ToLower()).Contains(Helpers.convertToUnSign3(SearchStaffName.Trim()).ToLower());
                if (!checkStaffName)
                {
                    return false;
                }
                bool checkDate = SearchDate == null || (SearchDate != null && p.NgayLapHoaDon.Date == SearchDate.Value.Date);
                if (!checkDate)
                {
                    return false;
                }
                bool checkMinTotalPrice = String.IsNullOrEmpty(SearchMinPrice) || p.TongTien >= decimal.Parse(SearchMinPrice);
                if (!checkMinTotalPrice)
                {
                    return false;
                }
                bool checkMaxTotalPrice = String.IsNullOrEmpty(SearchMaxPrice) || p.TongTien <= decimal.Parse(SearchMaxPrice);
                if (!checkMaxTotalPrice)
                {
                    return false;
                }
                return true;
            }).ToList();
            FilterBills = new ObservableCollection<HOADON>(temp);
        }
    }
}
