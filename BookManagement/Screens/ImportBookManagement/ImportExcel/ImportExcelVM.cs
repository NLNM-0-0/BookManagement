using BookManagement.Models;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using System.Drawing;
using System.Windows.Forms;
using System.IO.Packaging;
using System.Reflection;
using OfficeOpenXml.FormulaParsing.ExpressionGraph.FunctionCompilers;
using DocumentFormat.OpenXml.Office2016.Drawing.Command;

namespace BookManagement
{
    public class ImportExcelVM:BaseViewModel
    {
        #region Action
        public Action ImportSuccess { get;set; }
        #endregion 
        #region GenericDataRepository
        private GenericDataRepository<DAUSACH> bookHeaderRepo = new GenericDataRepository<DAUSACH>();
        private GenericDataRepository<THELOAI> categoryRepo = new GenericDataRepository<THELOAI>();
        private GenericDataRepository<TACGIA> authorRepo = new GenericDataRepository<TACGIA>();
        private GenericDataRepository<SACH> bookRepo = new GenericDataRepository<SACH>();
        private GenericDataRepository<PHIEUNHAPSACH> importRepo = new GenericDataRepository<PHIEUNHAPSACH>();
        #endregion

        #region Command
        public ICommand DowloadTemplateCommand { get; set; }
        public ICommand UploadExcelCommand { get; set; }
        private System.Windows.Controls.UserControl PreviousItem;
        #endregion

