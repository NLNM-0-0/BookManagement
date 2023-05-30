

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
    public class DebtDetailViewModel:BaseViewModel
    {
        #region Properties
        private GenericDataRepository<KHACHHANG> khRepo;
        public ObservableCollection<PHIEUTHUNO> ListPhieuThu { get; set; }
        private List<PHIEUTHUNO> allPhieuThu;
        public KHACHHANG KhachHang { get; set; }
        private string maKH;

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

        #region
        public ICommand SearchCommand { get; set; }
        public ICommand ResetCommand { get; set; }
        #endregion
        public DebtDetailViewModel(string maKH = "kh001")
        {
            khRepo = new GenericDataRepository<KHACHHANG>();
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
        }

        private async Task ResetSearch()
        {
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
                MainViewModel.SetLoading(true);
                ConfirmDialog notification = new ConfirmDialog()
                {
                    Header = "Error",
                    ContentString = "Min price is bigger than max price"
                };
                MainViewModel.SetLoading(false);
                await DialogHost.Show(notification, "Main");

                return;
            }
            List<PHIEUTHUNO> temp = allPhieuThu.Where(p => {
                bool checkId = SearchMaNV == null || SearchMaNV == "" || p.MaNhanVien.ToLower().Contains(SearchMaNV.Trim().ToLower());
                if (!checkId)
                {
                    return false;
                }
                bool checkDate = SearchFromDate == null || (SearchFromDate != null && p.NgayThu >= SearchFromDate);
                if (!checkDate)
                {
                    return false;
                }
                bool checkToDate = SearchToDate == null || (SearchToDate != null && p.NgayThu <= SearchToDate);
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
            allPhieuThu = new List<PHIEUTHUNO>(KhachHang.PHIEUTHUNOes);
            ListPhieuThu = new ObservableCollection<PHIEUTHUNO>(allPhieuThu);
        }
    }
}
