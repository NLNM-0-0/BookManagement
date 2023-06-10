using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DynamicExpresso;
using MaterialDesignThemes.Wpf;

namespace BookManagement {
    public class MainViewModel : BaseViewModel {

        #region Loading Field
        private static bool isLoading;
        public static bool IsLoading {
            get { return isLoading; }
            set {
                if(isLoading != value) {
                    isLoading = value;
                    NotifyStaticPropertyChanged();
                }
            }
        }

        public static event PropertyChangedEventHandler StaticPropertyChanged;

        private static void NotifyStaticPropertyChanged([CallerMemberName] string name = null) {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(name));
        }

        static CancellationTokenSource cts = new CancellationTokenSource();
        public static async void SetLoading(bool value, bool haveTimeout = true, int delay = 2003, string header = null, string message = null) {
            IsLoading = value;
            if(!haveTimeout) return;
            if(value == false) {
                cts.Cancel();
                cts = new CancellationTokenSource();
            }
            if(value == true) {
                try {
                    await Task.Delay(delay, cts.Token);
                } catch { return; }
                bool flag = false;
                if(IsLoading == true) {
                    flag = true;
                    IsLoading = false;
                }
                if(flag) {
                    if(message == null) message = "Chúng tôi đang xử lý yêu cầu của bạn.\n Nó sẽ tốn 1 khoảng thời gian để cập nhật!";
                    if(header == null) header = "Đang xử lý";
                    App.Current.Dispatcher.Invoke(() => {
                        var dl = new ConfirmDialog() {
                            ContentString = message,
                            Header = header,
                        };
                        var t = DialogHost.GetDialogSession("App");
                        if(t != null) DialogHost.Close("App");
                        DialogHost.Show(dl, "App");
                    });
                }
            }
        }
        #endregion

        private readonly NavigationStore _navigationStore;
        
        public DrawerVM DrawerVM { get; }
        public ICommand CloseCM { get; }
        public BaseViewModel CurrentViewModel => _navigationStore?.CurrentViewModel;

        public MainViewModel(
            DrawerVM drawerVM) {
            DrawerVM = drawerVM;
            IsLoading = false;
            
            _navigationStore = NavigationStore.instance;

            _navigationStore.CurrentVMChanged += OnCurrentVMChanged;

            CloseCM = new ImmediateCommand<object>(p => {
                App.Current.MainWindow.Close();
            });
        }
        private void OnCurrentVMChanged() {
            OnPropertyChanged(nameof(CurrentViewModel));
        }

        static public void OnClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            if ((sender as Window).WindowState == WindowState.Minimized)
                (sender as Window).WindowState = WindowState.Normal;
            var view = new ConfirmDialog()
            {
                Param = sender,
                CM = new RelayCommand<object>(t => true, t =>
                {
                    (t as Window).Closing -= MainViewModel.OnClosing;
                    (t as Window).Close();
                    App.Current.Shutdown();
                }),
                Header = "Bạn có chắc?",
                ContentString = "Quy trình của bạn có thể không được lưu nếu bạn đóng ứng dụng. Vui lòng kiểm tra trước khi đóng ứng dụng!"
            };

            //need to handle mulltiple identifier
            UserControl Main = UpdateDialog("Main");

            DialogHost.Show(view, "App", null, (sd, agr) =>
            {
                if (Main != null)
                    UpdateDialog("Main", Main);
            });
        }

        public static UserControl UpdateDialog(string Identifier, UserControl content = null) {
            DialogSession dialog;
            try {
                dialog = DialogHost.GetDialogSession(Identifier);
            }
            catch {
                dialog = null;
            }
            if(content != null) {
                DialogHost.Show(content, Identifier);
                return null;
            }
            content = dialog?.Content as UserControl;
            if(content != null) dialog.Close();
            return content;
        }
    }
}
