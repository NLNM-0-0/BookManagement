using BookManagement.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace BookManagement
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider serviceProvider;

        public App()
        {
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            string primaryColor = ConfigurationManager.AppSettings["PrimaryColor"];

            if (!string.IsNullOrEmpty(primaryColor))
            {
                Color primaryHueColor = (Color)ColorConverter.ConvertFromString(primaryColor);

                Application.Current.Resources["PrimaryHue"] = primaryHueColor;
                Application.Current.Resources["PrimaryHueMidBrush"] = new SolidColorBrush(primaryHueColor);

                HSLColor hSL = HSLColor.FromRGB(primaryHueColor);
                hSL.Lightness += 0.2;
                Application.Current.Resources["PrimaryHueLightBrush"] = new SolidColorBrush(hSL.ToRGB());

                hSL.Lightness -= 0.4;
                Application.Current.Resources["PrimaryHueDarkBrush"] = new SolidColorBrush(hSL.ToRGB());
            }

            /*SplashScreen splashScreen = new SplashScreen();
            splashScreen.Show();*/

            Internet.instance = new Internet();
            Task.Run(async () => {
                DPIService dpi = new DPIService();
                await load();
            }).ContinueWith(_ => {
                Internet.instance.Start();

                MainWindow = null;
                INavigationService initial;

                initial = serviceProvider.GetRequiredService<INavigationService>();

                initial.Navigate();
                Internet.IsConnected = true;
                if (!Internet.CheckConnection())
                {
                    Internet.IsConnected = false;
                    NavigateProvider.OfflineScreen().NoBackNavigate();
                }

                var mainWindow = serviceProvider.GetRequiredService<MainWindow>();

                Login p = serviceProvider.GetRequiredService<Login>(); //initial
                /*p.Show();
                p.Hide();*/
                //Subcribe access SplashScreen from MainWindow
                mainWindow.Loaded += (sender, args) =>
                {
                    /*splashScreen.Close();
                    splashScreen = null;*/
                };
                MainWindow = mainWindow;
                p.Show();

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        async Task load()
        {
            GenericDataRepository<THAMSO> ruleRepo = new GenericDataRepository<THAMSO>();
            ICollection<THAMSO> ruleList = await ruleRepo.GetAllAsync();
            RuleStore.instance.CurrentRules = new ObservableCollection<THAMSO>(ruleList);
        }
    }
}
