using BookManagement.Models;
using BookManagement.Screens.SaleBook.AddCustomer;
using DocumentFormat.OpenXml.Wordprocessing;
using MaterialDesignThemes.Wpf;
using OfficeOpenXml.FormulaParsing.ExpressionGraph.FunctionCompilers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BookManagement
{
    public class PayConfirmDialogVM : BaseViewModel
    {
        #region Access property
        public bool IsAllowSaveBillWithDebt =>
            AccountStore.instance.CurrentAccount.NHOMNGUOIDUNG.CHUCNANGs.Any(p => p.MaChucNang == AppEnum.GhiNoKhachHang);
        #endregion

        #region Action
        public Action AddBillSuccess {  get; set; }
        #endregion
        #region GenericDataRepository
        private GenericDataRepository<KHACHHANG> customerRepo = new GenericDataRepository<KHACHHANG> { };
        private GenericDataRepository<HOADON> billRepo = new GenericDataRepository<HOADON> { };
        private GenericDataRepository<SACH> bookRepo = new GenericDataRepository<SACH> { };
        private GenericDataRepository<CHITIETHOADON> billDetailRepo = new GenericDataRepository<CHITIETHOADON> { };
        #endregion

        #region Command
        public ICommand ConfirmCommand { get; set; }
        public ICommand AddCustomerCommand { get; set; }
        #endregion

        #region 
        private bool isBillHasDebt;
        public bool IsBillHasDebt
        {
            get => isBillHasDebt;
            set
            {
                isBillHasDebt = value;
                OnPropertyChanged();
            }
        }
        private bool isBillNotHasDebt;
        public bool IsBillNotHasDebt
        {
            get => isBillNotHasDebt;
            set
            {
                isBillNotHasDebt = value;
                OnPropertyChanged();
            }
        }
        private KHACHHANG selectedCustomer;
        public KHACHHANG SelectedCustomer
        {
            get => selectedCustomer;
            set
            {
                selectedCustomer = value;
                OnPropertyChanged();
            }
        }
        private String selectedCustomerString;
        public String SelectedCustomerString
        {
            get => selectedCustomerString;
            set
            {
                selectedCustomerString = value;
                if(selectedCustomer!= null && selectedCustomer.DienThoai == selectedCustomerString) 
                {
                    CustomerName = selectedCustomer.TenKhachHang;
                }
                else
                {
                    CustomerName = "";
                }    
                OnPropertyChanged();
            }
        }
        private String customerName;
        public String CustomerName
        {
            get => customerName;
            set
            {
                customerName = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<KHACHHANG> customers;
        public ObservableCollection<KHACHHANG> Customers
        {
            get => customers;
            set
            {
                customers = value;
                OnPropertyChanged();
            }
        }
        public string EarnMoney { get; set; }
        public bool IsPrintBill { get; set; }
        private decimal sum { get; set; }
        private System.Windows.Controls.UserControl PreviousItem;
        #endregion

        #region Constructor
        public PayConfirmDialogVM(List<CHITIETHOADON> billDetails)
        {
            IsBillHasDebt = false;
            IsBillNotHasDebt = true;
            Task.Run(async () =>
            {
                await Load();
            }).ContinueWith((_) =>
            {
                sum = billDetails.Sum(b => b.SoLuong * b.DonGia);
                AddCustomerCommand = new RelayCommandWithNoParameter(async () =>
                {
                    PreviousItem = MainViewModel.UpdateDialog("Main");
                    AddCustomer addCustomer = new AddCustomer();
                    AddCustomerVM addCustomerVM = new AddCustomerVM(customers.ToList());
                    addCustomerVM.ClosedDialog += AddCustomerBack;
                    addCustomerVM.CloseDialogCommand = new RelayCommandWithNoParameter(() =>
                    {
                        DialogHost.CloseDialogCommand.Execute(null, null);
                        if (PreviousItem != null)
                        {
                            DialogHost.Show(PreviousItem, "Main");
                        }
                    });
                    addCustomer.DataContext = addCustomerVM;
                    await DialogHost.Show(addCustomer, "Main");
                });

                ConfirmCommand = new RelayCommand<object>((p) =>
                {
                    return IsBillHasDebt == false || (IsBillHasDebt && !String.IsNullOrEmpty(EarnMoney) && !String.IsNullOrEmpty(CustomerName));
                }, async (p) =>
                {
                    PreviousItem = MainViewModel.UpdateDialog("Main");
                    //Check condition
                    if (IsBillHasDebt == true)
                    {
                        if(decimal.Parse(EarnMoney) > sum)
                        {
                            var dl = new ConfirmDialog()
                            {
                                Header = "Oops",
                                ContentString = "Bạn thu số tiền vượt quá số tiền hóa đơn.",
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
                            return;
                        }
                        if(decimal.Parse(EarnMoney) == sum)
                        {
                            var dl = new ConfirmDialog()
                            {
                                Header = "Oops",
                                ContentString = "Số tiền bạn thu vừa đủ. Xin hãy chọn đã nhận đủ tiền.",
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
                            return;
                        }
                        if (selectedCustomer!=null && !String.IsNullOrEmpty(selectedCustomer.MaKhachHang))
                        {
                            if(selectedCustomer.TienNo > RuleStore.instance.getValue(AppEnum.TienNoToiDaKhiBan))
                            {
                                var dl = new ConfirmDialog()
                                {
                                    Header = "Oops",
                                    ContentString = $"Hệ thống chỉ bán cho khách có tiền nợ không quá {RuleStore.instance.getValue(AppEnum.TienNoToiDaKhiBan).ToString("G29")}đ.",
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
                                return;
                            }    
                        }
                    }
                    int minInStockAfterSell = RuleStore.instance.getValue(AppEnum.LuongTonToiDaKhiBan);
                    if (billDetails.Any(b=>b.SACH.SoLuong - b.SoLuong < minInStockAfterSell))
                    {
                        var dl = new ConfirmDialog()
                        {
                            Header = "Oops",
                            ContentString = $"Hệ thống chỉ bán các sách có lượng tồn sau khi bán lơn hoặc bằng {minInStockAfterSell}.",
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
                        return;
                    }


                    HOADON bill = new HOADON();
                    bill.MaHoaDon = await GenerateId.Gen(typeof(HOADON), "MaHoaDon");
                    bill.MaNhanVien = AccountStore.instance.CurrentAccount.MaNhanVien;
                    bill.TongTien = sum;
                    if (IsBillHasDebt)
                    {
                        if(!String.IsNullOrEmpty(EarnMoney))
                        {
                            bill.SoTienNo = bill.TongTien - decimal.Parse(EarnMoney);
                        }
                        if (selectedCustomer != null)
                        {
                            if (String.IsNullOrEmpty(selectedCustomer.MaKhachHang))
                            {
                                selectedCustomer.MaKhachHang = await GenerateId.Gen(typeof(KHACHHANG), "MaKhachHang");
                                selectedCustomer.TienNo = bill.SoTienNo;
                                await customerRepo.Add(selectedCustomer);
                            }
                            else
                            {
                                selectedCustomer.TienNo += bill.SoTienNo;
                                await customerRepo.Update(selectedCustomer);
                            }
                            bill.MaKhachHang = selectedCustomer.MaKhachHang;
                        }
                    }
                    else
                    {
                        bill.SoTienNo = 0;
                    }
                    
                    bill.NgayLapHoaDon = DateTime.Now;
                    await billRepo.Add(bill);

                    //Dòng dưới để đảm bảo ko bị duplicate KHACHHANG
                    bill.KHACHHANG = selectedCustomer;

                    bill.CHITIETHOADONs = billDetails;
                    for (int i = 0; i < billDetails.Count; i++)
                    {
                        CHITIETHOADON billDetail = billDetails[i];
                        billDetail.MaHoaDon = bill.MaHoaDon;

                        //Dòng dưới để đảm bảo ko bị duplicate DAUSACH, THELOAI
                        SACH book = billDetail.SACH;
                        billDetail.SACH = null;
                        await billDetailRepo.Add(billDetail);
                        billDetail.SACH = book;
                        billDetail.SACH.SoLuong -= billDetail.SoLuong;

                        await bookRepo.Update(billDetail.SACH);
                    }

                    AddBillSuccess?.Invoke();

                    if (IsPrintBill)
                    {
                        BillInfoPdf pdf = new BillInfoPdf();
                        pdf.DataContext = new BillInforPdfViewModel(bill);
                        try
                        {
                            pdf.Print();
                        }
                        catch
                        {
                            var dl = new ConfirmDialog()
                            {
                                Header = "Oops",
                                ContentString = "Đã có lỗi xảy ra. Xin hãy kiểm tra lại bạn đã đóng file hay chưa."
                            };
                            await DialogHost.Show(dl, "Main");
                        }
                    }
                });
            });
            
        }
        #endregion
        private void AddCustomerBack(KHACHHANG customer)
        {
            Customers.Add(customer);
            SelectedCustomer = customer;
        }
        private async Task Load()
        {
            Customers = new ObservableCollection<KHACHHANG>(await customerRepo.GetAllAsync());
        }
    }
}
