

using BookManagement.Models;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BookManagement
{
    public class DebtDetailViewModel : BaseViewModel
    {
        #region Properties
        private GenericDataRepository<KHACHHANG> khRepo;
        private GenericDataRepository<PHIEUTHUNO> ptRepo;

        public ObservableCollection<PHIEUTHUNO> ListPhieuThu { get; set; }
        private List<PHIEUTHUNO> allPhieuThu;
        public KHACHHANG KhachHang { get; set; }
        private string maKH;

        private string searchMaPhieuThu;
        public string SearchMaPhieuThu
        {
            get => searchMaPhieuThu;
            set { searchMaPhieuThu = value; OnPropertyChanged(); }
        }
        private string searchMaNV;
        public string SearchMaNV
        {
            get => searchMaNV;
            set { searchMaNV = value; OnPropertyChanged(); }
        }
        private string minPriceSearch;
        public string MinPriceSearch
        {
            get => minPriceSearch;
            set
            {
                minPriceSearch = value;
                OnPropertyChanged();
            }
        }
        private string maxpriceSearch;
        public string MaxPriceSearch
        {
            get => maxpriceSearch;
            set
            {
                maxpriceSearch = value;
                OnPropertyChanged();
            }
        }
        public DateTime? SearchFromDate { get; set; }
        public DateTime? SearchToDate { get; set; }

        #endregion

        #region Commands
        public ICommand SearchCommand { get; set; }
        public ICommand ResetCommand { get; set; }
        public ICommand PrintPhieuThuCommand { get; set; }
        #endregion
        public DebtDetailViewModel(string maKH)
        {
            khRepo = new GenericDataRepository<KHACHHANG>();
            ptRepo = new GenericDataRepository<PHIEUTHUNO>();
            this.maKH = maKH;
            Task.Run(async () =>
            {
                MainViewModel.SetLoading(true);
                await Load();
            }).ContinueWith((first) =>
            {
                MainViewModel.SetLoading(false);

            });

            SearchCommand = new RelayCommandWithNoParameter(async () => await SearchAsync());
            ResetCommand = new RelayCommandWithNoParameter(async () => await ResetSearch());
            PrintPhieuThuCommand = new RelayCommand<object>((p) =>
            {
                return p != null;
            }, async (p) =>
            {
                var phieuThu = p as PHIEUTHUNO;
                if (phieuThu != null)
                {
                    DebtCollectPdf pdf = new DebtCollectPdf();
                    pdf.DataContext = new DebtCollectPdfVM(phieuThu);
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
                        await DialogHost.Show(dl, "Main");
                    }
                }
            });
        }

        private async Task ResetSearch()
        {
            SearchMaPhieuThu = string.Empty;
            SearchMaNV = string.Empty;
            MaxPriceSearch = string.Empty;
            MinPriceSearch = string.Empty;
            SearchToDate = null;
            SearchFromDate = null;
            await SearchAsync();
        }

        private async Task SearchAsync()
        {
            double maxPrice = double.MaxValue;
            double minPrice = 0;

            if (!string.IsNullOrEmpty(MaxPriceSearch))
            {
                double.TryParse(MaxPriceSearch, out maxPrice);
            }
            if (!string.IsNullOrEmpty(MinPriceSearch))
            {
                double.TryParse(MinPriceSearch, out minPrice);
            }
            if (minPrice > maxPrice)
            {
                ConfirmDialog notification = new ConfirmDialog()
                {
                    Header = "Lỗi",
                    ContentString = "Giá tối thiểu đang lớn hơn giá tối đa."
                };
                await DialogHost.Show(notification, "Main");

                return;
            }
            if (SearchFromDate > SearchToDate)
            {
                ConfirmDialog notification = new ConfirmDialog()
                {
                    Header = "Lỗi",
                    ContentString = "Giá trị ngày không hợp lệ"
                };
                await DialogHost.Show(notification, "Main");

                return;
            }
            List<PHIEUTHUNO> temp = allPhieuThu.Where(p =>
            {
                bool checkId = SearchMaNV == null || SearchMaNV == "" || p.MaNhanVien.ToLower().Contains(SearchMaNV.Trim().ToLower());
                if (!checkId)
                {
                    return false;
                }
                bool checkPt = SearchMaPhieuThu == null || SearchMaPhieuThu == "" || p.MaPhieuThu.ToLower().Contains(SearchMaPhieuThu.Trim().ToLower());
                if (!checkPt)
                {
                    return false;
                }
                bool checkDate = SearchFromDate == null || (SearchFromDate != null && p.NgayThu >= SearchFromDate.Value.Date);
                if (!checkDate)
                {
                    return false;
                }
                bool checkToDate = SearchToDate == null || (SearchToDate != null && p.NgayThu <= SearchToDate.Value.AddDays(1).Date);
                if (!checkToDate)
                {
                    return false;
                }
                bool checkMinTotalPrice = MinPriceSearch == null || MinPriceSearch == "" || p.SoTienThu >= (decimal)minPrice;
                if (!checkMinTotalPrice)
                {
                    return false;
                }
                bool checkMaxTotalPrice = MaxPriceSearch == null || MaxPriceSearch == "" || p.SoTienThu <= (decimal)maxPrice;
                if (!checkMaxTotalPrice)
                {
                    return false;
                }
                return true;
            }).ToList();
            ListPhieuThu = new ObservableCollection<PHIEUTHUNO>(temp);
        }

        private async Task Load()
        {
            KhachHang = await khRepo.GetSingleAsync(k => k.MaKhachHang == maKH, k => k.PHIEUTHUNOes);
            allPhieuThu = new List<PHIEUTHUNO>((await ptRepo.GetListAsync(
                p => p.MaKhachHang == maKH,
                p => p.NHANVIEN,
                p => p.KHACHHANG)).OrderByDescending(p => p.NgayThu));
            ListPhieuThu = new ObservableCollection<PHIEUTHUNO>(allPhieuThu);
        }
    }
}
