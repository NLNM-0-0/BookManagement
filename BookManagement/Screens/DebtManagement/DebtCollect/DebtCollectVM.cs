using BookManagement.Models;
using MaterialDesignThemes.Wpf;
using System;
using System.Windows.Input;

namespace BookManagement
{
    public class DebtCollectVM:BaseViewModel
    {
        #region Action
        public Action AddDebtCollectSuccess { get; set; }
        #endregion
        #region GenericDataRepository
        GenericDataRepository<PHIEUTHUNO> debtCollectRepo = new GenericDataRepository<PHIEUTHUNO>();
        GenericDataRepository<KHACHHANG> customerRepo = new GenericDataRepository<KHACHHANG>();
        #endregion

        #region Commands
        public ICommand CollectCommand { get; set; }
        #endregion

        #region Properties
        private System.Windows.Controls.UserControl PreviousItem;
        public string CustomerName { get => "Tên: " + Customer.TenKhachHang; }
        public string CustomerPhone { get => "SĐT: " + Customer.DienThoai; }
        public string DebtCollect { get; set; }
        public bool IsPrint { get; set; }
        private KHACHHANG customer;
        public KHACHHANG Customer
        { 
            get { return customer; } 
            set {  
                customer = value; 
                OnPropertyChanged();
            } 
        }
        #endregion

        #region Constructor
        public DebtCollectVM(KHACHHANG customer)
        {
            Customer = customer;
            CollectCommand = new RelayCommand<object>((p) =>
            {
                return DebtCollect != null && DebtCollect.Length > 0;
            },async (p) =>
            {
                PreviousItem = MainViewModel.UpdateDialog("Main");
                if (RuleStore.instance.getValue(AppEnum.ApDungQDKiemTraTienThu) == 1)
                {
                    if (decimal.Parse(DebtCollect) > Customer.TienNo)
                    {
                        var dl = new ConfirmDialog()
                        {
                            Header = "Oops",
                            ContentString = "Bạn không được phép thu vượt quá số tiền khách hàng đang nợ",
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
                PHIEUTHUNO debtCollect = new PHIEUTHUNO();
                debtCollect.MaPhieuThu = await GenerateId.Gen(typeof(PHIEUTHUNO), "MaPhieuThu");
                debtCollect.NgayThu = DateTime.Now;
                debtCollect.SoTienThu = decimal.Parse(DebtCollect);
                debtCollect.MaNhanVien = AccountStore.instance.CurrentAccount.MaNhanVien;
                debtCollect.MaKhachHang = Customer.MaKhachHang;
                await debtCollectRepo.Add(debtCollect);

                debtCollect.KHACHHANG = Customer;
                debtCollect.NHANVIEN = AccountStore.instance.CurrentAccount;

                Customer.TienNo -= decimal.Parse(DebtCollect);
                await customerRepo.Update(Customer);

                AddDebtCollectSuccess?.Invoke();

                if (IsPrint)
                {
                    DebtCollectPdf pdf = new DebtCollectPdf();
                    pdf.DataContext = new DebtCollectPdfVM(debtCollect);
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
        }
        #endregion
    }
}
