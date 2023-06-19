using BookManagement.Models;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BookManagement
{
    public class AddBookVM : BaseViewModel
    {
        #region
        public Action<SACH,int, decimal> AddBookSuccess;
        #endregion
        #region GenericDataRepository
        private GenericDataRepository<DAUSACH> bookHeaderRepo = new GenericDataRepository<DAUSACH>();
        private GenericDataRepository<THELOAI> categoryRepo = new GenericDataRepository<THELOAI>();
        private GenericDataRepository<TACGIA> authorRepo = new GenericDataRepository<TACGIA>();
        private GenericDataRepository<SACH> bookRepo = new GenericDataRepository<SACH>();
        #endregion

        #region Commands
        public ICommand ChooseAuthorsGroupCommand { get; set; }
        public ICommand AddBookCommand { get; set; }
        private System.Windows.Controls.UserControl PreviousItem;
        #endregion

        #region Properties
        private ICollection<SACH> allBooks;
        private ICollection<DAUSACH> allBookHeaders;
        private ObservableCollection<string> bookHeaderNames;
        public ObservableCollection<string> BookHeaderNames
        {
            get => bookHeaderNames;
            set
            {
                bookHeaderNames = value;
                OnPropertyChanged();
            }
        }
        private string selectedBookHeaderName;
        public string SelectedBookHeaderName
        {
            get => selectedBookHeaderName;
            set
            {
                selectedBookHeaderName = value;
                OnPropertyChanged();
            }
        }
        private string selectedBookHeaderNameString;
        public string SelectedBookHeaderNameString
        {
            get => selectedBookHeaderNameString;
            set
            {
                selectedBookHeaderNameString = value;
                if(String.IsNullOrEmpty(value))
                {
                    FinalBookHeader = null;
                }    
                else if(SelectedBookHeaderName == value && Authors!=null && Authors.Count != 0)
                {
                    Task.Run(() =>
                    {
                        try
                        {
                            List<String> authorNames = Authors.Select(a => a.TenTacGia).ToList();
                            FinalBookHeader = allBookHeaders.Single(p =>
                            {
                                if (p.TenSach != selectedBookHeaderName)
                                {
                                    return false;
                                }
                                List<String> tempAuthorNames = p.TACGIAs.Select(a => a.TenTacGia).ToList();
                                if (tempAuthorNames.Count != authorNames.Count)
                                {
                                    return false;
                                }
                                if (tempAuthorNames.Intersect(authorNames).ToList().Count != authorNames.Count)
                                {
                                    return false;
                                }
                                return true;
                            });
                        }
                        catch
                        {
                            FinalBookHeader = new DAUSACH();
                        }
                    });
                }   
                else
                {
                    FinalBookHeader = new DAUSACH();
                }    
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
                        FilterTimeRepublish = new ObservableCollection<int>();
                        SelectedNXBString = "";
                        SelectedCategoryString = "";
                    }
                    else
                    {
                        FilterNXBs = new ObservableCollection<string>((finalBookHeader.SACHes).Select(p => p.NhaXuatBan).Distinct());
                        FilterTimeRepublish = new ObservableCollection<int>();
                        SelectedCategory = finalBookHeader.THELOAI;
                        SelectedCategoryString = finalBookHeader.THELOAI.TenTheLoai;
                    }
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
                if (String.IsNullOrEmpty(selectedBookHeaderNameString))
                {
                    FinalBookHeader = null;
                }
                else if (SelectedBookHeaderName == selectedBookHeaderNameString && value != null && value.Count != 0)
                {
                    Task.Run(() =>
                    {
                        try
                        {
                            List<String> authorNames = Authors.Select(a => a.TenTacGia).ToList();
                            FinalBookHeader = allBookHeaders.Single(p =>
                            {
                                if (p.TenSach != selectedBookHeaderName)
                                {
                                    return false;
                                }
                                List<String> tempAuthorNames = p.TACGIAs.Select(a => a.TenTacGia).ToList();
                                if (tempAuthorNames.Count != authorNames.Count)
                                {
                                    return false;
                                }
                                if (tempAuthorNames.Intersect(authorNames).ToList().Count != authorNames.Count)
                                {
                                    return false;
                                }
                                return true;
                            });
                        }
                        catch
                        {
                            FinalBookHeader = new DAUSACH();
                        }
                    });
                }
                else
                {
                    FinalBookHeader = new DAUSACH();
                }
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
                SelectedTimeRepublishString = "";
                SelectedTimeRepublish = null;
                if(String.IsNullOrEmpty(selectedNXBString) ||
                    selectedNXB == null ||
                    selectedNXB != selectedNXBString ||
                    finalBookHeader == null ||
                    finalBookHeader.MaDauSach == null)
                {
                    FilterTimeRepublish = new ObservableCollection<int>();
                }
                else
                {
                    FilterTimeRepublish = new ObservableCollection<int>(
                        allBooks.Where(
                            b=>b.MaDauSach == FinalBookHeader.MaDauSach && b.NhaXuatBan == SelectedNXB
                        ).Select(b=>b.LanTaiBan).ToList()
                    );
                }
                OnPropertyChanged();
            }
        }
        private ObservableCollection<int> filterTimeRepublish;
        public ObservableCollection<int> FilterTimeRepublish
        {
            get => filterTimeRepublish;
            set
            {
                filterTimeRepublish = value;
                OnPropertyChanged();
            }
        }
        private int? selectedTimeRepublish;
        public int? SelectedTimeRepublish
        {
            get => selectedTimeRepublish;
            set
            {
                selectedTimeRepublish = value;
                OnPropertyChanged();
            }    
        }
        private string selectedTimeRepublishString;
        public string SelectedTimeRepublishString
        {
            get => selectedTimeRepublishString;
            set
            {
                selectedTimeRepublishString = value;
                int temp;
                if(int.TryParse(selectedTimeRepublishString, out temp))
                {
                    if (selectedTimeRepublish != null &&
                        !String.IsNullOrEmpty(value) ||
                        selectedTimeRepublish == temp
                    )
                    {
                        Task.Run(async () =>
                        {
                            decimal tempPrice = (await bookRepo.GetSingleAsync(
                                b => b.MaDauSach == FinalBookHeader.MaDauSach
                                && b.NhaXuatBan == SelectedNXB
                                && b.LanTaiBan == SelectedTimeRepublish)
                            ).DonGiaNhapMoiNhat;
                            App.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                Price = tempPrice.ToString("G29");
                            }));
                        });
                    }
                }  
                OnPropertyChanged();    
            }    
        }
        public string Amount { get; set; }

        public string Price { get; set; }
        #endregion
        public AddBookVM()
        {
            Task.Run(async() =>
            {
                await Load();
            });
            ChooseAuthorsGroupCommand = new RelayCommandWithNoParameter(async () => 
            {
                PreviousItem = MainViewModel.UpdateDialog("Main");
                MainViewModel.SetLoading(true);
                SelectAuthorGroupDialog selectAuthorGroupDialog = new SelectAuthorGroupDialog();
                SelectAuthorGroupDialogVM selectAuthorGroupDialogVM = new SelectAuthorGroupDialogVM(SelectedBookHeaderNameString);
                selectAuthorGroupDialogVM.CloseDialogCommand = new RelayCommandWithNoParameter(async () =>
                {
                    DialogHost.CloseDialogCommand.Execute(null, null);
                    if (PreviousItem != null)
                    {
                        await DialogHost.Show(PreviousItem, "Main");
                    }
                });
                selectAuthorGroupDialogVM.ChooseAuthorSuccess += OnChooseAuthorSuccess;
                selectAuthorGroupDialog.DataContext = selectAuthorGroupDialogVM;
                MainViewModel.SetLoading(false);
                await DialogHost.Show(selectAuthorGroupDialog, "Main");
            });
            AddBookCommand = new RelayCommand<object>(p =>
            {
                return !String.IsNullOrEmpty(SelectedBookHeaderNameString) &&
                !String.IsNullOrEmpty(SelectedCategoryString) &&
                !String.IsNullOrEmpty(SelectedNXBString) &&
                Authors != null && Authors.Count > 0 && 
                !String.IsNullOrEmpty(Price) &&
                !String.IsNullOrEmpty(Amount);
            }, async p =>
            {
                int selectedTimeRepublish = -1;
                if(int.TryParse(SelectedTimeRepublishString, out selectedTimeRepublish) && selectedTimeRepublish >= 0)
                {  
                    if (int.Parse(Amount) < RuleStore.instance.getValue(AppEnum.LuongSachNhapItNhat))
                    {
                        PreviousItem = MainViewModel.UpdateDialog("Main");
                        var dl = new ConfirmDialog()
                        {
                            Header = "Oops",
                            ContentString = "Bạn phải nhập sách số lượng lớn hơn " + RuleStore.instance.getValue(AppEnum.LuongSachNhapItNhat).ToString() + ".",
                            CM = new RelayCommandWithNoParameter(() =>
                            {
                                DialogHost.CloseDialogCommand.Execute(null, null);
                                if (PreviousItem != null)
                                {
                                    DialogHost.Show(PreviousItem, "Main");
                                }
                            })
                        };
                        await DialogHost.Show(dl, "Main");
                    }
                    else
                    {
                        //category handle
                        THELOAI bookCategory;
                        if (SelectedCategory == null || SelectedCategory.TenTheLoai != SelectedCategoryString)
                        {
                            bookCategory = new THELOAI();
                            bookCategory.TenTheLoai = SelectedCategoryString.Trim();
                        }
                        else
                        {
                            bookCategory = SelectedCategory;
                        }

                        //book header handle
                        DAUSACH bookHeader;
                        if (FinalBookHeader != null && FinalBookHeader.MaDauSach != null)
                        {
                            bookHeader = FinalBookHeader;
                        }
                        else
                        {
                            bookHeader = new DAUSACH();
                            bookHeader.TenSach = SelectedBookHeaderNameString.Trim();
                            bookHeader.THELOAI = bookCategory;
                            bookHeader.TACGIAs = Authors;
                        }

                        //book handle
                        SACH book;
                        if (bookHeader == null || 
                            bookHeader.MaDauSach == null || 
                            SelectedNXB == null ||
                            SelectedNXB != SelectedNXBString ||
                            SelectedTimeRepublish == null ||
                            SelectedTimeRepublish != selectedTimeRepublish)
                        {
                            book = new SACH();
                            book.SoLuong = int.Parse(Amount);
                            book.DonGiaNhapMoiNhat = decimal.Parse(Price);
                            book.DAUSACH = bookHeader;
                            book.NhaXuatBan = SelectedNXBString.Trim();
                            book.LanTaiBan = selectedTimeRepublish;
                            DialogHost.CloseDialogCommand.Execute(null, null);
                            AddBookSuccess?.Invoke(book, book.SoLuong ?? 0, book.DonGiaNhapMoiNhat);
                        }
                        else
                        {
                            book = await bookRepo.GetSingleAsync(
                                b => b.MaDauSach == bookHeader.MaDauSach && b.NhaXuatBan == SelectedNXB && b.LanTaiBan == selectedTimeRepublish,
                                b => b.DAUSACH,
                                b => b.DAUSACH.THELOAI,
                                b => b.DAUSACH.TACGIAs);      
                            if (book.SoLuong >= RuleStore.instance.getValue(AppEnum.LuongTonToiDaKhiNhap))
                            {
                                PreviousItem = MainViewModel.UpdateDialog("Main");
                                var dl = new ConfirmDialog()
                                {
                                    Header = "Oops",
                                    ContentString = "Chỉ nhập các sách có lượng tồn ít hơn " + RuleStore.instance.getValue(AppEnum.LuongTonToiDaKhiNhap).ToString() + ".",
                                    CM = new RelayCommandWithNoParameter(() =>
                                    {
                                        DialogHost.CloseDialogCommand.Execute(null, null);
                                        if (PreviousItem != null)
                                        {
                                            DialogHost.Show(PreviousItem, "Main");
                                        }
                                    })
                                };
                                await DialogHost.Show(dl, "Main");
                            }
                            else
                            {
                                DialogHost.CloseDialogCommand.Execute(null, null);
                                AddBookSuccess?.Invoke(book, int.Parse(Amount), Decimal.Parse(Price));
                            }
                        }
                    }
                }
                else
                {
                    PreviousItem = MainViewModel.UpdateDialog("Main");
                    var dl = new ConfirmDialog()
                    {
                        Header = "Oops",
                        ContentString = "Số lần xuất bản bạn đang nhập không phải số tự nhiên. Xin hãy nhập lại.",
                        CM = new RelayCommandWithNoParameter(() =>
                        {
                            DialogHost.CloseDialogCommand.Execute(null, null);
                            if (PreviousItem != null)
                            {
                                DialogHost.Show(PreviousItem, "Main");
                            }
                        })
                    };
                    await DialogHost.Show(dl, "Main");

                }    
            });
        }
        public void OnChooseAuthorSuccess(List<TACGIA> authors)
        {
            Authors = new ObservableCollection<TACGIA>(authors);
            if(PreviousItem!= null)
            {
                DialogHost.Show(PreviousItem, "Main");
            }
        }
        public async Task Load()
        {
            allBooks = await bookRepo.GetAllAsync();
            allBookHeaders = new ObservableCollection<DAUSACH>(await bookHeaderRepo.GetAllAsync(p=>p.SACHes, p=>p.TACGIAs, p=>p.THELOAI));
            BookHeaderNames = new ObservableCollection<string>(allBookHeaders.Select(p => p.TenSach).Distinct().ToList());

            allNXBs = (await bookRepo.GetAllAsync()).Select(p=>p.NhaXuatBan).Distinct().ToList();
            FilterNXBs = new ObservableCollection<string>(allNXBs);

            allCategories = await categoryRepo.GetAllAsync();
            FilterCategories = new ObservableCollection<THELOAI>(allCategories);

            allAuthors = await authorRepo.GetAllAsync();
        }
    }
}
