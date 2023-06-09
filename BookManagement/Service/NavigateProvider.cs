using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookManagement.Models;
using Microsoft.Extensions.DependencyInjection;

namespace BookManagement {
    public class NavigateProvider {
        public static IServiceProvider serviceProvider;
        public static bool Back() {
            var nav = NavigationStore.instance.stackScreen;
            if(nav.Count < 2) {
                return false;
            }
            var t = nav[nav.Count - 2];
            if(t.Item2 == null) t.Item1.Navigate();
            else t.Item1.Navigate(t.Item2);
            nav.RemoveAt(nav.Count - 1);
            nav.RemoveAt(nav.Count - 1);
            return true;
        }

        public static bool LoginScreenHandle(bool type = false)
        {
            try
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    Login p = App.serviceProvider.GetRequiredService<Login>();
                    bool IsMainVisible = App.Current.MainWindow.Visibility == System.Windows.Visibility.Visible;
                    if (!IsMainVisible && !type)
                    {
                        App.Current.MainWindow.Show();
                        p.Hide();
                    }
                    else if (IsMainVisible && type)
                    {
                        p.Show();
                        App.Current.MainWindow.Hide();
                    }
                });
                return true;
            }
            catch { return false; }
        }

        static public INavigationService SaleBookScreen()
        {
            return new NavigationService<SaleBookScreenVM>(
                serviceProvider.GetRequiredService<SaleBookScreenVM>);
        }
        static public INavigationService BillManagementScreen()
        {
            return new NavigationService<BillManagementScreenVM>(
                serviceProvider.GetRequiredService<BillManagementScreenVM>);
        }
        static public INavigationService BillDetailScreen()
        {
            return new ParamNavigationService<BillDetailViewModel>(
                (p) => new BillDetailViewModel(p as string));
        }
        static public INavigationService BookManagementScreen()
        {
            return new NavigationService<BookManagementScreenVM>(
                serviceProvider.GetRequiredService<BookManagementScreenVM>);
        }
        static public INavigationService DebtManagementScreen()
        {
            return new NavigationService<DebtManagementScreenVM>(
                serviceProvider.GetRequiredService<DebtManagementScreenVM>);
        }
        static public INavigationService DeptDetailScreen()
        {
            return new ParamNavigationService<DebtDetailViewModel>(
                (p) => new DebtDetailViewModel(p as string));
        }
        static public INavigationService HomeScreen()
        {
            return new NavigationService<ReportScreenVM>(
                serviceProvider.GetRequiredService<ReportScreenVM>);
        }
        static public INavigationService ImportBookManagementScreen()
        {
            return new NavigationService<ImportBookManagementScreenVM>(
                serviceProvider.GetRequiredService<ImportBookManagementScreenVM>);
        }
        static public INavigationService ImportBookPage()
        {
            return new ParamNavigationService<ImportBookPageVM>(
                (p) => new ImportBookPageVM(p as PHIEUNHAPSACH));
        }
        static public INavigationService SettingScreen()
        {
            return new NavigationService<SettingScreenVM>(
                serviceProvider.GetRequiredService<SettingScreenVM>);
        }
        static public INavigationService StaffManagementScreen()
        {
            return new NavigationService<UserManagementScreenVM>(
                serviceProvider.GetRequiredService<UserManagementScreenVM>);
        }
        static public INavigationService NoAccessScreen()
        {
            return new NavigationService<NoAccessScreenVM>(
                serviceProvider.GetRequiredService<NoAccessScreenVM>);
        }
        
    }
}
