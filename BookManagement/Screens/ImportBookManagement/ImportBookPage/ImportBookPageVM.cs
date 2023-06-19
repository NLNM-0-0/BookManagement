using BookManagement.Models;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Vml.Office;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.ExpressionGraph.FunctionCompilers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace BookManagement
{
    public class ImportBookPageVM : BaseViewModel
    {
        #region GenericDataRepository
        private GenericDataRepository<DAUSACH> bookHeaderRepo = new GenericDataRepository<DAUSACH>();
        private GenericDataRepository<THELOAI> categoryRepo = new GenericDataRepository<THELOAI>();
        private GenericDataRepository<TACGIA> authorRepo = new GenericDataRepository<TACGIA>();
        private GenericDataRepository<SACH> bookRepo = new GenericDataRepository<SACH>();
        private GenericDataRepository<PHIEUNHAPSACH> importRepo = new GenericDataRepository<PHIEUNHAPSACH>();
        #endregion
        #region Commands
        public ICommand AddBookCommand { get; set; }
        public ICommand DoneImportCommand { get; set; }
        public ICommand RemoveBookCommand { get; set; }
        public ICommand SearchBookCommand { get; set; }
        #endregion

        #region Props
        private string searchBy;
        public string SearchBy
        {
            get { return searchBy; }
            set
            {
                searchBy = value;
                OnPropertyChanged();
            }
        }
        private string searchByValue;
        public string SearchByValue
        {
            get { return searchByValue; }
            set
            {
                searchByValue = value;
                OnPropertyChanged();
            }
        }
        private DateTime selectedDate;
        public DateTime SelectedDate
        {
            get { return selectedDate; }
            set
            {
                selectedDate = value;
                OnPropertyChanged();
            }
        }
        private PHIEUNHAPSACH selectedImportPage;
        public PHIEUNHAPSACH SelectedImportPage
        {
            get { return selectedImportPage; }
            set
            {
                selectedImportPage = value;
                OnPropertyChanged();
            }
        }
        private bool isNew;
        public bool IsNew
        {
            get { return isNew; }
            set
            {
                isNew = value;
                OnPropertyChanged();
            }
        }
        private decimal totalPrice;
        public decimal TotalPrice
        {
            get { return totalPrice; }
            set
            {
                totalPrice = value;
                OnPropertyChanged();
            }
        }
        private List<CHITIETPHIEUNHAP> allImportDetails;
        private ObservableCollection<CHITIETPHIEUNHAP> filterImportDetails;
        public ObservableCollection<CHITIETPHIEUNHAP> FilterImportDetails
        {
            get => filterImportDetails;
            set
            {
                filterImportDetails = value;
                OnPropertyChanged();
            }
        }
        #endregion
        public ImportBookPageVM(PHIEUNHAPSACH import)
        {
            import.CHITIETPHIEUNHAPs = (import.CHITIETPHIEUNHAPs.OrderBy(p => p.SACH.DAUSACH.TenSach).ThenBy(p => p.SACH.NhaXuatBan)).ToList();
            SelectedImportPage = import;
            allImportDetails = import.CHITIETPHIEUNHAPs.ToList();
            FilterImportDetails = new ObservableCollection<CHITIETPHIEUNHAP>(allImportDetails);
            IsNew = SelectedImportPage.MaPhieuNhap == null;
            SelectedDate = SelectedImportPage.NgayNhap ?? DateTime.Now;
            TotalPrice = SelectedImportPage.TongTien;
            SearchBy = IsNew?"Tên sách":"Id";

            #region Define Commands
            AddBookCommand = new RelayCommandWithNoParameter(async () =>
            {
                MainViewModel.SetLoading(true);
                AddBook addExistingBook = new AddBook();
                AddBookVM addExistingBookVM = new AddBookVM();
                addExistingBookVM.AddBookSuccess += OnAddBookSuccess;
                addExistingBook.DataContext = addExistingBookVM;
                MainViewModel.SetLoading(false);
                await DialogHost.Show(addExistingBook, "Main");  
            });
            DoneImportCommand = new RelayCommand<object>(p=>
            {
                return allImportDetails !=null && allImportDetails.Count > 0;
            }, async(p) =>
            {
                if (IsNew)
                {
                    MainViewModel.SetLoading(true);
                    await Save();
                    MainViewModel.SetLoading(false);
                }
                NavigateProvider.ImportBookManagementScreen().Navigate();
            });
            SearchBookCommand = new RelayCommandWithNoParameter(() =>
            {
                MainViewModel.SetLoading(true);
                Search();
                MainViewModel.SetLoading(false);
            });
            RemoveBookCommand = new RelayCommand<CHITIETPHIEUNHAP>(p => p != null, p =>
            {
                allImportDetails.Remove(p);
                FilterImportDetails.Remove(p);
                TotalPrice -= p.SoLuong * p.DonGiaNhap;
            });
            
            #endregion
        }
        private void OnAddBookSuccess(SACH book, int number, decimal unitImportPrice)
        {
            if(book.MaSach==null)
            {
                List<string> authorNames = book.DAUSACH.TACGIAs.Select(a=>a.TenTacGia).ToList();
                List<CHITIETPHIEUNHAP> importListHasBookHeader = allImportDetails.Where(p =>
                {
                    if (p.SACH.DAUSACH.TenSach != book.DAUSACH.TenSach)
                    {
                        return false;
                    }
                    List<string> tempAuthorNames = p.SACH.DAUSACH.TACGIAs.Select(a => a.TenTacGia).ToList();
                    if(tempAuthorNames.Count!=authorNames.Count)
                    {
                        return false;
                    }
                    return tempAuthorNames.Intersect(authorNames).ToList().Count == authorNames.Count;
                }).ToList();
                if(importListHasBookHeader.Any(p=>p.SACH.DAUSACH.THELOAI.TenTheLoai!=book.DAUSACH.THELOAI.TenTheLoai))
                {
                    var dl = new ConfirmDialog()
                    {
                        ContentString = "Không thể tồn tại 2 sách có cùng tên, cùng nhóm tác giả nhưng khác thể loại.",
                        Header = "Oops"
                    };
                    DialogHost.Show(dl, "Main");
                    return;
                }    
                if(importListHasBookHeader.Any(p=>p.SACH.NhaXuatBan == book.NhaXuatBan && p.SACH.LanTaiBan == book.LanTaiBan))
                {
                    var dl = new ConfirmDialog()
                    {
                        ContentString = "Bạn đã thêm vào sách đã có.",
                        Header = "Oops"
                    };
                    DialogHost.Show(dl, "Main");
                    return;
                }
                CHITIETPHIEUNHAP importDetail = new CHITIETPHIEUNHAP();
                importDetail.SACH = book;
                importDetail.SoLuong = number;
                importDetail.DonGiaNhap = unitImportPrice;
                allImportDetails.Insert(0, importDetail);
                SearchByValue = "";
                FilterImportDetails = new ObservableCollection<CHITIETPHIEUNHAP>(allImportDetails);
            }
            else if(allImportDetails.Any(p => p.SACH.MaSach == book.MaSach))
            {
                var dl = new ConfirmDialog()
                {
                    ContentString = "Bạn đã thêm vào sách đã có.",
                    Header = "Oops"
                };
                DialogHost.Show(dl, "Main");
                return;
            }
            else
            {
                CHITIETPHIEUNHAP importDetail = new CHITIETPHIEUNHAP();
                importDetail.SACH = book;
                importDetail.SoLuong = number;
                importDetail.DonGiaNhap = unitImportPrice;
                allImportDetails.Insert(0, importDetail);
                SearchByValue = "";
                FilterImportDetails = new ObservableCollection<CHITIETPHIEUNHAP>(allImportDetails);
            }
            TotalPrice += number * unitImportPrice;
        }
        private void Search()
        {
            if (SearchBy == "Id" && !IsNew)
            {
                FilterImportDetails = new ObservableCollection<CHITIETPHIEUNHAP>(allImportDetails.Where(p => p.SACH.MaSach.ToLower().Trim().Contains(SearchByValue == null ? "" : SearchByValue.ToLower().Trim())));
            }
            else if (SearchBy == "Tên sách")
            {
                FilterImportDetails = new ObservableCollection<CHITIETPHIEUNHAP>(allImportDetails.Where(p => Helpers.convertToUnSign3(p.SACH.DAUSACH.TenSach).ToLower().Trim().Contains(SearchByValue == null ? "" : Helpers.convertToUnSign3(SearchByValue.Trim()).ToLower())));
            }
            else if (SearchBy == "Tác giả")
            {
                FilterImportDetails = new ObservableCollection<CHITIETPHIEUNHAP>(allImportDetails.Where(p => {
                    List<TACGIA> authors = p.SACH.DAUSACH.TACGIAs.ToList();
                    List<string> authorNames = authors.Select(author => author.TenTacGia).ToList();
                    return authorNames.Any(a => Helpers.convertToUnSign3(a).ToLower().Contains(SearchByValue == null ? "" : Helpers.convertToUnSign3(SearchByValue.Trim()).ToLower()));
                }));
            }
            else if (SearchBy == "Thể loại")
            {
                FilterImportDetails = new ObservableCollection<CHITIETPHIEUNHAP>(allImportDetails.Where(p => Helpers.convertToUnSign3(p.SACH.DAUSACH.THELOAI.TenTheLoai.Trim()).ToLower().Contains(SearchByValue == null ? "" : Helpers.convertToUnSign3(SearchByValue).ToLower().Trim())));
            }
            else if (SearchBy == "NXB")
            {
                FilterImportDetails = new ObservableCollection<CHITIETPHIEUNHAP>(allImportDetails.Where(p => Helpers.convertToUnSign3(p.SACH.NhaXuatBan.Trim()).ToLower().Contains(SearchByValue == null ? "" : Helpers.convertToUnSign3(SearchByValue.Trim()).ToLower())));
            }
        }
        private async Task Save()
        {
            //import page handle
            PHIEUNHAPSACH import = new PHIEUNHAPSACH();
            import.MaPhieuNhap = await GenerateId.Gen(typeof(PHIEUNHAPSACH), "MaPhieuNhap");
            import.NgayNhap = SelectedDate;
            import.MaNhanVien = AccountStore.instance.CurrentAccount.MaNhanVien;
            import.TongTien = TotalPrice;
            await importRepo.Add(import);

            //book handle
            for(int i = 0; i < allImportDetails.Count; i++)
            {
                SACH book = allImportDetails[i].SACH;
                if(book.MaSach == null)
                {
                    //book header handle
                    DAUSACH bookHeader;
                    if(book.DAUSACH.MaDauSach == null)
                    {
                        bookHeader = new DAUSACH();
                        bookHeader.MaDauSach = await GenerateId.Gen(typeof(DAUSACH), "MaDauSach");

                        //book's name handle
                        bookHeader.TenSach = book.DAUSACH.TenSach;

                        List<string> authorNames = book.DAUSACH.TACGIAs.Select(a => a.TenTacGia).ToList();
                        for (int u = i + 1; u < allImportDetails.Count; u++)
                        {
                            DAUSACH tempBookHeader = allImportDetails.ElementAt(u).SACH.DAUSACH;
                            List<string> tempAuthorNames = tempBookHeader.TACGIAs.Select(a => a.TenTacGia).ToList();
                            if (tempBookHeader.TenSach == bookHeader.TenSach && 
                                tempAuthorNames.Count == authorNames.Count && 
                                tempAuthorNames.Intersect(authorNames).ToList().Count == tempAuthorNames.Count)
                            {
                                tempBookHeader.MaDauSach = bookHeader.MaDauSach;
                            }
                        }

                        //author handle
                        for (int j = 0; j < book.DAUSACH.TACGIAs.Count; j++)
                        {
                            TACGIA author = book.DAUSACH.TACGIAs.ElementAt(j);
                            if (author.MaTacGia == null)
                            {
                                TACGIA doubleAuthor = await authorRepo.GetSingleAsync(p => p.TenTacGia == author.TenTacGia);
                                if (doubleAuthor == null)
                                {
                                    author.MaTacGia = await GenerateId.Gen(typeof(TACGIA), "MaTacGia");
                                    await authorRepo.Add(author);
                                }
                                else
                                {
                                    author.MaTacGia = doubleAuthor.MaTacGia;
                                }
                            }
                        }

                        //category handle
                        THELOAI bookCategory;
                        if(book.DAUSACH.THELOAI.MaTheLoai == null)
                        {
                            bookCategory = new THELOAI();
                            bookCategory.MaTheLoai = await GenerateId.Gen(typeof(THELOAI), "MaTheLoai");
                            bookCategory.TenTheLoai = book.DAUSACH.THELOAI.TenTheLoai;
                            for (int u = i + 1; u < allImportDetails.Count; u++)
                            {
                                THELOAI tempCategory = allImportDetails.ElementAt(u).SACH.DAUSACH.THELOAI;
                                if (tempCategory.TenTheLoai == bookCategory.TenTheLoai)
                                {
                                    tempCategory.MaTheLoai = bookCategory.MaTheLoai;
                                }
                            }
                            await categoryRepo.Add(bookCategory);
                        }    
                        else
                        {
                            bookCategory = book.DAUSACH.THELOAI;
                        }
                        bookHeader.MaTheLoai = bookCategory.MaTheLoai;

                        
                        await bookHeaderRepo.Add(bookHeader);
                    }
                    else
                    {
                        bookHeader = book.DAUSACH;
                    }

                    for (int j = 0; j < book.DAUSACH.TACGIAs.Count; j++)
                    {
                        TACGIA author = book.DAUSACH.TACGIAs.ElementAt(j);
                        try
                        {
                            await AuthorDetailAPI.Add(author.MaTacGia, bookHeader.MaDauSach);
                        }
                        catch 
                        {
                            //Đã có CHITIETTACGIA này trong hệ thống
                        }
                    }

                    book.MaDauSach = bookHeader.MaDauSach;
                    book.MaSach = await GenerateId.Gen(typeof(SACH), "MaSach");
                    book.DAUSACH = null;
                    book.IsActive = true;
                    await bookRepo.Add(book);
                }
                else
                {
                    SACH tempBook = await bookRepo.GetSingleAsync(p=>p.MaSach == book.MaSach);
                    tempBook.SoLuong += allImportDetails[i].SoLuong;
                    tempBook.DonGiaNhapMoiNhat = allImportDetails[i].DonGiaNhap;
                    await bookRepo.Update(tempBook);
                }
                await ImportDetailAPI.Add(book.MaSach, import.MaPhieuNhap, allImportDetails[i].SoLuong, allImportDetails[i].DonGiaNhap);
            }    
        }
    }
}
