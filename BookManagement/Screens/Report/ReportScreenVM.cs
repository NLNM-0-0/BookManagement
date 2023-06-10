using BookManagement.Models;
using DocumentFormat.OpenXml.Bibliography;
using LiveCharts;
using LiveCharts.Wpf;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Microsoft.Xaml.Behaviors.Core;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BookManagement
{
    internal class ReportScreenVM : BaseViewModel
    {
        #region Properties
        private GenericDataRepository<BAOCAOTON> stockRepo;
        private GenericDataRepository<SACH> sachRepo;
        private GenericDataRepository<KHACHHANG> khRepo;
        private GenericDataRepository<PHIEUNHAPSACH> phieuNhapRepo;
        private GenericDataRepository<HOADON> hoaDonRepo;
        private GenericDataRepository<CHITIETBAOCAOTON> ctStockRepo;
        private List<SACH> allSach;
        private List<PHIEUNHAPSACH> allPhieuNhap;
        private List<PHIEUTHUNO> allPhieuThu;
        private List<HOADON> allHoaDon;
        private List<KHACHHANG> allKhachHang;
        private DateTime earliestPhieuNhap;
        private DateTime earliestHoaDon = DateTime.Now;

        private GenericDataRepository<BAOCAOCONGNO> debtRepo;
        private GenericDataRepository<CHITIETBAOCAOCONGNO> ctDebtRepo;
        private GenericDataRepository<PHIEUTHUNO> phieuThuRepo;
        private ObservableCollection<CHITIETBAOCAOTON> AllStockReports { get; set; }
        public ObservableCollection<CHITIETBAOCAOTON> FilterStockReports { get; set; }
        private ObservableCollection<CHITIETBAOCAOCONGNO> AllDebtReports { get; set; }
        public ObservableCollection<CHITIETBAOCAOCONGNO> FilterDebtReports { get; set; }

        private bool isStockFirstLoad = true;
        private bool isDebtFirstLoad = true;

        public List<string> Months { get; set; }
        public List<string> Years { get; set; }
        private string selectedStockYear;
        public string SelectedStockYear
        {
            get { return selectedStockYear; }
            set { selectedStockYear = value; OnPropertyChanged(); }
        }

        private string selectedStockMonth;
        public string SelectedStockMonth
        {
            get { return selectedStockMonth; }
            set { selectedStockMonth = value; OnPropertyChanged(); }
        }

        private string selectedDebtYear;
        public string SelectedDebtYear
        {
            get { return selectedDebtYear; }
            set { selectedDebtYear = value; OnPropertyChanged(); }
        }

        private string selectedDebtMonth;
        public string SelectedDebtMonth
        {
            get { return selectedDebtMonth; }
            set { selectedDebtMonth = value; OnPropertyChanged(); }
        }
        private string selectedTabIndex;
        public string SelectedTabIndex
        {
            get { return selectedTabIndex; }
            set
            {
                if (selectedTabIndex != value)
                {
                    selectedTabIndex = value; OnPropertyChanged();
                }
            }
        }
        private string stockSearchBy;
        public string StockSearchBy
        {
            get { return stockSearchBy; }
            set
            {
                if (stockSearchBy != value)
                {
                    stockSearchBy = value; 
                    OnPropertyChanged();
                }
            }
        }
        
        private string stockSearchByValue;
        public string StockSearchByValue
        {
            get { return stockSearchByValue; }
            set
            {
                if (stockSearchByValue != value)
                {
                    stockSearchByValue = value;
                    OnPropertyChanged();
                }
            }
        }
        private string debtSearchByValue;
        public string DebtSearchByValue
        {
            get { return debtSearchByValue; }
            set
            {
                if (debtSearchByValue != value)
                {
                    debtSearchByValue = value;
                    OnPropertyChanged();
                }
            }
        }
        private string debtSearchBy;
        public string DebtSearchBy
        {
            get { return debtSearchBy; }
            set
            {
                if (debtSearchBy != value)
                {
                    debtSearchBy = value;
                    OnPropertyChanged();
                }
            }
        }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }
        public ChartValues<double> Values1 { get; set; }
        public ChartValues<double> Values2 { get; set; }

        #endregion


        #region Commands
        public ICommand ViewStockCommand { get; set; }
        public ICommand TabSelected { get; set; }
        public ICommand ViewDebtCommand { get; set; }
        public ICommand SearchStockCommand { get; set; }
        public ICommand SearchDebtCommand { get; set; }
        public ICommand ExportStockCommand { get; set; }
        public ICommand ExportDebtCommand { get; set; }
        #endregion

        public ReportScreenVM()
        {
            stockRepo = new GenericDataRepository<BAOCAOTON>();
            sachRepo = new GenericDataRepository<SACH>();
            khRepo = new GenericDataRepository<KHACHHANG>();
            phieuNhapRepo = new GenericDataRepository<PHIEUNHAPSACH>();
            hoaDonRepo = new GenericDataRepository<HOADON>();
            ctStockRepo = new GenericDataRepository<CHITIETBAOCAOTON>();
            debtRepo = new GenericDataRepository<BAOCAOCONGNO>();
            ctDebtRepo = new GenericDataRepository<CHITIETBAOCAOCONGNO>();
            phieuThuRepo = new GenericDataRepository<PHIEUTHUNO>();

            Months = new List<string>();
            for(int i =0; i<12; i++)
            {
                Months.Add((i + 1).ToString());
            }

            Years = new List<string> { "2022", "2023" };

            var selected=DateTime.Now.AddMonths(-1);
            SelectedStockMonth=selected.Month.ToString();
            SelectedStockYear=selected.Year.ToString();

            SelectedDebtMonth = selected.Month.ToString();
            SelectedDebtYear = selected.Year.ToString();

            Task.Run(async () =>
            {
                MainViewModel.SetLoading(true);
                SelectedTabIndex = "0";
                allSach = new List<SACH>(
                    await sachRepo.GetAllAsync(
                        s=>s.DAUSACH,
                        s => s.CHITIETPHIEUNHAPs,
                        s => s.CHITIETHOADONs,
                        s => s.CHITIETBAOCAOTONs));
                allKhachHang = new List<KHACHHANG>(
                   await khRepo.GetAllAsync(k => k.CHITIETBAOCAOCONGNOes, k => k.PHIEUTHUNOes, k => k.HOADONs));

                await ViewStock();
            }).ContinueWith((first) =>
            {
                MainViewModel.SetLoading(false);

            });

            ViewStockCommand = new RelayCommandWithNoParameter(async()=>await ViewStock());
            ViewDebtCommand=new RelayCommandWithNoParameter(async()=>await ViewDebt());
            ExportStockCommand = new RelayCommandWithNoParameter(ExportStock);
            ExportDebtCommand = new RelayCommandWithNoParameter(ExportDebt);
            TabSelected = new RelayCommand<object>((p)=>p!=null, async (p) => await ChangeTab(p));
            SearchStockCommand = new RelayCommandWithNoParameter(() =>
            {
                MainViewModel.IsLoading = true;
                SearchStock();
                MainViewModel.IsLoading = false;
            });
            SearchDebtCommand = new RelayCommandWithNoParameter(() =>
            {
                MainViewModel.IsLoading = true;
                SearchDebt();
                MainViewModel.IsLoading = false;
            });
        }
        private void SearchStock()
        {
            if(StockSearchBy == "Mã sách")
            {
                FilterStockReports = new ObservableCollection<CHITIETBAOCAOTON>(
                    AllStockReports.Where(p => p.MaSach.ToLower().Contains(StockSearchByValue.Trim().ToLower())));
            }
            else if (StockSearchBy == "Tên sách")
            {
                FilterStockReports = new ObservableCollection<CHITIETBAOCAOTON>(
                    AllStockReports.Where(p => p.SACH.DAUSACH.TenSach.ToLower().Contains(StockSearchByValue.Trim().ToLower())));
            }
            else if (StockSearchBy == "NXB")
            {
                FilterStockReports = new ObservableCollection<CHITIETBAOCAOTON>(
                    AllStockReports.Where(p => p.SACH.NhaXuatBan.ToLower().Contains(StockSearchByValue.Trim().ToLower())));
            }
        }
        private void ResetSearchStock()
        {
            StockSearchBy = "Mã sách";
            StockSearchByValue = "";
            FilterStockReports = new ObservableCollection<CHITIETBAOCAOTON>(AllStockReports);
        }
        private void SearchDebt()
        {
            if (DebtSearchBy == "Mã KH")
            {
                FilterDebtReports = new ObservableCollection<CHITIETBAOCAOCONGNO>(
                    AllDebtReports.Where(p => p.MaKhachHang.ToLower().Contains(DebtSearchByValue.Trim().ToLower())));
            }
            else if (DebtSearchBy == "Tên KH")
            {
                FilterDebtReports = new ObservableCollection<CHITIETBAOCAOCONGNO>(
                    AllDebtReports.Where(p => p.KHACHHANG.TenKhachHang.ToLower().Contains(DebtSearchByValue.Trim().ToLower())));
            }
        }
        private void ResetSearchDebt()
        {
            DebtSearchBy = "Mã KH";
            DebtSearchByValue = "";
            FilterDebtReports = new ObservableCollection<CHITIETBAOCAOCONGNO>(AllDebtReports);
        }
        private void ExportDebt()
        {
            MainViewModel.IsLoading = true;
            string filePath = "";
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel Sheet(*.xlsx)|*.xlsx|All Files(*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                filePath = saveFileDialog.FileName;
            }

            if (string.IsNullOrEmpty(filePath))
            {
                ConfirmDialog notification = new ConfirmDialog()
                {
                    Header = "Lỗi",
                    ContentString = "Xuất file không thành công."
                };
                MainViewModel.SetLoading(false);
                DialogHost.Show(notification, "Main");
            }
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            try
            {
                using (ExcelPackage p = new ExcelPackage())
                {
                    p.Workbook.Properties.Title = "Báo cáo công nợ";
                    p.Workbook.Worksheets.Add("Test sheet");

                    var ws = p.Workbook.Worksheets[0];
                    ws.Name = $"Debt report {SelectedDebtMonth}/{SelectedDebtYear}";
                    ws.Cells.Style.Font.SetFromFont("Times New Roman", 13);

                    string[] columnHeaders =
                        { "Mã KH", "Tên khách hàng", "Nợ đầu", "Phát sinh", "Lượng thu", "Nợ cuối" };

                    var columnCount = columnHeaders.Count();
                    ws.Cells[1, 1].Value = $"Báo cáo công nợ tháng {SelectedDebtMonth}/{SelectedDebtYear}";
                    ws.Cells[1, 1, 1, columnCount].Merge = true;
                    ws.Cells[1, 1, 1, columnCount].Style.Font.Bold = true;
                    ws.Cells[1, 1, 1, columnCount / 2]
                        .Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    int colIndex = 1;
                    int rowIndex = 2;
                    foreach (var item in columnHeaders)
                    {
                        var cell = ws.Cells[rowIndex, colIndex];
                        var fill = cell.Style.Fill;
                        fill.PatternType = ExcelFillStyle.Solid;
                        fill.BackgroundColor.SetColor(Color.FromArgb(218, 227, 227));

                        var border = cell.Style.Border;
                        border.Bottom.Style = border.Top.Style
                            = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;
                        cell.Value = item;
                        colIndex++;
                    }

                    foreach (var report in AllDebtReports)
                    {
                        colIndex = 1;
                        rowIndex++;

                        ws.Cells[rowIndex, colIndex++].Value = report.MaKhachHang;
                        ws.Cells[rowIndex, colIndex++].Value = report.KHACHHANG.TenKhachHang;
                        ws.Cells[rowIndex, colIndex++].Value = report.NoDau;
                        ws.Cells[rowIndex, colIndex++].Value = report.PhatSinh;
                        ws.Cells[rowIndex, colIndex++].Value = report.NoDau + report.PhatSinh - report.NoCuoi;
                        ws.Cells[rowIndex, colIndex++].Value = report.NoCuoi;
                    }

                    using (ExcelRange excelRange = ws.Cells[$"A1:F{rowIndex}"])
                    {
                        excelRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        excelRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        excelRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        excelRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }

                    ws.Cells.AutoFitColumns();
                    for (int i = 1; i <= 6; i++)
                    {
                        ws.Column(i).Width *= 1.2;
                    }

                    Byte[] bin = p.GetAsByteArray();
                    File.WriteAllBytes(filePath, bin);
                }

                ConfirmDialog notification = new ConfirmDialog()
                {
                    Header = "Thông báo",
                    ContentString = "Xuất file thành công."
                };
                MainViewModel.SetLoading(false);
                DialogHost.Show(notification, "Main");
            }
            catch
            {
                ConfirmDialog notification = new ConfirmDialog()
                {
                    Header = "Lỗi",
                    ContentString = "Có lỗi khi lưu file."
                };
                MainViewModel.SetLoading(false);
                DialogHost.Show(notification, "Main");
            }
        }

        private void ExportStock()
        {
            MainViewModel.IsLoading = true;
            string filePath = "";
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel Sheet(*.xlsx)|*.xlsx|All Files(*.*)|*.*";
            if (saveFileDialog.ShowDialog()==true)
            {
                filePath=saveFileDialog.FileName;
            }

            if (string.IsNullOrEmpty(filePath))
            {
                ConfirmDialog notification = new ConfirmDialog()
                {
                    Header = "Lỗi",
                    ContentString = "Xuất file không thành công."
                };
                MainViewModel.SetLoading(false);
                DialogHost.Show(notification, "Main");
            }
            try
            {
                using (ExcelPackage p =new ExcelPackage())
                {
                    p.Workbook.Properties.Title = "Báo cáo tồn";
                    p.Workbook.Worksheets.Add("Test sheet");

                    var ws=p.Workbook.Worksheets[0];
                    ws.Name = $"Stock report {SelectedStockMonth}/{SelectedStockYear}";
                    ws.Cells.Style.Font.SetFromFont("Times New Roman", 13);

                    string[] columnHeaders = 
                        { "Mã sách", "Tên sách", "Tồn đầu", "Nhập", "Xuất", "Tồn cuối" };

                    var columnCount = columnHeaders.Count();
                    ws.Cells[1, 1].Value = $"Báo cáo tồn tháng {SelectedStockMonth}/{SelectedStockYear}";
                    ws.Cells[1, 1, 1, columnCount].Merge = true;
                    ws.Cells[1, 1, 1, columnCount].Style.Font.Bold = true;
                    ws.Cells[1, 1, 1, columnCount / 2]
                        .Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    int colIndex = 1;
                    int rowIndex = 2;
                    foreach(var item in columnHeaders)
                    {
                        var cell =ws.Cells[rowIndex, colIndex];
                        var fill = cell.Style.Fill;
                        fill.PatternType = ExcelFillStyle.Solid;
                        fill.BackgroundColor.SetColor(Color.FromArgb(218, 227, 227));

                        var border=cell.Style.Border;
                        border.Bottom.Style = border.Top.Style
                            = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;
                        cell.Value = item;
                        colIndex++;
                    }

                    foreach(var report in AllStockReports)
                    {
                        colIndex = 1;
                        rowIndex++;

                        ws.Cells[rowIndex, colIndex++].Value = report.MaSach;
                        ws.Cells[rowIndex, colIndex++].Value = report.SACH.DAUSACH.TenSach;
                        ws.Cells[rowIndex, colIndex++].Value = report.TonDau.ToString();
                        ws.Cells[rowIndex, colIndex++].Value=report.PhatSinh.ToString();
                        var converter = new StockConverter();
                        ws.Cells[rowIndex, colIndex++].Value = converter.Convert(report, null, null, null);
                        ws.Cells[rowIndex, colIndex++].Value = report.TonCuoi.ToString();
                    }

                    using (ExcelRange excelRange = ws.Cells[$"A1:F{rowIndex}"])
                    {
                        excelRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        excelRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        excelRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        excelRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }

                    ws.Cells.AutoFitColumns();
                    for (int i = 1; i <= 6; i++)
                    {
                        ws.Column(i).Width *= 1.2;
                    }

                    Byte[] bin = p.GetAsByteArray();
                    File.WriteAllBytes(filePath, bin);
                }

                ConfirmDialog notification = new ConfirmDialog()
                {
                    Header = "Thông báo",
                    ContentString = "Xuất file thành công."
                };
                MainViewModel.SetLoading(false);
                DialogHost.Show(notification, "Main");
            }
            catch
            {
                ConfirmDialog notification = new ConfirmDialog()
                {
                    Header = "Lỗi",
                    ContentString = "Có lỗi khi lưu file."
                };
                MainViewModel.SetLoading(false);
                DialogHost.Show(notification, "Main");
            }
        }

        private async Task ChangeTab(object p)
        {
            var e = p as SelectionChangedEventArgs;
            if (e != null && e.OriginalSource as System.Windows.Controls.TabControl!=null)
            {
                if ((e.OriginalSource as System.Windows.Controls.TabControl).SelectedIndex < 0)
                    return;
                if (SelectedTabIndex == "0")
                {
                    if (isStockFirstLoad)
                    {
                        await ViewStock();
                        isStockFirstLoad = false;
                    }
                    ResetSearchStock();

                }
                else if (SelectedTabIndex == "1")
                {
                    if (isDebtFirstLoad)
                    {
                        await ViewDebt();
                        isDebtFirstLoad = false;
                    }
                    ResetSearchDebt();

                }
            }
        }


        #region Methods
        private async Task ViewDebt()
        {
            MainViewModel.SetLoading(true);
            var now = DateTime.Now;

            if (string.IsNullOrEmpty(SelectedDebtMonth) || string.IsNullOrEmpty(SelectedDebtYear))
            {
                ConfirmDialog notification = new ConfirmDialog()
                {
                    Header = "Error",
                    ContentString = "Giá trị tháng hoặc năm được chọn không hợp lệ."
                };
                MainViewModel.SetLoading(false);
                await DialogHost.Show(notification, "Main");
            }
            else
            {
                var selectedMonth = int.Parse(SelectedDebtMonth);
                var selectedYear = int.Parse(SelectedDebtYear);
                if (selectedYear > now.Year
                || selectedYear == now.Year && selectedMonth > now.Month)
                {
                    ConfirmDialog notification = new ConfirmDialog()
                    {
                        Header = "Không hợp lệ",
                        ContentString = "Chưa đến thời điểm lập báo cáo với tháng và năm được chọn."
                    };
                    MainViewModel.SetLoading(false);
                    await DialogHost.Show(notification, "Main");
                }
                else if (selectedMonth == now.Month && selectedYear == now.Year)
                {
                    var allReports = new List<BAOCAOCONGNO>(await debtRepo.GetAllAsync(s => s.CHITIETBAOCAOCONGNOes));
                    ConfirmDialog notification = new ConfirmDialog()
                    {
                        Header = "Lưu ý",
                        ContentString = "Báo cáo hiển thị là bản tạm thời tính đến thời điểm hiện tại."
                    };
                    MainViewModel.SetLoading(false);
                    await DialogHost.Show(notification, "Main");
                    AllDebtReports = new ObservableCollection<CHITIETBAOCAOCONGNO>((
                        await CalculateDebtWithoutSave(allReports, selectedMonth, selectedYear))
                        .OrderBy(p => p.KHACHHANG.TenKhachHang));
                    ResetSearchDebt();
                }
                else
                {
                    var allReports = new List<BAOCAOCONGNO>(await debtRepo.GetAllAsync(s => s.CHITIETBAOCAOCONGNOes));
                    var baocao = await getDebtReport(allReports, selectedMonth, selectedYear);

                    if (baocao == null || baocao.CHITIETBAOCAOCONGNOes == null || string.IsNullOrEmpty(baocao.MaBaoCaoCongNo))
                    {
                        ConfirmDialog notification = new ConfirmDialog()
                        {
                            Header = "Không hợp lệ",
                            ContentString = "Không có báo cáo vào tháng và năm được chọn"
                        };
                        MainViewModel.SetLoading(false);
                        await DialogHost.Show(notification, "Main");
                    }
                    AllDebtReports = new ObservableCollection<CHITIETBAOCAOCONGNO>((
                        await ctDebtRepo.GetListAsync(ct => ct.MaBaoCaoCongNo == baocao.MaBaoCaoCongNo, ct => ct.KHACHHANG))
                        .OrderBy(p=>p.KHACHHANG.TenKhachHang));
                    ResetSearchDebt();

                }
            }
            MainViewModel.SetLoading(false);
        }
        private async Task ViewStock()
        {
            MainViewModel.SetLoading(true);
            var now = DateTime.Now;

            if (string.IsNullOrEmpty(SelectedStockMonth) || string.IsNullOrEmpty(SelectedStockYear))
            {
                ConfirmDialog notification = new ConfirmDialog()
                {
                    Header = "Error",
                    ContentString = "Giá trị tháng hoặc năm được chọn không hợp lệ."
                };
                MainViewModel.SetLoading(false);
                await DialogHost.Show(notification, "Main");
            }
            else
            {
                var selectedMonth = int.Parse(SelectedStockMonth);
                var selectedYear = int.Parse(SelectedStockYear);
                if (selectedYear > now.Year
                || selectedYear == now.Year && selectedMonth > now.Month)
                {
                    ConfirmDialog notification = new ConfirmDialog()
                    {
                        Header = "Không hợp lệ",
                        ContentString = "Chưa đến thời điểm lập báo cáo với tháng và năm được chọn."
                    };
                    MainViewModel.SetLoading(false);
                    await DialogHost.Show(notification, "Main");
                }
                else if (selectedMonth == now.Month && selectedYear == now.Year)
                {
                    var allReports = new List<BAOCAOTON>(await stockRepo.GetAllAsync(s => s.CHITIETBAOCAOTONs));
                    ConfirmDialog notification = new ConfirmDialog()
                    {
                        Header = "Lưu ý",
                        ContentString = "Báo cáo hiển thị là bản tạm thời tính đến thời điểm hiện tại."
                    };
                    MainViewModel.SetLoading(false);
                    await DialogHost.Show(notification, "Main");
                    AllStockReports = new ObservableCollection<CHITIETBAOCAOTON>((
                        await CalculateWithoutSave(allReports, selectedMonth, selectedYear)).
                        OrderBy(p => p.SACH.DAUSACH.TenSach).ThenBy(p => p.SACH.NhaXuatBan));
                    ResetSearchStock();
                }
                else
                {
                    var allReports = new List<BAOCAOTON>(await stockRepo.GetAllAsync(s => s.CHITIETBAOCAOTONs));
                    var baocao = await getReport(allReports, selectedMonth, selectedYear);

                    if (baocao == null || baocao.CHITIETBAOCAOTONs == null || string.IsNullOrEmpty(baocao.MaBaoCaoTon))
                    {
                        ConfirmDialog notification = new ConfirmDialog()
                        {
                            Header = "Không hợp lệ",
                            ContentString = "Không có báo cáo vào tháng và năm được chọn"
                        };
                        MainViewModel.SetLoading(false);
                        await DialogHost.Show(notification, "Main");
                    }
                    AllStockReports = new ObservableCollection<CHITIETBAOCAOTON>((
                        await ctStockRepo.GetListAsync(ct => ct.MaBaoCaoTon == baocao.MaBaoCaoTon, ct => ct.SACH, ct => ct.SACH.DAUSACH)).
                        OrderBy(p=>p.SACH.DAUSACH.TenSach).ThenBy(p=>p.SACH.NhaXuatBan));
                    ResetSearchStock();
                }
            }
            MainViewModel.SetLoading(false);

        }

        private async Task<BAOCAOTON> getReport(List<BAOCAOTON> allReport, int selectedMonth, int selectedYear)
        {
            var selectedReport = allReport.Where(s => s.Thang.Date.Month == selectedMonth
                                         && s.Thang.Date.Year == selectedYear).ToList();

            if (selectedReport == null||selectedReport.Count<=0)
            {
                if (allPhieuNhap == null)
                {
                    allPhieuNhap = new List<PHIEUNHAPSACH>(await phieuNhapRepo.GetAllAsync());
                }
                if (allPhieuNhap.Count > 0)
                {
                    earliestPhieuNhap = allPhieuNhap[0].NgayNhap.Value;
                    foreach (var p in allPhieuNhap)
                    {
                        if (p.NgayNhap.Value < earliestPhieuNhap)
                        {
                            earliestPhieuNhap = p.NgayNhap.Value;
                        }
                    }
                }
                else
                    return new BAOCAOTON();
                if (allHoaDon == null)
                {
                    allHoaDon = new List<HOADON>(await hoaDonRepo.GetAllAsync());
                }
                var date = new DateTime(selectedYear, selectedMonth, DateTime.DaysInMonth(selectedYear, selectedMonth));

                if (date.Year < earliestPhieuNhap.Date.Year
                    || date.Year == earliestPhieuNhap.Date.Year && date.Month < earliestPhieuNhap.Date.Month)
                {
                    return new BAOCAOTON();
                }
                date = date.AddMonths(-1);
                BAOCAOTON previousReport;
                if (date.Year<earliestPhieuNhap.Date.Year 
                    ||date.Year==earliestPhieuNhap.Date.Year&&date.Month<earliestPhieuNhap.Date.Month)
                {

                    previousReport = new BAOCAOTON();
                    previousReport.CHITIETBAOCAOTONs = new List<CHITIETBAOCAOTON>();
                    previousReport.Thang = date;
                }
                else
                    previousReport = await getReport(allReport, date.Month, date.Year);
                return await CalculateStockReport(previousReport);
            }
            else if (selectedReport != null&&selectedReport.Count>0)
            {
                return selectedReport[0];
            }

            return new BAOCAOTON();
    }
        private async Task<BAOCAOCONGNO> getDebtReport(List<BAOCAOCONGNO> allReport, int selectedMonth, int selectedYear)
        {
            var selectedReport = allReport.Where(s => s.Thang.Date.Month == selectedMonth
                                         && s.Thang.Date.Year == selectedYear).ToList();

            if (selectedReport == null || selectedReport.Count <= 0)
            {
                if (allPhieuThu == null)
                {
                    allPhieuThu = new List<PHIEUTHUNO>(await phieuThuRepo.GetAllAsync());
                }
                if (allHoaDon == null)
                {
                    allHoaDon = new List<HOADON>(await hoaDonRepo.GetAllAsync());
                }
                if (allHoaDon.Count > 0)
                {
                    foreach (var p in allHoaDon)
                    {
                        if (p.NgayLapHoaDon < earliestHoaDon && p.SoTienNo>0)
                        {
                            earliestHoaDon = p.NgayLapHoaDon;
                        }
                    }
                }
                else
                    return new BAOCAOCONGNO();
                var date = new DateTime(selectedYear, selectedMonth, DateTime.DaysInMonth(selectedYear, selectedMonth));

                if (date.Year < earliestHoaDon.Date.Year
                    || date.Year == earliestHoaDon.Date.Year && date.Month < earliestHoaDon.Date.Month)
                {
                    return new BAOCAOCONGNO();
                }
                date = date.AddMonths(-1);
                BAOCAOCONGNO previousReport;
                if (date.Year < earliestHoaDon.Date.Year
                    || date.Year == earliestHoaDon.Date.Year && date.Month < earliestHoaDon.Date.Month)
                {
                    previousReport = new BAOCAOCONGNO();
                    previousReport.CHITIETBAOCAOCONGNOes = new List<CHITIETBAOCAOCONGNO>();
                    previousReport.Thang = date;
                }
                else
                    previousReport = await getDebtReport(allReport, date.Month, date.Year);
                return await CalculateDebtReport(previousReport);
            }
            else if (selectedReport != null && selectedReport.Count > 0)
            {
                return selectedReport[0];
            }

            return new BAOCAOCONGNO();
        }
        private async Task<BAOCAOTON> CalculateStockReport(BAOCAOTON previous)
        {
            var current = previous.Thang.AddMonths(1);
            var now = DateTime.Now;
            if (allPhieuNhap == null)
            {
                allPhieuNhap = new List<PHIEUNHAPSACH>(await phieuNhapRepo.GetAllAsync());
            }
            if (allHoaDon == null)
            {
                allHoaDon = new List<HOADON>(await hoaDonRepo.GetAllAsync());
            }
            var report = new BAOCAOTON();
            report.MaBaoCaoTon = await GenerateId.Gen(typeof(BAOCAOTON), "MaBaoCaoTon");
            report.Thang = new DateTime(current.Year, current.Month, DateTime.DaysInMonth(current.Year, current.Month));

            await stockRepo.Add(report);

            var phieuNhaps = allPhieuNhap.Where(s => s.NgayNhap.Value.Month == current.Date.Month
                                                && s.NgayNhap.Value.Year == current.Date.Year).ToList();
            var hoadons = allHoaDon.Where(s => s.NgayLapHoaDon.Month == current.Date.Month
                                    && s.NgayLapHoaDon.Year == current.Date.Year).ToList();

            for (int i=0; i<allSach.Count; i++)
            {
                int slNhap = 0;
                int slXuat = 0;
                if (allSach[i].CHITIETPHIEUNHAPs != null)
                {
                    slNhap = allSach[i].CHITIETPHIEUNHAPs
                        .Where(s => phieuNhaps.Any(pn => pn.MaPhieuNhap == s.MaPhieuNhap))
                        .Sum(s => s.SoLuong);
                }
                if (allSach[i].CHITIETHOADONs != null)
                {
                    slXuat = allSach[i].CHITIETHOADONs
                        .Where(s => hoadons.Any(hd => hd.MaHoaDon == s.MaHoaDon))
                        .Sum(item => item.SoLuong);
                }

                var ct = new CHITIETBAOCAOTON();
                ct.MaSach = allSach[i].MaSach;
                ct.MaBaoCaoTon = report.MaBaoCaoTon;

                var ctSach = await ctStockRepo.GetSingleAsync(
                    bc => bc.MaSach == ct.MaSach&&bc.MaBaoCaoTon==previous.MaBaoCaoTon);
                if (ctSach!=null)
                {
                    ct.TonDau = ctSach.TonCuoi;
                }
                else
                {
                    ct.TonDau = 0;
                }
                ct.PhatSinh = slNhap;
                ct.TonCuoi = ct.TonDau + ct.PhatSinh - slXuat;
                allSach[i].CHITIETBAOCAOTONs.Add(ct);
                await ctStockRepo.Add(ct);
                await sachRepo.Update(allSach[i]);
            }
            return report;
        }

        private async Task<BAOCAOCONGNO> CalculateDebtReport(BAOCAOCONGNO previous)
        {
            var current = previous.Thang.AddMonths(1);
            var now = DateTime.Now;
            if (allPhieuThu == null)
            {
                allPhieuThu = new List<PHIEUTHUNO>(await phieuThuRepo.GetAllAsync());
            }
            if (allHoaDon == null)
            {
                allHoaDon = new List<HOADON>(await hoaDonRepo.GetAllAsync());
            }
            var report = new BAOCAOCONGNO();
            report.MaBaoCaoCongNo = await GenerateId.Gen(typeof(BAOCAOCONGNO), "MaBaoCaoCongNo");
            report.Thang = new DateTime(current.Year, current.Month, DateTime.DaysInMonth(current.Year, current.Month));

            await debtRepo.Add(report);

            var phieuThus = allPhieuThu.Where(s => s.NgayThu.Month == current.Date.Month
                                                && s.NgayThu.Year == current.Date.Year).ToList();
            var hoadons = allHoaDon.Where(s => s.NgayLapHoaDon.Month == current.Date.Month
                                    && s.NgayLapHoaDon.Year == current.Date.Year && s.SoTienNo>0).ToList();

            for (int i = 0; i < allKhachHang.Count; i++)
            {
                decimal tienThu = 0;
                decimal tienNoThem = 0;
                if (allKhachHang[i].PHIEUTHUNOes != null)
                {
                    tienThu = allKhachHang[i].PHIEUTHUNOes
                        .Where(s => phieuThus.Any(pn => pn.MaPhieuThu == s.MaPhieuThu))
                        .Sum(s => s.SoTienThu)??0;
                }
                if (allKhachHang[i].HOADONs != null)
                {
                    tienNoThem = allKhachHang[i].HOADONs
                        .Where(s => hoadons.Any(hd => hd.MaHoaDon == s.MaHoaDon))
                        .Sum(item => item.SoTienNo);
                }

                var ct = new CHITIETBAOCAOCONGNO();
                ct.MaKhachHang = allKhachHang[i].MaKhachHang;
                ct.MaBaoCaoCongNo = report.MaBaoCaoCongNo;

                var ctDebt = await ctDebtRepo.GetSingleAsync(
                    bc => bc.MaKhachHang == ct.MaKhachHang && bc.MaBaoCaoCongNo == previous.MaBaoCaoCongNo);
                if (ctDebt != null)
                {
                    ct.NoDau = ctDebt.NoCuoi;
                }
                else
                {
                    ct.NoDau = 0;
                }
                ct.PhatSinh = tienNoThem;
                ct.NoCuoi = ct.NoDau + ct.PhatSinh - tienThu;
                allKhachHang[i].CHITIETBAOCAOCONGNOes.Add(ct);
                await ctDebtRepo.Add(ct);
                await khRepo.Update(allKhachHang[i]);
            }
            return report;
        }

        private async Task<List<CHITIETBAOCAOTON>> CalculateWithoutSave(List<BAOCAOTON> allReport,int selectedMonth, int selectedYear)
        {
            List<CHITIETBAOCAOTON> reports = new List<CHITIETBAOCAOTON>();
            if (allPhieuNhap == null)
            {
                allPhieuNhap = new List<PHIEUNHAPSACH>(await phieuNhapRepo.GetAllAsync());
            }
            if (allHoaDon == null)
            {
                allHoaDon = new List<HOADON>(await hoaDonRepo.GetAllAsync());
            }
            var current = new DateTime(selectedYear, selectedMonth,
                DateTime.DaysInMonth(selectedYear, selectedMonth));
            var phieuNhaps = allPhieuNhap.Where(s => s.NgayNhap.Value.Month == current.Date.Month
                                    && s.NgayNhap.Value.Year == current.Date.Year).ToList();
            var hoadons = allHoaDon.Where(s => s.NgayLapHoaDon.Month == current.Date.Month
                                    && s.NgayLapHoaDon.Year == current.Date.Year).ToList();

            current = current.AddMonths(-1);
            var previousReport = await getReport(allReport, current.Month, current.Year);

            for (int i = 0; i < allSach.Count; i++)
            {
                int slNhap = 0;
                int slXuat = 0;
                if (allSach[i].CHITIETPHIEUNHAPs != null)
                {
                    slNhap = allSach[i].CHITIETPHIEUNHAPs
                        .Where(s => phieuNhaps.Any(pn => pn.MaPhieuNhap == s.MaPhieuNhap))
                        .Sum(s => s.SoLuong);
                }
                if (allSach[i].CHITIETHOADONs != null)
                {
                    slXuat = allSach[i].CHITIETHOADONs
                        .Where(s => hoadons.Any(hd => hd.MaHoaDon == s.MaHoaDon))
                        .Sum(item => item.SoLuong);
                }

                var ct = new CHITIETBAOCAOTON();
                ct.MaSach = allSach[i].MaSach;

                var ctSach = await ctStockRepo.GetSingleAsync(
                    bc => bc.MaSach == ct.MaSach && bc.MaBaoCaoTon == previousReport.MaBaoCaoTon);
                if (ctSach != null)
                {
                    ct.TonDau = ctSach.TonCuoi;
                }
                else
                {
                    ct.TonDau = 0;
                }
                ct.PhatSinh = slNhap;
                ct.TonCuoi = ct.TonDau + ct.PhatSinh - slXuat;
                ct.SACH = allSach[i];
                reports.Add(ct);
            }
            return reports;
        }
        private async Task<List<CHITIETBAOCAOCONGNO>> CalculateDebtWithoutSave(List<BAOCAOCONGNO> allReport, int selectedMonth, int selectedYear)
        {
            List<CHITIETBAOCAOCONGNO> reports = new List<CHITIETBAOCAOCONGNO>();
            if (allPhieuThu == null)
            {
                allPhieuThu = new List<PHIEUTHUNO>(await phieuThuRepo.GetAllAsync());
            }
            if (allHoaDon == null)
            {
                allHoaDon = new List<HOADON>(await hoaDonRepo.GetAllAsync());
            }
            var current = new DateTime(selectedYear, selectedMonth,
                DateTime.DaysInMonth(selectedYear, selectedMonth));
            var phieuThus = allPhieuThu.Where(s => s.NgayThu.Month == current.Date.Month
                                                && s.NgayThu.Year == current.Date.Year).ToList();
            var hoadons = allHoaDon.Where(s => s.NgayLapHoaDon.Month == current.Date.Month
                                    && s.NgayLapHoaDon.Year == current.Date.Year && s.SoTienNo > 0).ToList();

            current = current.AddMonths(-1);
            var previousReport = await getDebtReport(allReport, current.Month, current.Year);

            for (int i = 0; i < allKhachHang.Count; i++)
            {
                decimal tienThu = 0;
                decimal tienNoThem = 0;
                if (allKhachHang[i].PHIEUTHUNOes != null)
                {
                    tienThu = allKhachHang[i].PHIEUTHUNOes
                        .Where(s => phieuThus.Any(pn => pn.MaPhieuThu == s.MaPhieuThu))
                        .Sum(s => s.SoTienThu) ?? 0;
                }
                if (allKhachHang[i].HOADONs != null)
                {
                    tienNoThem = allKhachHang[i].HOADONs
                        .Where(s => hoadons.Any(hd => hd.MaHoaDon == s.MaHoaDon))
                        .Sum(item => item.SoTienNo);
                }

                var ct = new CHITIETBAOCAOCONGNO();
                ct.MaKhachHang = allKhachHang[i].MaKhachHang;

                var ctDebt = await ctDebtRepo.GetSingleAsync(
                    bc => bc.MaKhachHang == ct.MaKhachHang && bc.MaBaoCaoCongNo == previousReport.MaBaoCaoCongNo);
                if (ctDebt != null)
                {
                    ct.NoDau = ctDebt.NoCuoi;
                }
                else
                {
                    ct.NoDau = 0;
                }
                ct.PhatSinh = tienNoThem;
                ct.NoCuoi = ct.NoDau + ct.PhatSinh - tienThu;
                ct.KHACHHANG = allKhachHang[i];
                reports.Add(ct);
            }
            return reports;
        }

        #endregion
    }
}
