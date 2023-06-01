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
    public class DebtManagementScreenVM : BaseViewModel
    {
        #region GenericDataRepository
        GenericDataRepository<KHACHHANG> customerRepo = new GenericDataRepository<KHACHHANG>();
        #endregion

        #region Commands
        public ICommand SearchCommand { get; set; }
        public ICommand ResetCommand { get; set; }
        public ICommand ShowCustomerCommand { get; set; }
        public ICommand CollectMoneyCommand { get; set; }
        #endregion

        #region Properties
        public string SearchId { get; set; }
        public DateTime? SearchDate { get; set; }
        public string SearchMinDebt { get; set; }
        public string SearchMaxDebt { get; set; }
        public string SearchCustomerName { get; set; }
        public string SearchPhone { get; set; }
        public string SearchEmail { get; set; }

        private ObservableCollection<KHACHHANG> filterCustomers;
        public ObservableCollection<KHACHHANG> FilterCustomers
        {
            get => filterCustomers;
            set
            {
                filterCustomers = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<KHACHHANG> allCustomers;
        public ObservableCollection<KHACHHANG> AllCustomers
        {
            get => allCustomers;
            set
            {
                allCustomers = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructor
        public DebtManagementScreenVM()
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
                    SearchMinDebt = "";
                    SearchMaxDebt = "";
                    SearchCustomerName = "";
                    SearchEmail = "";
                    SearchPhone = "";
                    FilterCustomers = new ObservableCollection<KHACHHANG>(AllCustomers);
                });
                ShowCustomerCommand = new RelayCommand<KHACHHANG>(p => p!=null, p =>
                {
                    NavigateProvider.DeptDetailScreen().Navigate(p.MaKhachHang);
                });
                CollectMoneyCommand = new RelayCommand<KHACHHANG>(p => p != null, async p =>
                {
                    DebtCollect debtCollect = new DebtCollect();
                    DebtCollectVM debtCollectVM = new DebtCollectVM(p);
                    debtCollectVM.AddDebtCollectSuccess += OnAddDebtCollectSuccess;
                    debtCollect.DataContext = debtCollectVM;
                    await DialogHost.Show(debtCollect, "Main");
                });
                MainViewModel.IsLoading = false;
            });
        }
        #endregion

        public async Task Load()
        {
            AllCustomers = new ObservableCollection<KHACHHANG>(await customerRepo.GetAllAsync());
            FilterCustomers = new ObservableCollection<KHACHHANG>(AllCustomers);
        }

        public void Search()
        {
            ICollection<KHACHHANG> temp = AllCustomers.Where(p => {
                bool checkId = SearchId == null || SearchId == "" || p.MaKhachHang.ToLower().Contains(SearchId.Trim().ToLower());
                if (!checkId)
                {
                    return false;
                }
                bool checkCustomerName = SearchCustomerName == null || SearchCustomerName == "" || p.TenKhachHang.ToLower().Contains(SearchCustomerName.Trim().ToLower());
                if (!checkCustomerName)
                {
                    return false;
                }
                bool checkDate = SearchDate == null || (SearchDate != null && p.NgaySinh == SearchDate);
                if (!checkDate)
                {
                    return false;
                }
                bool checkPhone = SearchPhone == null || SearchPhone == "" || p.DienThoai.Contains(SearchPhone.Trim());
                if (!checkPhone)
                {
                    return false;
                }
                bool checkEmail = SearchEmail == null || SearchEmail == "" || p.Email.ToLower().Contains(SearchEmail.Trim().ToLower());
                if (!checkEmail)
                {
                    return false;
                }
                bool checkMinTotalPrice = SearchMinDebt == null || SearchMinDebt == "" || p.TienNo >= decimal.Parse(SearchMinDebt);
                if (!checkMinTotalPrice)
                {
                    return false;
                }
                bool checkMaxTotalPrice = SearchMaxDebt == null || SearchMaxDebt == "" || p.TienNo <= decimal.Parse(SearchMaxDebt);
                if (!checkMaxTotalPrice)
                {
                    return false;
                }
                return true;
            }).ToList();
            FilterCustomers = new ObservableCollection<KHACHHANG>(temp);
        }

        public void OnAddDebtCollectSuccess()
        {
            FilterCustomers = new ObservableCollection<KHACHHANG>(FilterCustomers);
        }
    }
}
