using BookManagement.Models;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using MaterialDesignThemes.Wpf;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BookManagement
{
    public class ImportBookManagementScreenVM : BaseViewModel
    {
        #region Access property
        public bool IsAllowImportBook =>
            AccountStore.instance.CurrentAccount.NHOMNGUOIDUNG.CHUCNANGs.Any(p => p.MaChucNang == AppEnum.LapPhieuNhapSach);
        public bool IsAllowSearchImport =>
            AccountStore.instance.CurrentAccount.NHOMNGUOIDUNG.CHUCNANGs.Any(p => p.MaChucNang == AppEnum.TraCuuPhieuNhapSach);
        #endregion

        #region GenericDataRepository
        GenericDataRepository<PHIEUNHAPSACH> importRepo = new GenericDataRepository<PHIEUNHAPSACH>();
        #endregion

        #region Commands
        public ICommand SearchCommand { get; set; }
        public ICommand ResetCommand { get; set; }
        public ICommand ShowImportCommand { get; set; }
        public ICommand ChangeScreenCommand { get; set; }
        public ICommand ExportExcelCommand { get; set; }
        public ICommand ImportExcelCommand { get; set; }
        #endregion

        #region Properties
        public string SearchId { get; set; }
        public DateTime? SearchDate { get; set; }
        public string SearchMinPrice { get; set; }
        public string SearchMaxPrice { get; set; }
        public string SearchStaffName { get; set; }

        private ObservableCollection<PHIEUNHAPSACH> filterImports;
        public ObservableCollection<PHIEUNHAPSACH> FilterImports
        {
            get => filterImports;
            set
            {
                filterImports = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<PHIEUNHAPSACH> allImports;
        public ObservableCollection<PHIEUNHAPSACH> AllImports
        {
            get => allImports;
            set
            {
                allImports = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructor
        public ImportBookManagementScreenVM()
        {
            Task.Run(async() =>
            {
                MainViewModel.IsLoading = true;
                if(IsAllowSearchImport)
                {
                    await Load();
                }    
                else
                {
                    AllImports = new ObservableCollection<PHIEUNHAPSACH> { };
                    FilterImports = new ObservableCollection<PHIEUNHAPSACH> { };
                }    
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
                    SearchStaffName = "";
                    FilterImports = new ObservableCollection<PHIEUNHAPSACH>(AllImports);
                });
                ShowImportCommand = new RelayCommand<PHIEUNHAPSACH>(p => true, p =>
                {
                    NavigateProvider.ImportBookPage().Navigate(p);
                });
                ChangeScreenCommand = new RelayCommandWithNoParameter(() =>
                {
                    PHIEUNHAPSACH import = new PHIEUNHAPSACH();
                    NavigateProvider.ImportBookPage().Navigate(import);
                });
                ExportExcelCommand = new RelayCommand<PHIEUNHAPSACH>(p => p != null, async(p) =>
                {
                    MainViewModel.IsLoading = true;
                    bool result = await saveExcelFile(p);
                    if(result)
                    {
                        MainViewModel.SetLoading(false);
                    }    
                    else
                    {
                        var dl = new ConfirmDialog()
                        {
                            ContentString="File đang được sử dụng. Xin hãy đóng file rùi thử lại!",
                            Header ="Oops"
                        };
                        MainViewModel.IsLoading = false;
                        await DialogHost.Show(dl, "Main");
                    }    
                });
                ImportExcelCommand = new RelayCommandWithNoParameter(async () =>
                {
                    MainViewModel.IsLoading = true;
                    ImportExcel importExcel = new ImportExcel();
                    ImportExcelVM importExcelVM = new ImportExcelVM();
                    importExcelVM.ImportSuccess += OnImportSuccess;
                    importExcel.DataContext = importExcelVM;
                    MainViewModel.IsLoading = false;
                    await DialogHost.Show(importExcel, "Main");
                });
                MainViewModel.IsLoading = false;
            });
        }
        #endregion
        private async void OnImportSuccess()
        {
            MainViewModel.IsLoading = true;
            DialogHost.CloseDialogCommand.Execute(null, null);
            await Load();
            Search();
            MainViewModel.IsLoading = false;
        }
        private async Task<bool> saveExcelFile(PHIEUNHAPSACH import)
        {
            string userRoot = System.Environment.GetEnvironmentVariable("USERPROFILE");
            string downloadFolder = Path.Combine(userRoot, "Downloads");

            FileInfo fileInfo = new FileInfo($"{downloadFolder}\\{import.MaPhieuNhap}.xlsx");
            if(!IsFileLocked(fileInfo))
            {
                if (fileInfo.Exists)
                {
                    fileInfo.Delete();
                }
                using (var package = new ExcelPackage(fileInfo))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("MainReport");
                    worksheet.Cells[1, 1].Value = "Mã phiếu nhập";
                    worksheet.Cells[1, 1].Style.Font.Bold = true;
                    worksheet.Cells[1, 2].Value = import.MaPhieuNhap;

                    worksheet.Cells[2, 1].Value = "Tên nhân viên";
                    worksheet.Cells[2, 1].Style.Font.Bold = true;
                    worksheet.Cells[2, 2].Value = import.NHANVIEN.TenNhanVien;

                    worksheet.Cells[3, 1].Value = "Ngày nhập";
                    worksheet.Cells[3, 1].Style.Font.Bold = true;
                    worksheet.Cells[3, 2].Value = import.NgayNhap?.ToString("dd/MM/yyyy") ?? "";

                    worksheet.Cells[5, 1].Value = "Mã sách";
                    worksheet.Cells[5, 2].Value = "Tên sách";
                    worksheet.Cells[5, 3].Value = "Nhà xuất bản";
                    worksheet.Cells[5, 4].Value = "Đơn giá";
                    worksheet.Cells[5, 5].Value = "Số lượng";
                    worksheet.Cells[5, 6].Value = "Tổng";
                    for (int i = 0; i < import.CHITIETPHIEUNHAPs.Count; i++)
                    {
                        CHITIETPHIEUNHAP importDetail = import.CHITIETPHIEUNHAPs.ElementAt(i);
                        worksheet.Cells[6 + i, 1].Value = importDetail.SACH.MaSach;
                        worksheet.Cells[6 + i, 2].Value = importDetail.SACH.DAUSACH.TenSach;
                        worksheet.Cells[6 + i, 3].Value = importDetail.SACH.NhaXuatBan;
                        worksheet.Cells[6 + i, 4].Value = importDetail.SACH.DonGiaNhap;
                        worksheet.Cells[6 + i, 5].Value = importDetail.SoLuong;
                        worksheet.Cells[6 + i, 6].Value = importDetail.SoLuong * importDetail.SACH.DonGiaNhap;
                    }
                    using (ExcelRange excelRange = worksheet.Cells[$"A5:F{5 + import.CHITIETPHIEUNHAPs.Count}"])
                    {
                        excelRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        excelRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        excelRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        excelRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }
                    using (ExcelRange excelRange = worksheet.Cells[$"A{6 + import.CHITIETPHIEUNHAPs.Count}:F{6 + import.CHITIETPHIEUNHAPs.Count}"])
                    {
                        excelRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }
                    worksheet.Cells[6 + import.CHITIETPHIEUNHAPs.Count, 5].Value = "Tổng tiền";
                    worksheet.Cells[6 + import.CHITIETPHIEUNHAPs.Count, 5].Style.Font.Bold = true;

                    worksheet.Cells[6 + import.CHITIETPHIEUNHAPs.Count, 6].Value = import.TongTien;
                    worksheet.Cells[6 + import.CHITIETPHIEUNHAPs.Count, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[6 + import.CHITIETPHIEUNHAPs.Count, 6].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[6 + import.CHITIETPHIEUNHAPs.Count, 6].Style.Font.Bold = true;



                    var allCells = worksheet.Cells[1, 1, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column];
                    var cellFont = allCells.Style.Font;
                    cellFont.SetFromFont("Times New Roman", 13);

                    worksheet.Cells.AutoFitColumns();
                    for (int i = 1; i <= 6; i++)
                    {
                        worksheet.Column(i).Width *= 1.2;
                    }

                    await package.SaveAsAsync(fileInfo);

                    System.Diagnostics.Process.Start(fileInfo.FullName);
                }
                return true;
            }    
            else
            {
                return false;
            }    
        }
        private bool IsFileLocked(FileInfo file)
        {
            try
            {
                if (!file.Exists)
                {
                    return false;
                }
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                return true;
            }
            return false;
        }
        public async Task Load()
        {
            AllImports = new ObservableCollection<PHIEUNHAPSACH>(
                (await importRepo.GetAllAsync(
                    p => p.NHANVIEN,
                    p => p.CHITIETPHIEUNHAPs,
                    p => p.CHITIETPHIEUNHAPs.Select(ct => ct.SACH),
                    p => p.CHITIETPHIEUNHAPs.Select(ct => ct.SACH.DAUSACH),
                    p => p.CHITIETPHIEUNHAPs.Select(ct => ct.SACH.DAUSACH.TACGIAs),
                    p => p.CHITIETPHIEUNHAPs.Select(ct => ct.SACH.DAUSACH.THELOAI)
                )).ToList().OrderByDescending(p => p.NgayNhap)
            );
            FilterImports = new ObservableCollection<PHIEUNHAPSACH>(AllImports);
        }

        public void Search()
        {
            ICollection<PHIEUNHAPSACH> temp = AllImports.Where(p => {
                bool checkId = SearchId == null || SearchId == "" || p.MaPhieuNhap.ToLower().Contains(SearchId.Trim().ToLower());
                if(!checkId)
                {
                    return false;
                }    
                bool checkDate = SearchDate == null || (SearchDate != null && p.NgayNhap == SearchDate);
                if (!checkDate)
                {
                    return false;
                }
                bool checkMinTotalPrice = SearchMinPrice == null || SearchMinPrice == "" || p.TongTien >= decimal.Parse(SearchMinPrice);
                if (!checkMinTotalPrice)
                {
                    return false;
                }
                bool checkMaxTotalPrice = SearchMaxPrice == null || SearchMaxPrice == "" || p.TongTien <= decimal.Parse(SearchMaxPrice);
                if (!checkMaxTotalPrice)
                {
                    return false;
                }
                bool checkStaffName = SearchStaffName == null || SearchStaffName == "" || p.NHANVIEN.TenNhanVien.ToLower().Contains(SearchStaffName.Trim().ToLower());
                if (!checkStaffName)
                {
                    return false;
                }
                return true;
            }).ToList();
            FilterImports = new ObservableCollection<PHIEUNHAPSACH>(temp);
        }
    }
}
