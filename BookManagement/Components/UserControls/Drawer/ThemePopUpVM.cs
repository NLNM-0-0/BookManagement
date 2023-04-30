using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BookManagement 
{
    public class ThemePopUpVM : BaseViewModel
    {
        #region Command
        public ICommand OnChangePrimaryColor { get; set; }
        #endregion

        public ThemePopUpVM()
        {
            OnChangePrimaryColor = new RelayCommand<object>((p) => {
                return true;
            },
            (p) => {
                Button button = p as Button;

                SolidColorBrush newBrush = new SolidColorBrush((button.Background as SolidColorBrush).Color);

                HSLColor hSL = HSLColor.FromRGB(newBrush.Color);
                hSL.Lightness += 0.2;

                Application.Current.Resources["PrimaryHueMidBrush"] = newBrush;
                Application.Current.Resources["PrimaryHueLightBrush"] = new SolidColorBrush(hSL.ToRGB());
                hSL.Lightness -= 0.4;
                Application.Current.Resources["PrimaryHueDarkBrush"] = new SolidColorBrush(hSL.ToRGB());

                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["PrimaryColor"].Value = newBrush.Color.ToString();
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            });
        }
    }
}
