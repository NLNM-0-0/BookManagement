using BookManagement.Models;
using MaterialDesignThemes.Wpf;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BookManagement {
    public class LoginViewModel : BaseViewModel {
        private readonly GenericDataRepository<Models.NHANVIEN> userRepo = new GenericDataRepository<NHANVIEN>();

        private string userName;
        public string UserName {
            get => userName;
            set {
                userName = value;
            } 
        }
        public string Password { get; set; }

        public bool IsLoading { get; set; } = false;

        public ICommand OnLogin { get; set; }
        public ICommand CloseCM { get; set; }

        #region Keyhandle
        public ICommand KeyHandle_EnterKeepSignIn { get; }
        public ICommand KeyHandle_EnterLogin { get; }
        #endregion
        public LoginViewModel() {
            UserName = "";
            Password = "";

            OnLogin = new RelayCommand<object>(p => {
                return !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password);
            }, async p => {
                if(await Login()) {
                    NavigateProvider.LoginScreenHandle(false);
                }
            });

            CloseCM = new RelayCommand<object>(p => true, p => {
                (p as Window).Close();
            });

            KeyHandle_EnterLogin = new RelayCommand<object>(p => {
                return (bool)p;
            }, p => {
                OnLogin.Execute(p);
            });
        }

        private async Task<bool> Login() {
            IsLoading = true;
            var user = await userRepo.GetSingleAsync(u => u.UserName == UserName && u.Password == Password, u => u.NHOMNGUOIDUNG, u => u.NHOMNGUOIDUNG.CHUCNANGs);
            IsLoading = false;
            if (user != null)
            {
                if (user.isActive == false)
                {
                    var confirmDialog = new ConfirmDialog()
                    {
                        Header = "Whoops",
                        ContentString = "Tài khoản này không còn hoạt động được nữa."
                    };
                    await DialogHost.Show(confirmDialog, "Login");
                    return false;
                }
                else
                {
                    AccountStore.instance.CurrentAccount = user;
                    UserName = "";
                    Password = "";
                    return true;
                }    
            }
            
            var dl = new ConfirmDialog() {
                Header = "Whoops",
                ContentString = "Tên đăng nhập hoặc mật khẩu sai. Xin hãy thử lại!"
            };
            await DialogHost.Show(dl, "Login");
            return false;
        }
        static public void OnClosing(object sender, CancelEventArgs e)
        {
            if (DialogHost.IsDialogOpen("Login"))
                DialogHost.Close("Login");

            e.Cancel = true;
            (sender as Window).Closing -= LoginViewModel.OnClosing;
            App.Current.Shutdown();
        }
    }
}

