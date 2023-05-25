using BookManagement.Models;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.InkML;
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
    public class AddBookVM : BaseViewModel
    {
        #region
        public Action<SACH> AddBookSuccess;
        #endregion
        #region GenericDataRepository
        private GenericDataRepository<DAUSACH> bookHeaderRepo = new GenericDataRepository<DAUSACH>();
        private GenericDataRepository<THELOAI> categoryRepo = new GenericDataRepository<THELOAI>();
        private GenericDataRepository<TACGIA> authorRepo = new GenericDataRepository<TACGIA>();
        private GenericDataRepository<SACH> bookRepo = new GenericDataRepository<SACH>();
        #endregion

        #region Commands
        public ICommand SeeDetailAuthorsCommand { get; set; }
        public ICommand AddBookCommand { get; set; }
        private System.Windows.Controls.UserControl PreviousItem;
        #endregion

        #region Properties
        public ICollection<DAUSACH> allBookHeaers;
        private ObservableCollection<DAUSACH> filterBookHeaders;
        public ObservableCollection<DAUSACH> FilterBookHeaders
        {
            get => filterBookHeaders;
            set
            {
                filterBookHeaders = value;
                OnPropertyChanged();
            }
        }
        private DAUSACH selectedBookHeader;
        public DAUSACH SelectedBookHeader
        {
            get => selectedBookHeader;
            set
            {
                selectedBookHeader = value;
                OnPropertyChanged();
            }
        }
        private DAUSACH finalBookHeader;
        public DAUSACH FinalBookHeader
        {
            get => finalBookHeader;
            set
            {
                finalBookHeader = value;
                if(finalBookHeader != null)
                {
                    if (finalBookHeader.MaDauSach == null)
                    {
                        FilterNXBs = new ObservableCollection<string>(allNXBs);
                        Authors = new ObservableCollection<TACGIA>();
                    }
                    else
                    {
                        FilterNXBs = new ObservableCollection<string>((finalBookHeader.SACHes).Select(p => p.NhaXuatBan));

                        Authors = new ObservableCollection<TACGIA>(finalBookHeader.TACGIAs);

                        SelectedCategory = finalBookHeader.THELOAI;
                        SelectedCategoryString = finalBookHeader.THELOAI.TenTheLoai;
                    }
                }    
                else
                {
                    Authors = new ObservableCollection<TACGIA>();
                }    
                OnPropertyChanged();
            }
        }
        private string selectedBookHeaderString;
        public string SelectedBookHeaderString
        {
            get => selectedBookHeaderString;
            set
            {
                selectedBookHeaderString = value;
                selectedNXBString = "";
                if (selectedBookHeaderString == null || selectedBookHeaderString.Length == 0)
                {
                    FinalBookHeader = null;
                    AuthorActionText = "";
                }
                else if(allBookHeaers.Any(p => p.TenSach == selectedBookHeaderString))
                {
                    FinalBookHeader = SelectedBookHeader;
                    AuthorActionText = "Xem chi tiết";
                }
                else
                {
                    FinalBookHeader = new DAUSACH();
                    AuthorActionText = "Chỉnh";
                }
                OnPropertyChanged();
            }
        }
        private List<string> allNXBs;
        private ObservableCollection<string> filternxbs;
        public ObservableCollection<string> FilterNXBs
        {
            get => filternxbs;
            set
            {
                filternxbs = value;
                OnPropertyChanged();
            }
        }
        private string selectedNXB;
        public string SelectedNXB
        {
            get => selectedNXB;
            set
            {
                selectedNXB = value;
                OnPropertyChanged();
            }
        }
        private string selectedNXBString;
        public string SelectedNXBString
        {
            get => selectedNXBString;
            set
            {
                selectedNXBString = value;
                if(selectedNXBString == null ||
                    selectedNXBString.Length == 0 || 
                    selectedNXB == null ||
                    selectedBookHeader == null || 
                    selectedBookHeaderString == null ||
                    selectedBookHeader.TenSach != selectedBookHeaderString ||
                    selectedNXB != selectedNXBString)
                {
                    IsCanChangePrice = true;
                }
                else
                {
                    Task.Run(async() =>
                    {
                        MainViewModel.IsLoading = true;
                        decimal tempPrice = (await bookRepo.GetSingleAsync(b => b.MaDauSach == SelectedBookHeader.MaDauSach && b.NhaXuatBan == SelectedNXB)).DonGiaNhap;
                        App.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            IsCanChangePrice = false;
                            Price = tempPrice.ToString("G29");
                        }));
                        MainViewModel.IsLoading = false;
                    });
                    
                }
                OnPropertyChanged();
            }
        }
        private ICollection<TACGIA> allAuthors;
        private ObservableCollection<TACGIA> authors;
        public ObservableCollection<TACGIA> Authors
        {
            get => authors;
            set
            {
                authors = value;
                OnPropertyChanged();
            }
        }
        private String authorActionText;
        public String AuthorActionText
        {
            get => authorActionText;
            set
            {
                authorActionText = value;
                OnPropertyChanged();
            }
        }
        private ICollection<THELOAI> allCategories;
        private ObservableCollection<THELOAI> filterCategories;
        public ObservableCollection<THELOAI> FilterCategories
        {
            get => filterCategories;
            set
            {
                filterCategories = value;
                OnPropertyChanged();
            }
        }
        private THELOAI selectedCategory;
        public THELOAI SelectedCategory
        {
            get => selectedCategory;
            set
            {
                selectedCategory = value;
                OnPropertyChanged();
            }
        }
        private string selectedCategoryString;
        public string SelectedCategoryString
        {
            get => selectedCategoryString;
            set
            {
                selectedCategoryString = value;
                OnPropertyChanged();
            }
        }
        public string Amount { get; set; }
        private bool isCanChangePrice;
        public bool IsCanChangePrice
        {
            get => isCanChangePrice;
            set
            {
                isCanChangePrice = value;
                OnPropertyChanged();
            }
        }
        public string Price { get; set; }
        #endregion
        public AddBookVM()
        {
            Task.Run(async() =>
            {
                await Load();
            });
            SeeDetailAuthorsCommand = new RelayCommandWithNoParameter(async () => 
            {
                PreviousItem = MainViewModel.UpdateDialog("Main");
                MainViewModel.SetLoading(true);
                AuthorsDetail authorsdetailDialog = new AuthorsDetail();
                AuthorsDetailVM authorsDetailVM = new AuthorsDetailVM(Authors, AuthorActionText == "Chỉnh");
                authorsDetailVM.EditAuthorSuccess += OnEditAuthorSuccess;
                authorsdetailDialog.DataContext = authorsDetailVM;
                MainViewModel.SetLoading(false);
                await DialogHost.Show(authorsdetailDialog, "Main");
            });
            AddBookCommand = new RelayCommand<object>(p =>
            {
                return SelectedBookHeaderString != null && SelectedBookHeaderString.Length > 0 &&
                SelectedCategoryString != null && SelectedCategoryString.Length > 0 &&
                SelectedNXBString != null && SelectedNXBString.Length > 0 &&
                Authors != null && Authors.Count > 0 && 
                Price != null && Price.Length > 0 &&
                Amount != null && Amount.Length > 0;
            }, async p =>
            {
                //category handle
                THELOAI bookCategory;
                if (SelectedCategory == null || SelectedCategory.TenTheLoai != SelectedCategoryString)
                {
                    bookCategory = new THELOAI();
                    bookCategory.TenTheLoai = SelectedCategoryString;
                }
                else
                {
                   bookCategory = SelectedCategory;
                }

                //book header handle
                DAUSACH bookHeader;     
                if (SelectedBookHeader == null || SelectedBookHeader.TenSach != SelectedBookHeaderString)
                {
                    bookHeader = new DAUSACH();
                    bookHeader.TenSach = SelectedBookHeaderString;
                    bookHeader.THELOAI = bookCategory;
                }
                else
                {
                    bookHeader = SelectedBookHeader;
                }
                bookHeader.TACGIAs = Authors;

                //book handle
                SACH book;
                if (SelectedBookHeader == null || SelectedBookHeader.TenSach != SelectedBookHeaderString || SelectedNXB == null ||
                    (SelectedBookHeader.TenSach == SelectedBookHeaderString && SelectedNXB != SelectedNXBString))
                {
                    book = new SACH();
                    book.SoLuong = int.Parse(Amount);
                    book.DonGiaNhap = decimal.Parse(Price);
                    book.DAUSACH = bookHeader;  
                    book.NhaXuatBan = SelectedNXBString;
                }
                else
                {
                    book = await bookRepo.GetSingleAsync(
                        b => b.MaDauSach == SelectedBookHeader.MaDauSach && b.NhaXuatBan == SelectedNXB,
                        b => b.DAUSACH,
                        b => b.DAUSACH.THELOAI,
                        b => b.DAUSACH.TACGIAs);
                    book.SoLuong = int.Parse(Amount);
                }
                DialogHost.CloseDialogCommand.Execute(null, null);
                AddBookSuccess?.Invoke(book);
            });
        }
        public void OnEditAuthorSuccess(ObservableCollection<TACGIA> authors)
        {
            Authors = new ObservableCollection<TACGIA>(authors);
            DialogHost.Show(PreviousItem, "Main");
        }
        public async Task Load()
        {
            allBookHeaers = new ObservableCollection<DAUSACH>(await bookHeaderRepo.GetAllAsync(p=>p.SACHes, p=>p.TACGIAs, p=>p.THELOAI));
            FilterBookHeaders = new ObservableCollection<DAUSACH>(allBookHeaers);

            allNXBs = (await bookRepo.GetAllAsync()).Select(p=>p.NhaXuatBan).Distinct().ToList();
            FilterNXBs = new ObservableCollection<string>(allNXBs);

            allCategories = await categoryRepo.GetAllAsync();
            FilterCategories = new ObservableCollection<THELOAI>(allCategories);

            allAuthors = await authorRepo.GetAllAsync();
            AuthorActionText = "";
        }
    }
}