        #region Properties
        private ICollection<DAUSACH> bookHeaders;
        private ICollection<THELOAI> categories;
        private ICollection<TACGIA> authors;
        private List<ImportExcelObject> importExcelObjects = new List<ImportExcelObject>();
        #endregion
        public ImportExcelVM()
        {
            Task.Run(async () =>
            {
                MainViewModel.IsLoading = true;
                await Load();
                DowloadTemplateCommand = new RelayCommandWithNoParameter(async () =>
                {
                    await DowloadTemplate();
                });
                UploadExcelCommand = new RelayCommandWithNoParameter(async () =>
                {
                    MainViewModel.IsLoading = true;
                    OpenFileDialog openDialog = new OpenFileDialog();
                    openDialog.Title = "Select file";
                    openDialog.InitialDirectory = @"c:\";
                    openDialog.Filter = "Excel Sheet(*.xlsx)|*.xlsx|All Files(*.*)|*.*";
                    openDialog.FilterIndex = 1;
                    openDialog.RestoreDirectory = true;
                    if (openDialog.ShowDialog() == DialogResult.OK)
                    {
                        if (openDialog.FileName != "")
                        {
                            FileInfo fileInfo = new FileInfo(openDialog.FileName);
                            int isAvailable = await CheckGetDataFromExcel(fileInfo);
                            PreviousItem = MainViewModel.UpdateDialog("Main");
                            if (isAvailable == -1)
                            {
                                var dl = new ConfirmDialog()
                                {
                                    Header = "Lỗi",
                                    ContentString = "File bạn vừa nhập đang được mở hoặc không tồn tại. Xin hãy sửa lại.",
                                };
                                MainViewModel.IsLoading = false;
                                await DialogHost.Show(dl, "Main");
                                return;
                            }
                            else if (isAvailable == 0)
                            {
                                var dl = new ConfirmDialog()
                                {
                                    Header = "Lỗi",
                                    ContentString = "File bạn vừa nhập không đúng dạng dữ liệu tiêu chuẩn. Xin hãy xem lại qui định phía trên file và kiểm tra lại.",
                                };
                                MainViewModel.IsLoading = false;
                                await DialogHost.Show(dl, "Main");
                                return;
                            }
                            else if (isAvailable == 1)
                            {
                                bool isDataMatchRule = await CheckRule();
                                if (!isDataMatchRule)
                                {
                                    var dl = new ConfirmDialog()
                                    {
                                        Header = "Lỗi",
                                        ContentString = "Dữ liệu vừa được nhập vào không khớp với qui định hãy kiểm tra lại.",
                                    };
                                    MainViewModel.IsLoading = false;
                                    await DialogHost.Show(dl, "Main");
                                    return;
                                }
                                else
                                {
                                    bool isDataSuitable = CheckData();
                                    if(isDataSuitable)
                                    {
                                        await SaveImport();
                                        MainViewModel.IsLoading = false;
                                        DialogHost.CloseDialogCommand.Execute(null, null);
                                        ImportSuccess?.Invoke();
                                        return;
                                    }    
                                    else
                                    {
                                        var dl = new ConfirmDialog()
                                        {
                                            Header = "Lỗi",
                                            ContentString = "Dữ liệu vừa được nhập vào không khớp với những qui định được nêu trên. Xin hãy tải lại bản mẫu và xem lại qui định phía trên file.",
                                        };
                                        MainViewModel.IsLoading = false;
                                        await DialogHost.Show(dl, "Main");
                                        return;
                                    }    
                                }   
                            }    
                        }
                        else
                        {
                            var dl = new ConfirmDialog()
                            {
                                Header = "Lỗi",
                                ContentString = "Hãy chọn file excel."
                            };
                            MainViewModel.IsLoading = false;
                            await DialogHost.Show(dl, "Main");
                            return;
                        }
                    }
                    MainViewModel.IsLoading = false;
                    return;
                });
                MainViewModel.IsLoading = false;
            });
        }
        private async Task Load()
        {
            bookHeaders = await bookHeaderRepo.GetAllAsync(p=>p.SACHes, p=>p.THELOAI, p=>p.TACGIAs);
            categories = await categoryRepo.GetAllAsync();
            authors = await authorRepo.GetAllAsync();
        }
        private async Task<bool> CheckRule()
        {
            int minImportBook = RuleStore.instance.getValue(AppEnum.LuongSachNhapItNhat);
            int maxImportStock = RuleStore.instance.getValue(AppEnum.LuongTonToiDaKhiNhap);
            for (int i = 0; i < importExcelObjects.Count; i++)
            {
                ImportExcelObject importExcelObject = importExcelObjects[i];
                if(importExcelObject.Amount < minImportBook)
                {
                    return false;
                }    
                SACH book = await bookRepo.GetSingleAsync(p => p.DAUSACH.TenSach == importExcelObject.BookName && p.NhaXuatBan == importExcelObject.NXB);
                if (book != null) 
                {
                    if(book.SoLuong >= maxImportStock) 
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        private bool CheckData()
        {
            for (int i = 0; i < importExcelObjects.Count; i++)
            {
                ImportExcelObject firstImportExcelObject = importExcelObjects[i];
                List<String> authorNames = firstImportExcelObject.Authors;
                for(int j = i + 1; j < importExcelObjects.Count; j++)
                {
                    ImportExcelObject secondImportExcelObect = importExcelObjects[j];
                    List<String> tempAuthorNames = secondImportExcelObect.Authors;
                    if (firstImportExcelObject.BookName == secondImportExcelObect.BookName &&
                        authorNames.Count() == tempAuthorNames.Count() &&
                        authorNames.Intersect(tempAuthorNames).ToList().Count() == authorNames.Count())
                    {
                        if(firstImportExcelObject.TheLoai != secondImportExcelObect.TheLoai)
                        {
                            return false;
                        }      
                        if(firstImportExcelObject.NXB == secondImportExcelObect.NXB &&
                            firstImportExcelObject.TimeRepublish == secondImportExcelObect.TimeRepublish)
                        {
                            return false;
                        }    
                    }    
                }    
            }
            return true;
        }
        private async Task SaveImport()
        {
            //import page handle
            PHIEUNHAPSACH import = new PHIEUNHAPSACH();
            import.MaPhieuNhap = await GenerateId.Gen(typeof(PHIEUNHAPSACH), "MaPhieuNhap");
            import.NgayNhap = DateTime.Now;
            import.MaNhanVien = AccountStore.instance.CurrentAccount.MaNhanVien;
            import.TongTien = importExcelObjects.Sum(p=>p.UnitPrice*p.Amount);
            await importRepo.Add(import);

            for (int i = 0; i < importExcelObjects.Count; i++)
            {
                ImportExcelObject importExcelObject = importExcelObjects[i];
                List<String> authorNames = importExcelObject.Authors;
                SACH book = await bookRepo.GetSingleAsync(p => {
                    if(p.DAUSACH.TenSach!=importExcelObject.BookName)
                    {
                        return false;
                    }    
                    if(p.NhaXuatBan != importExcelObject.NXB)
                    {
                        return false;
                    }    
                    if(p.LanTaiBan != importExcelObject.TimeRepublish)
                    {
                        return false;
                    }
                    List<String> tempAuthorNames = p.DAUSACH.TACGIAs.Select(a => a.TenTacGia).ToList();
                    if(tempAuthorNames.Count != authorNames.Count)
                    {
                        return false;
                    }    
                    return tempAuthorNames.Intersect(authorNames).ToList().Count() == authorNames.Count();
                });
                if(book == null)
                {
                    book = new SACH();

                    //book header handle
                    DAUSACH bookHeader = await bookHeaderRepo.GetSingleAsync(p =>
                    {
                        if (p.TenSach != importExcelObject.BookName)
                        {
                            return false;
                        }
                        List<String> tempAuthorNames = p.TACGIAs.Select(a => a.TenTacGia).ToList();
                        if (tempAuthorNames.Count != authorNames.Count)
                        {
                            return false;
                        }
                        return tempAuthorNames.Intersect(authorNames).ToList().Count() == authorNames.Count();
                    });
                    if (bookHeader == null)
                    {
                        bookHeader = new DAUSACH();
                        bookHeader.MaDauSach = await GenerateId.Gen(typeof(DAUSACH), "MaDauSach");

                        //category handle
                        THELOAI bookCategory = await categoryRepo.GetSingleAsync(p => p.TenTheLoai == importExcelObject.TheLoai);
                        if (bookCategory == null)
                        {
                            bookCategory = new THELOAI();
                            bookCategory.MaTheLoai = await GenerateId.Gen(typeof(THELOAI), "MaTheLoai");
                            bookCategory.TenTheLoai = importExcelObject.TheLoai;
                            await categoryRepo.Add(bookCategory);
                        }
                        bookHeader.MaTheLoai = bookCategory.MaTheLoai;

                        //book's name handle
                        bookHeader.TenSach = importExcelObject.BookName;

                        //add to db
                        bookHeader.MaDauSach = await GenerateId.Gen(typeof(DAUSACH), "MaDauSach");
                        await bookHeaderRepo.Add(bookHeader);

                        //author handle
                        for (int j = 0; j < importExcelObject.Authors.Count; j++)
                        {
                            String authorName = importExcelObject.Authors[j];
                            TACGIA author = await authorRepo.GetSingleAsync(p => p.TenTacGia == authorName);
                            if (author == null)
                            {
                                author = new TACGIA();
                                author.TenTacGia = authorName;
                                author.MaTacGia = await GenerateId.Gen(typeof(TACGIA), "MaTacGia");
                                await authorRepo.Add(author);
                            }

                            //add to db relation with DAUSACH and TACGIA
                            await AuthorDetailAPI.Add(author.MaTacGia, bookHeader.MaDauSach);
                        }
                    }
                    book.MaDauSach = bookHeader.MaDauSach;
                    book.MaSach = await GenerateId.Gen(typeof(SACH), "MaSach");
                    book.DonGiaNhapMoiNhat = importExcelObject.UnitPrice;
                    book.SoLuong = importExcelObject.Amount;
                    book.NhaXuatBan = importExcelObject.NXB;
                    book.LanTaiBan = importExcelObject.TimeRepublish;
                    book.IsActive = true;
                    await bookRepo.Add(book);
                }
                else
                {
                    book.SoLuong += importExcelObject.Amount;
                    book.DonGiaNhapMoiNhat = importExcelObject.UnitPrice;
                    await bookRepo.Update(book);
                }
                await ImportDetailAPI.Add(book.MaSach, import.MaPhieuNhap, importExcelObject.Amount, importExcelObject.UnitPrice);
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

        private async Task<int> CheckGetDataFromExcel(FileInfo fileInfo)
        {
            if (IsFileLocked(fileInfo))
            {
                return -1;
            }    
            else if(fileInfo.Exists)
            {
                using (var package = new ExcelPackage(fileInfo))
                {
                    await package.LoadAsync(fileInfo.FullName);
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    if (worksheet.Cells[6, 1].Value == null || worksheet.Cells[6, 1].Value.ToString() != "Tên sách")
                    {
                        return 0;
                    }
                    if (worksheet.Cells[6, 2].Value == null || worksheet.Cells[6, 2].Value.ToString() != "Tác giả")
                    {
                        return 0;
                    }
                    if (worksheet.Cells[6, 3].Value == null || worksheet.Cells[6, 3].Value.ToString() != "Thể loại")
                    {
                        return 0;
                    }
                    if (worksheet.Cells[6, 4].Value == null || worksheet.Cells[6, 4].Value.ToString() != "Nhà xuất bản")
                    {
                        return 0;
                    }
                    if (worksheet.Cells[6, 5].Value == null || worksheet.Cells[6, 5].Value.ToString() != "Lần tái bản")
                    {
                        return 0;
                    }
                    if (worksheet.Cells[6, 6].Value == null || worksheet.Cells[6, 6].Value.ToString() != "Đơn giá")
                    {
                        return 0;
                    }
                    if (worksheet.Cells[6, 7].Value == null || worksheet.Cells[6, 7].Value.ToString() != "Số lượng")
                    {
                        return 0;
                    }

                    int row = 7;
                    while (worksheet.Cells[row,1].Value != null && worksheet.Cells[row, 1].Value.ToString() != "")
                    {
                        if (worksheet.Cells[row, 2].Value == null || worksheet.Cells[row, 2].Value.ToString() == "")
                        {
                            return 0;
                        }
                        if (worksheet.Cells[row, 3].Value == null || worksheet.Cells[row, 3].Value.ToString() == "")
                        {
                            return 0;
                        }
                        if (worksheet.Cells[row, 4].Value == null || worksheet.Cells[row, 4].Value.ToString() == "")
                        {
                            return 0;
                        }
                        if (worksheet.Cells[row, 5].Value == null || worksheet.Cells[row, 5].Value.ToString() == "")
                        {
                            return 0;
                        }
                        if (worksheet.Cells[row, 6].Value == null || worksheet.Cells[row, 6].Value.ToString() == "")
                        {
                            return 0;
                        }
                        if (worksheet.Cells[row, 7].Value == null || worksheet.Cells[row, 7].Value.ToString() == "")
                        {
                            return 0;
                        }

                        ImportExcelObject importExcelObject = new ImportExcelObject();

                        String bookName = worksheet.Cells[row, 1].Value.ToString();
                        if(bookName.Length > 100)
                        {
                            return 0;
                        }    
                        importExcelObject.BookName = bookName;

                        String bookAuthors = worksheet.Cells[row, 2].Value.ToString();
                        List<string> authorNames = bookAuthors.Split(',').ToList();
                        if (authorNames.Any(p => String.IsNullOrEmpty(p) || p.Length > 100) ||
                            authorNames.Distinct().Count() != authorNames.Count())
                        {
                            return 0;
                        }    
                        else
                        {
                            importExcelObject.Authors = authorNames.ToList();
                        }

                        String category = worksheet.Cells[row, 3].Value.ToString();
                        if(category.Length > 100)
                        {
                            return 0;
                        }    
                        importExcelObject.TheLoai = category;
                        

                        String NXB = worksheet.Cells[row, 4].Value.ToString();
                        if(NXB.Length > 100)
                        {
                            return 0;
                        }    
                        importExcelObject.NXB = NXB;

                        String LanTaiBan = worksheet.Cells[row, 5].Value.ToString();
                        if(LanTaiBan.Length > 4)
                        {
                            return 0;
                        }
                        try
                        {
                            importExcelObject.TimeRepublish = int.Parse(LanTaiBan);
                            if (importExcelObject.TimeRepublish < 0)
                            {
                                return 0;
                            }
                        }
                        catch
                        {
                            return 0;
                        }

                        String unitPrice = worksheet.Cells[row, 6].Value.ToString();
                        if(unitPrice.Length > 9)
                        {
                            return 0;
                        }    
                        try
                        {
                            importExcelObject.UnitPrice = decimal.Parse(unitPrice);
                            if (importExcelObject.UnitPrice < 0) 
                            {
                                return 0;
                            }
                        }
                        catch
                        {
                            return 0;
                        }

                        String amount = worksheet.Cells[row, 7].Value.ToString();
                        if (amount.Length > 9)
                        {
                            return 0;
                        }
                        try
                        {
                            importExcelObject.Amount = int.Parse(amount);
                            if (importExcelObject.Amount <= 0)
                            {
                                return 0;
                            }
                        }
                        catch
                        {
                            return 0;
                        }

                        importExcelObjects.Add(importExcelObject);
                        row++;
                    } 
                    if(row == 7)
                    {
                        return 0;
                    }    
                }
                return 1;
            }
            else
            {
                return -1;
            }
        }
        private async Task DowloadTemplate()
        {
            MainViewModel.IsLoading = true;
            string filePath = "";
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = "Template";
            saveFileDialog.Filter = "Excel Sheet(*.xlsx)|*.xlsx|All Files(*.*)|*.*";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = saveFileDialog.FileName;
            }
            else
            {
                PreviousItem = MainViewModel.UpdateDialog("Main");
                ConfirmDialog notification = new ConfirmDialog()
                {
                    Header = "Lỗi",
                    ContentString = "Xuất file không thành công.",
                    CM = new RelayCommandWithNoParameter(() =>
                    {
                        DialogHost.CloseDialogCommand.Execute(null, null);
                        DialogHost.Show(PreviousItem, "Main");
                    })
                };
                MainViewModel.SetLoading(false);
                await DialogHost.Show(notification, "Main");
                return;
            }
           
            try
            {
                using (ExcelPackage package = new ExcelPackage())
                {
                    //MainReport
                    ExcelWorksheet worksheetMain = package.Workbook.Worksheets.Add("MainReport");
                    worksheetMain.Cells["A1:F1"].Merge = true;
                    worksheetMain.Cells["A2:F2"].Merge = true;
                    worksheetMain.Cells["A3:F3"].Merge = true;
                    worksheetMain.Cells["A4:F4"].Merge = true;
                    worksheetMain.Cells["A5:F5"].Merge = true;
                    worksheetMain.Cells[1, 1].Value = "//Các dòng này chỉ để giới thiệu với bạn. Bạn có thể xóa tùy bạn.";
                    worksheetMain.Cells[2, 1].Value = "//Bạn không thể điền cùng lúc 2 sách có cùng tên, cùng nhóm tác giả nhưng khác thể loại.";
                    worksheetMain.Cells[3, 1].Value = "//Số ký tự tối đa: Tên sách: 100, Mỗi tên tác giả: 100, Thể loại: 100, NXB: 100, Lần tái bản: 4, Đơn giá: 9, Số Lượng: 9";
                    worksheetMain.Cells[4, 1].Value = "//Sách gồm nhiều tác giả, giữa tên các tác tác giả cần thêm \",\".";
                    worksheetMain.Cells[5, 1].Value = "//Mỗi sách sẽ được phân biệt bởi tên, nhóm tác giả, nhà sản xuất và lần tái bản.";
                    using (ExcelRange excelRange = worksheetMain.Cells[$"A1:G5"])
                    {
                        excelRange.Style.Font.Color.SetColor(Color.FromArgb(42, 169, 82));
                    }

                    worksheetMain.Cells[6, 1].Value = "Tên sách";
                    worksheetMain.Cells[6, 2].Value = "Tác giả";
                    worksheetMain.Cells[6, 3].Value = "Thể loại";
                    worksheetMain.Cells[6, 4].Value = "Nhà xuất bản";
                    worksheetMain.Cells[6, 5].Value = "Lần tái bản";
                    worksheetMain.Cells[6, 6].Value = "Đơn giá";
                    worksheetMain.Cells[6, 7].Value = "Số lượng";
                    using (ExcelRange excelRange = worksheetMain.Cells[$"A6:G1000"])
                    {
                        excelRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        excelRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        excelRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        excelRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }

                    var allCellsMain = worksheetMain.Cells[1, 1, worksheetMain.Dimension.End.Row, worksheetMain.Dimension.End.Column];
                    var cellFontMain = allCellsMain.Style.Font;
                    cellFontMain.SetFromFont("Times New Roman", 13);

                    worksheetMain.Cells.AutoFitColumns();
                    for (int i = 1; i <= 7; i++)
                    {
                        worksheetMain.Column(i).Width *= 1.2;
                    }

                    //Book template
                    ExcelWorksheet worksheetBook = package.Workbook.Worksheets.Add("BookData");
                    worksheetBook.Cells[1, 1].Value = "Tên sách";
                    worksheetBook.Cells[1, 2].Value = "Tác giả";
                    worksheetBook.Cells[1, 3].Value = "Thể loại";
                    worksheetBook.Cells[1, 4].Value = "Nhà xuất bản";
                    worksheetBook.Cells[1, 5].Value = "Lần tái bản";
                    worksheetBook.Cells[1, 6].Value = "Đơn giá nhập mới nhất";
                    int row = 0;
                    for (int i = 0; i < bookHeaders.Count; i++)
                    {
                        DAUSACH bookHeader = bookHeaders.ElementAt(i);
                        for (int j = 0; j < bookHeader.SACHes.Count; j++)
                        {
                            worksheetBook.Cells[2 + row, 1].Value = bookHeader.TenSach;
                            worksheetBook.Cells[2 + row, 2].Value = string.Join(",", bookHeader.TACGIAs.Select(p => p.TenTacGia).ToList());
                            worksheetBook.Cells[2 + row, 3].Value = bookHeader.THELOAI.TenTheLoai;
                            worksheetBook.Cells[2 + row, 4].Value = bookHeader.SACHes.ElementAt(j).NhaXuatBan;
                            worksheetBook.Cells[2 + row, 5].Value = bookHeader.SACHes.ElementAt(j).LanTaiBan;
                            worksheetBook.Cells[2 + row, 6].Value = bookHeader.SACHes.ElementAt(j).DonGiaNhapMoiNhat;
                            row++;
                        }
                    }

                    var allCellsBook = worksheetBook.Cells[1, 1, worksheetBook.Dimension.End.Row, worksheetBook.Dimension.End.Column];
                    var cellFontBook = allCellsBook.Style.Font;
                    cellFontBook.SetFromFont("Times New Roman", 13);

                    worksheetBook.Cells.AutoFitColumns();
                    for (int i = 1; i <= 6; i++)
                    {
                        worksheetBook.Column(i).Width *= 1.2;
                    }

                    using (ExcelRange excelRange = worksheetBook.Cells[$"A1:F{row + 1}"])
                    {
                        excelRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        excelRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        excelRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        excelRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }

                    //Author template
                    ExcelWorksheet worksheetAuthor = package.Workbook.Worksheets.Add("AuthorData");
                    worksheetAuthor.Cells[1, 1].Value = "Tên tác giả";
                    for (int i = 0; i < authors.Count; i++)
                    {
                        TACGIA author = authors.ElementAt(i);

                        worksheetAuthor.Cells[2 + i, 1].Value = author.TenTacGia;

                    }

                    var allCellsAuthors = worksheetAuthor.Cells[1, 1, worksheetAuthor.Dimension.End.Row, worksheetAuthor.Dimension.End.Column];
                    var cellFontAuthors = allCellsAuthors.Style.Font;
                    cellFontAuthors.SetFromFont("Times New Roman", 13);

                    worksheetAuthor.Cells.AutoFitColumns();
                    for (int i = 1; i <= 1; i++)
                    {
                        worksheetAuthor.Column(i).Width *= 1.2;
                    }

                    using (ExcelRange excelRange = worksheetAuthor.Cells[$"A1:A{authors.Count + 1}"])
                    {
                        excelRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        excelRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        excelRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        excelRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }

                    //Category template
                    ExcelWorksheet worksheetCategory = package.Workbook.Worksheets.Add("CategoryData");
                    worksheetCategory.Cells[1, 1].Value = "Tên thể loại";
                    for (int i = 0; i < categories.Count; i++)
                    {
                        THELOAI category = categories.ElementAt(i);

                        worksheetCategory.Cells[2 + i, 1].Value = category.TenTheLoai;

                    }

                    var allCellsCategories = worksheetCategory.Cells[1, 1, worksheetCategory.Dimension.End.Row, worksheetCategory.Dimension.End.Column];
                    var cellFontCategories = allCellsCategories.Style.Font;
                    cellFontCategories.SetFromFont("Times New Roman", 13);

                    worksheetCategory.Cells.AutoFitColumns();
                    for (int i = 1; i <= 1; i++)
                    {
                        worksheetCategory.Column(i).Width *= 1.2;
                    }

                    using (ExcelRange excelRange = worksheetCategory.Cells[$"A1:A{categories.Count + 1}"])
                    {
                        excelRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        excelRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        excelRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        excelRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }
                    Byte[] bin = package.GetAsByteArray();
                    File.WriteAllBytes(filePath, bin);
                }

                PreviousItem = MainViewModel.UpdateDialog("Main");
                ConfirmDialog notification = new ConfirmDialog()
                {
                    Header = "Thông báo",
                    ContentString = "Xuất file thành công.",
                    CM = new RelayCommandWithNoParameter(() =>
                    {
                        DialogHost.CloseDialogCommand.Execute(null, null);
                        DialogHost.Show(PreviousItem, "Main");
                    })
                };
                MainViewModel.SetLoading(false);
                await DialogHost.Show(notification, "Main");
            }
            catch
            {
                PreviousItem = MainViewModel.UpdateDialog("Main");
                ConfirmDialog notification = new ConfirmDialog()
                {
                    Header = "Lỗi",
                    ContentString = "Có lỗi khi lưu file.",
                    CM = new RelayCommandWithNoParameter(() =>
                    {
                        DialogHost.CloseDialogCommand.Execute(null, null);
                        DialogHost.Show(PreviousItem, "Main");
                    })
                };
                MainViewModel.SetLoading(false);
                await DialogHost.Show(notification, "Main");
            }
        }

        private class ImportExcelObject
        {
            public string BookName { get; set; }
            public string NXB { get; set; }
            public List<string> Authors { get; set; }
            public string TheLoai { get; set; }
            public int TimeRepublish { get; set; }
            public decimal UnitPrice { get; set; }
            public int Amount { set; get; }
        }
    }
}
