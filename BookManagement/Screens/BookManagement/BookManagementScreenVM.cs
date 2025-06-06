﻿using BookManagement.Models;
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
    internal class BookManagementScreenVM : BaseViewModel
    {
        #region Access property
        public bool IsAllowChangeStatusOfBook =>
            AccountStore.instance.CurrentAccount.NHOMNGUOIDUNG.CHUCNANGs.Any(p => p.MaChucNang == AppEnum.DoiTrangThaiBanCuaSach);
        #endregion
        #region Public Properties
        private GenericDataRepository<SACH> sachRepo;
        private GenericDataRepository<DAUSACH> dauSachRepo;
        public List<SACH> BookList { get; set; }
        private List<SACH> bannedList;
        private List<SACH> notbannedList;
        public ObservableCollection<SACH> FilteredBooks { get; set; }

        private string _searchName;

        public string SearchName
        {
            get { return _searchName; }
            set { _searchName = value; OnPropertyChanged(); }
        }

        private string _searchAuthor;

        public string SearchAuthor
        {
            get { return _searchAuthor; }
            set { _searchAuthor = value; OnPropertyChanged(); }
        }
        private string _searchNXB;

        public string SearchNXB
        {
            get { return _searchNXB; }
            set { _searchNXB = value; OnPropertyChanged(); }
        }
        private string _searchType;

        public string SearchType
        {
            get { return _searchType; }
            set { _searchType = value; OnPropertyChanged(); }
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

        private string minQuantitySearch;
        public string MinQuantitySearch
        {
            get => minQuantitySearch;
            set
            {
                minQuantitySearch = value;
                OnPropertyChanged();
            }
        }
        private string maxQuantityearch;
        public string MaxQuantitySearch
        {
            get => maxQuantityearch;
            set
            {
                maxQuantityearch = value;
                OnPropertyChanged();
            }
        }

        private bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (value)
                {
                    StatusText = "Đang bán";
                    BookList = notbannedList;
                    FilteredBooks = new ObservableCollection<SACH>(BookList);
                    RemoveOrUnBanned = "Dừng bán";
                }
                else
                {
                    StatusText = "Dừng bán";
                    BookList = bannedList;
                    FilteredBooks = new ObservableCollection<SACH>(BookList);
                    RemoveOrUnBanned = "Bán tiếp";
                }
                _isChecked = value;
            }
        }
        private string _removeOrUnBanned;
        public string RemoveOrUnBanned
        {
            get => _removeOrUnBanned;
            set { _removeOrUnBanned = value; OnPropertyChanged(); }
        }

        private string _statusText;
        public string StatusText
        {
            get => _statusText;
            set { _statusText = value; OnPropertyChanged(); }
        }
        #endregion

        #region Commands
        public ICommand SearchCommand { get; set; }
        public ICommand ResetCommand { get; set; }
        public ICommand BanBookCommand { get; set; }
        #endregion

        #region Constructor
        public BookManagementScreenVM()
        {
            sachRepo = new GenericDataRepository<SACH>();
            dauSachRepo = new GenericDataRepository<DAUSACH>();
            SearchCommand = new RelayCommandWithNoParameter(async () => await Search());
            ResetCommand = new RelayCommandWithNoParameter(async () =>
            {
                SearchName = string.Empty;
                SearchAuthor = string.Empty;
                SearchNXB = string.Empty;
                SearchType = string.Empty;
                MaxPriceSearch = string.Empty;
                MinPriceSearch = string.Empty;
                MaxQuantitySearch = string.Empty;
                MinQuantitySearch = string.Empty;
                await Search();
            });
            BanBookCommand = new RelayCommand<object>((p) => p != null, async (p) => await BanBook(p));
            Task.Run(async () =>
            {
                MainViewModel.SetLoading(true);
                await Load();
            }).ContinueWith((first) =>
            {
                MainViewModel.SetLoading(false);

            });

        }
        private async Task BanBook(object obj)
        {
            MainViewModel.SetLoading(true);

            var removeBook = obj as SACH;
            if (removeBook == null)
                return;

            if (removeBook.IsActive == true)
            {
                removeBook.IsActive = false;
                notbannedList.Remove(removeBook);
                bannedList.Insert(0, removeBook);
                BookList = notbannedList;
                FilteredBooks = new ObservableCollection<SACH>(BookList);
            }
            else
            {
                removeBook.IsActive = true;
                notbannedList.Insert(0, removeBook);
                bannedList.Remove(removeBook);
                BookList = bannedList;
                FilteredBooks = new ObservableCollection<SACH>(BookList);
            }

            await sachRepo.Update(removeBook);
            MainViewModel.SetLoading(false);
        }
        private async Task Load()
        {
            BookList = new List<SACH>(
                await sachRepo.GetListAsync(s => s.SoLuong > 0, s => s.DAUSACH, s => s.DAUSACH.TACGIAs, s => s.DAUSACH.THELOAI))
                .OrderBy(p => p.DAUSACH.TenSach).ThenBy(p => p.NhaXuatBan).ToList();
            bannedList = BookList.Where(b => b.IsActive == false).ToList();
            notbannedList = BookList.Where(b => b.IsActive == true).ToList();
            RemoveOrUnBanned = "Dừng bán";
            StatusText = "Đang bán";
            IsChecked = true;
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                FilteredBooks = new ObservableCollection<SACH>(notbannedList);
            }));
        }
        #endregion

        #region Command Methods

        public async Task Search()
        {
            MainViewModel.SetLoading(true);

            double maxPrice = double.MaxValue;
            double minPrice = 0;
            int maxQuantity = int.MaxValue;
            int minQuantity = 0;
            if (!String.IsNullOrEmpty(MaxPriceSearch))
            {
                double.TryParse(MaxPriceSearch, out maxPrice);
            }
            if (!String.IsNullOrEmpty(MinPriceSearch))
            {
                double.TryParse(MinPriceSearch, out minPrice);
            }
            if (!String.IsNullOrEmpty(MaxQuantitySearch))
            {
                int.TryParse(MaxQuantitySearch, out maxQuantity);
            }
            if (!String.IsNullOrEmpty(MinQuantitySearch))
            {
                int.TryParse(MinQuantitySearch, out minQuantity);
            }
            if (minPrice > maxPrice)
            {
                MainViewModel.SetLoading(true);
                ConfirmDialog notification = new ConfirmDialog()
                {
                    Header = "Lỗi",
                    ContentString = "Giá tối thiểu đang lớn hơn giá tối đa."
                };
                MainViewModel.SetLoading(false);
                await DialogHost.Show(notification, "Main");

                return;
            }
            else if (minQuantity > maxQuantity)
            {
                MainViewModel.SetLoading(true);
                ConfirmDialog notification = new ConfirmDialog()
                {
                    Header = "Lỗi",
                    ContentString = "Số lượng tối thiểu đang lớn hơn số lượng tối đa."
                };
                MainViewModel.SetLoading(false);
                await DialogHost.Show(notification, "Main");

                return;
            }
            else
            {
                MainViewModel.SetLoading(true);
                FilteredBooks = new ObservableCollection<SACH>(BookList.Where(s => isContainSearch(s, maxPrice, minPrice, maxQuantity, minQuantity)).ToList());

                MainViewModel.SetLoading(false);
            }

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                FilteredBooks = new ObservableCollection<SACH>(FilteredBooks);
            }));
            MainViewModel.SetLoading(false);
        }

        private bool isContainSearch(SACH s, double maxPrice = double.MaxValue,
                                            double minPrice = 0,
                                            int maxQuantity = int.MaxValue,
                                            int minQuantity = 0)
        {
            var isContain = true;

            if (s.SoLuong >= minQuantity && s.SoLuong <= maxQuantity
                && (double)s.DonGiaNhapMoiNhat >= minPrice && (double)s.DonGiaNhapMoiNhat <= maxPrice)
            {
                isContain = true;
            }
            else
            {
                return false;
            }

            if (!string.IsNullOrEmpty(SearchName))
            {
                isContain = isContain
                    && (Helpers.convertToUnSign3(s.DAUSACH.TenSach)).ToLower().Trim()
                    .Contains(Helpers.convertToUnSign3(SearchName).ToLower().Trim());

                if (!isContain)
                {
                    return isContain;
                }
            }

            if (!string.IsNullOrEmpty(SearchAuthor))
            {
                if (s.DAUSACH != null && s.DAUSACH.TACGIAs != null)
                {
                    var temp = false;
                    foreach (var t in s.DAUSACH.TACGIAs)
                    {
                        if ((Helpers.convertToUnSign3(t.TenTacGia)).ToLower().Trim()
                            .Contains(Helpers.convertToUnSign3(SearchAuthor).ToLower().Trim()))
                        {
                            temp = true;
                            break;
                        }
                    }

                    if (!temp)
                    {
                        isContain = false;
                    }
                }
            }

            if (!string.IsNullOrEmpty(SearchNXB))
            {
                isContain = isContain
                    && (Helpers.convertToUnSign3(s.NhaXuatBan)).ToLower().Trim()
                    .Contains(Helpers.convertToUnSign3(SearchNXB).ToLower().Trim());

                if (!isContain)
                {
                    return isContain;
                }
            }

            if (!string.IsNullOrEmpty(SearchType))
            {
                if (s.DAUSACH != null && s.DAUSACH.THELOAI != null)
                {
                    isContain = isContain
                        && (Helpers.convertToUnSign3(s.DAUSACH.THELOAI.TenTheLoai)).ToLower().Trim()
                        .Contains(Helpers.convertToUnSign3(SearchType).ToLower().Trim());

                    if (!isContain)
                    {
                        return isContain;
                    }
                }
                else
                {
                    return false;
                }
            }

            return isContain;
        }

        #endregion
    }
}