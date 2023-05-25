using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace BookManagement {
    public class DPIService {
        //static public DPIService instance;
        public IServiceProvider serviceProvider;
        public DPIService() {
            IServiceCollection services = new ServiceCollection();

            #region Set Store and some initial dependences

            NavigationStore.instance = new NavigationStore();
            AccountStore.instance = new AccountStore();
            RuleStore.instance = new RuleStore();

            services.AddTransient<DrawerVM>();
            #endregion

            //Set service
            //setup Transient ViewModel
            #region Shop
            services.AddTransient<HomeScreenVM>();
            services.AddTransient<SaleBookScreenVM>();
            services.AddTransient<BookManagementScreenVM>();
            services.AddTransient<BillManagementScreenVM>();
            services.AddTransient<ImportBookManagementScreenVM>();
            services.AddTransient<ImportBookPageVM>();
            services.AddTransient<DebtManagementScreenVM>();
            services.AddTransient<SettingScreenVM>();
            services.AddTransient<UserManagementScreenVM>();
            services.AddTransient<NoAccessScreenVM>();
            #endregion

            //Setup MainWindow
            #region MainWindow Setup
            services.AddSingleton<INavigationService>(s => NavigateProvider.HomeScreen());

            services.AddSingleton<MainViewModel>();

            services.AddSingleton<MainWindow>(s => new MainWindow() {
                DataContext = s.GetRequiredService<MainViewModel>()
            });

            services.AddTransient<LoginViewModel>();
            services.AddSingleton<Login>(s => new Login()
            {
                DataContext = s.GetRequiredService<LoginViewModel>()
            });

            serviceProvider = services.BuildServiceProvider();

            NavigateProvider.serviceProvider = serviceProvider;
            App.serviceProvider = serviceProvider;
            #endregion
        }
    }
}
