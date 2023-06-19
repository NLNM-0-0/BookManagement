using BookManagement.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BookManagement
{
    public class SaleBookScreenVM : BaseViewModel
    {
        #region Const 
        private const String allCategoryId = "tatca";
        #endregion

        #region GenericDataRepository
        GenericDataRepository<THELOAI> categoryRepo = new GenericDataRepository<THELOAI>();
        GenericDataRepository<SACH> bookRepo = new GenericDataRepository<SACH>();
        #endregion

        #region Command
        public ICommand ChangeCategory { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand SelectBookCommand { get; set; }
        public ICommand CloseSearchCommand { get; set; }
        public ICommand IncreaseAmountCommand { get; set; }
        public ICommand DecreaseAmountCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand DeleteAllCommand { get; set; }
        public ICommand PayCommand { get; set; }
        public ICommand SeeDetalCommand { get; set; }
        #endregion

        #region Properties
        private ObservableCollection<THELOAI> categories;
        public ObservableCollection<THELOAI> Categories
        {
            get => categories;
            set => categories = value;  
        }
        private THELOAI selectedCategory;
        public THELOAI SelectedCategory
        {
            get => selectedCategory;
            set => selectedCategory = value;
        }
        private Dictionary<String, List<SACH>> allBooks = new Dictionary<string, List<SACH>>();
        private List<SACH> filterBooks;
        private ObservableCollection<SACH> searchedBooks;
        public ObservableCollection<SACH> SearchedBooks
        {
            get => searchedBooks;
            set
            {
                searchedBooks = value;
                OnPropertyChanged();
            }
        }
        public String SearchText { get; set; }
        private ObservableCollection<String> searchByOptions;
        public ObservableCollection<String> SearchByOptions
        {
            get => searchByOptions;
            set
            {
                searchByOptions = value;
                OnPropertyChanged();
            }
        }
        private String searchby;
        public String SearchBy
        {
            get { return searchby; }
            set
            {
                searchby = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<BillDetailCanPropertyChange> billDetails;
        public ObservableCollection<BillDetailCanPropertyChange> BillDetails
        {
            get => billDetails;
            set => billDetails = value;
        }
        private decimal total;
        public decimal Total
        {
            get => total;
            set => total = value;
        }
        private decimal percentProfit;
        public decimal PercentProfit
        {
            get => percentProfit;
            set {
                percentProfit = value;
                OnPropertyChanged();
            }
        }
        private int minAmountBookCanSell;
        private System.Windows.Controls.UserControl PreviousItem;
        #endregion

        #region Constructor
        public SaleBookScreenVM()
        {
            Task.Run(async () =>
            {
                await Load();
                SelectedCategory = Categories[0]??null;
                if(allBooks.ContainsKey(SelectedCategory.MaTheLoai))
                {
                    filterBooks = allBooks[SelectedCategory.MaTheLoai];
                    App.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        SearchedBooks = new ObservableCollection<SACH>(filterBooks);
                    }));
                }    
            }).ContinueWith((_)=>
            {
                PercentProfit = Convert.ToDecimal(RuleStore.instance.getValue(AppEnum.TiLeTinhDonGiaXuat) / 100.0);
                SearchByOptions = new ObservableCollection<string> { "Mã sách", "Tên sách", "NSX" };
                SearchBy = SearchByOptions[0];
                BillDetails = new ObservableCollection<BillDetailCanPropertyChange>();
                ChangeCategory = new RelayCommand<THELOAI>(p => p != null, p =>
                {
                    filterBooks = allBooks[p.MaTheLoai];
                    SearchedBooks = new ObservableCollection<SACH>(filterBooks);
                    SearchText = "";
                });
                SelectBookCommand = new RelayCommand<SACH>(p => p != null, p =>
                {
                    BillDetailCanPropertyChange billDetail = null;
                    try
                    {
                        billDetail = BillDetails.Single(b => b.BillDetail.MaSach == p.MaSach);
                    }
                    catch
                    {
                        //Không tồn tại sách trong bill detail
                    }

                    if (billDetail == null)
                    {
                        billDetail = new BillDetailCanPropertyChange();
                        billDetail.BillDetail.MaSach = p.MaSach;
                        billDetail.BillDetail.SACH = p;
                        billDetail.SoLuong = 1;
                        billDetail.BillDetail.DonGia = p.DonGiaNhapMoiNhat * PercentProfit;
                        BillDetails.Add(billDetail);
                        Total += p.DonGiaNhapMoiNhat * PercentProfit;
                    }
                    else
                    {
                        if(billDetail.BillDetail.SACH.SoLuong - billDetail.SoLuong <= minAmountBookCanSell)
                        {
                            PreviousItem = MainViewModel.UpdateDialog("Main");
                            var dl = new ConfirmDialog() {
                                Header = "Lỗi",
                                ContentString = $"Số lượng sách tồn ít nhất trong kho là {minAmountBookCanSell}.",
                                CM = new RelayCommandWithNoParameter(() =>
                                {
                                    DialogHost.CloseDialogCommand.Execute(null, null);
                                    if (PreviousItem != null)
                                    {
                                        DialogHost.Show(PreviousItem, "Main");
                                    }
                                })
                            };
                            DialogHost.Show(dl, "Main");
                        }       
                        else
                        {
                            billDetail.SoLuong++;
                            Total += billDetail.BillDetail.DonGia;
                        }    
                    }

                });
                IncreaseAmountCommand = new RelayCommand<BillDetailCanPropertyChange>(billDetail => billDetail != null, billDetail =>
                {
                    if (billDetail.BillDetail.SACH.SoLuong - billDetail.SoLuong <= minAmountBookCanSell)
                    {
                        PreviousItem = MainViewModel.UpdateDialog("Main");
                        var dl = new ConfirmDialog()
                        {
                            Header = "Lỗi",
                            ContentString = $"Số lượng sách tồn ít nhất trong kho là {minAmountBookCanSell}.",
                            CM = new RelayCommandWithNoParameter(() =>
                            {
                                DialogHost.CloseDialogCommand.Execute(null, null);
                                if (PreviousItem != null)
                                {
                                    DialogHost.Show(PreviousItem, "Main");
                                }
                            })
                        };
                        DialogHost.Show(dl, "Main");
                    }
                    else
                    { 
                        billDetail.SoLuong++;
                        Total += billDetail.BillDetail.DonGia;
                    }    
                });
                DecreaseAmountCommand = new RelayCommand<BillDetailCanPropertyChange>(p => p != null, p =>
                {
                    if (p.SoLuong > 1)
                    {
                        p.SoLuong--;
                    }
                    else
                    {
                        BillDetails.Remove(p);
                    }
                    Total -= p.BillDetail.DonGia;
                });
                DeleteCommand = new RelayCommand<BillDetailCanPropertyChange>(p => p != null, p =>
                {
                    BillDetails.Remove(p);
                    Total -= p.BillDetail.DonGia * p.SoLuong;
                });
                DeleteAllCommand = new RelayCommandWithNoParameter(() =>
                {
                    BillDetails.Clear();
                    Total = 0;
                });
                PayCommand = new RelayCommand<object>(p => BillDetails.Count > 0, p =>
                {
                    List<CHITIETHOADON> billDetails = BillDetails.Select(b => b.BillDetail).ToList();
                    PayConfirmDialog payConfirmDialog = new PayConfirmDialog();
                    PayConfirmDialogVM payConfirmDialogVM = new PayConfirmDialogVM(billDetails);
                    payConfirmDialogVM.AddBillSuccess += AddBillSuccesProcess;
                    payConfirmDialog.DataContext = payConfirmDialogVM;
                    DialogHost.Show(payConfirmDialog, "Main");

                });
                SearchCommand = new RelayCommandWithNoParameter(() => Search());
                CloseSearchCommand = new RelayCommandWithNoParameter(CloseSearch);
                SeeDetalCommand = new RelayCommand<SACH>(p => p != null, p =>
                {
                    BookDetail bookDetail = new BookDetail();
                    BookDetailVM bookDetailVM = new BookDetailVM(p);
                    bookDetail.DataContext = bookDetailVM;
                    DialogHost.Show(bookDetail, "Main");
                });
            });
            

        }
        #endregion

        #region function
        private void AddBillSuccesProcess()
        {
            BillDetails.Clear();
            Total = 0;
        }
        private void Search() { 
            if(SearchBy == SearchByOptions[0]) //Mã sách
            {
                SearchedBooks = new ObservableCollection<SACH>(filterBooks.Where(p=>p.MaSach.ToLower().Contains(SearchText.ToLower().Trim())));
            }    
            else if(SearchBy == SearchByOptions[1]) //Tên sách
            {
                SearchedBooks = new ObservableCollection<SACH>(filterBooks.Where(p => Helpers.convertToUnSign3(p.DAUSACH.TenSach).ToLower().Contains(Helpers.convertToUnSign3(SearchText.Trim()).ToLower())));
            }
            else if (SearchBy == SearchByOptions[2]) //NXB
            {
                SearchedBooks = new ObservableCollection<SACH>(filterBooks.Where(p => Helpers.convertToUnSign3(p.NhaXuatBan).ToLower().Contains(Helpers.convertToUnSign3(SearchText).ToLower().Trim())));
            }
        }
        private void CloseSearch() {
            SearchText = string.Empty;
        }
        private async Task Load()
        {
            Categories = new ObservableCollection<THELOAI>(await categoryRepo.GetAllAsync());
            THELOAI allCategory = new THELOAI();
            allCategory.MaTheLoai = allCategoryId;
            allCategory.TenTheLoai = "Tất cả";
            Categories.Insert(0, allCategory);

            allBooks.Add(allCategoryId, new List<SACH>());

            minAmountBookCanSell = RuleStore.instance.getValue(AppEnum.LuongTonToiDaKhiBan);
            List<SACH> books = (await bookRepo.GetListAsync(p=>p.SoLuong > minAmountBookCanSell && p.IsActive == true, p=>p.DAUSACH, p=>p.DAUSACH.TACGIAs, p=>p.DAUSACH.THELOAI))
                .OrderBy(p=>p.DAUSACH.TenSach).ThenBy(p=>p.NhaXuatBan).ToList();
            foreach (SACH item in books)
            {
                allBooks[allCategoryId].Add(item);
                if (!allBooks.ContainsKey(item.DAUSACH.THELOAI.MaTheLoai))
                {
                    List<SACH> tempListBook = new List<SACH>();
                    tempListBook.Add(item);
                    allBooks.Add(item.DAUSACH.THELOAI.MaTheLoai, tempListBook);
                }
                else
                {
                    allBooks[item.DAUSACH.THELOAI.MaTheLoai].Add(item);
                }
            }
        }
        #endregion 
    }
}
