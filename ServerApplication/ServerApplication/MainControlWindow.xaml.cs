using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ServerApplication
{
    public partial class MainControlWindow : Window
    {
        public MainControlWindow()
        {
            InitializeComponent();
            AppBrain.brain.Mcw = this;
            LoggedInAsLabel.Text = "Current User: " + AppBrain.brain.Username;
            AccessLevelTextBlock.Text = "Access Level: " + AppBrain.brain.AccessLevel;

            if (AppBrain.brain.AccessLevel != "2" && AppBrain.brain.AccessLevel != "3")
            {
                ManageSiteConfigurationButton.Visibility = Visibility.Hidden;
            }
        }

        private void Logout(object sender, RoutedEventArgs e)
        {
            MainWindow loginWindow = new MainWindow();
            loginWindow.Show();
            AppBrain.brain.Mcw = null;
            this.Close();
        }

        private void ManageAccount(object sender, RoutedEventArgs e)
        {
            ManageAccountWindow accountWindow = new ManageAccountWindow();
            accountWindow.Show();
            AppBrain.brain.Mcw = null;
            this.Close();
        }

        private void SiteConfiguration(object sender, RoutedEventArgs e)
        {
            ManageSiteConfigurationWindow siteConfigWindow = new ManageSiteConfigurationWindow();
            siteConfigWindow.Show();
            this.Close();
        }

        private void SwitchgearManagement(object sender, RoutedEventArgs e)
        {
            ManageSwitchgearsWindow switchgearsWindow = new ManageSwitchgearsWindow();
            switchgearsWindow.Show();
            this.Close();
        }
    }
}
