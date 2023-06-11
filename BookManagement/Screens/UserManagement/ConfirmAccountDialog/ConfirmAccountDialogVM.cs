using BookManagement.Models;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BookManagement
{
    public class ConfirmAccountDialogVM
    {
        #region GenericDataRepository
        private GenericDataRepository<NHANVIEN> userRepo =new GenericDataRepository<NHANVIEN>();
        #endregion 
        #region Action
        public Action VerifiedAccount { get; set; }
        #endregion

        #region Command
        public ICommand VerifyAccountCommand { get; set; }
        #endregion

        #region Properties
        public string Password { get; set; }
        private System.Windows.Controls.UserControl PreviousItem;
        #endregion

        public ConfirmAccountDialogVM(NHANVIEN userHasAccessToReset, NHANVIEN userNeedToReset) {
            VerifyAccountCommand = new RelayCommand<object>(p =>
            {
               return !String.IsNullOrEmpty(Password);
            },async p =>
            {
                string password = changePassword(Password);
                if (password != userHasAccessToReset.Password) 
                {
                    PreviousItem = MainViewModel.UpdateDialog("Main");
                    var dl = new ConfirmDialog()
                    {
                        Header = "Oops",
                        ContentString = "Mật khẩu vừa nhập không đúng. Xin hãy nhập lại.",
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
                    userNeedToReset.Password = changePassword("BUUK123");
                    await userRepo.Update(userNeedToReset);
                    DialogHost.CloseDialogCommand.Execute(null, null);
                    var dl = new ConfirmDialog()
                    {
                        Header = "Oops",
                        ContentString = $"Mật khẩu của {userNeedToReset.TenNhanVien} đã được reset lại thành BUUK123.",
                    };
                    await DialogHost.Show(dl, "Main");
                }    
            });
        }
        private String changePassword(String password)
        {
            byte[] temp = ASCIIEncoding.ASCII.GetBytes(password);
            byte[] hasData = new MD5CryptoServiceProvider().ComputeHash(temp);

            string hasPass = "";

            foreach (byte item in hasData)
            {
                hasPass += item;
            }
            return hasPass;
        }
    }
}
