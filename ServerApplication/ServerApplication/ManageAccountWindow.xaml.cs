using System;
using System.Collections.Generic;
using System.IO;
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
    public partial class ManageAccountWindow : Window
    {
        public ManageAccountWindow()
        {
            InitializeComponent();
            LoggedInAsLabel.Text = "Current User: " + AppBrain.brain.Username;
            AccessLevelTextBlock.Text = "Access Level: " + AppBrain.brain.AccessLevel;

            IncorrectPasswordTextBlock.Visibility = Visibility.Hidden;
            NewPasswordMismatchTextBlock.Visibility = Visibility.Hidden;
        }

        private void ControlPanelWindow(object sender, RoutedEventArgs e)
        {
            MainControlWindow controlPanelWindow = new MainControlWindow();
            controlPanelWindow.Show();
            this.Close();
        }

        private void Logout(object sender, RoutedEventArgs e)
        {
            MainWindow loginWindow = new MainWindow();
            loginWindow.Show();
            this.Close();
        }

        private void ChangePassword(object sender, RoutedEventArgs e)
        {
            StreamReader read = new StreamReader("S:\\Repos\\SD2\\ServerApplication\\ServerApplication\\ValidCredentials.csv");

            string currentPass = CurrentPasswordTextBox.Text;
            string newPass = NewPasswordTextBox.Text;
            string confirmedNewPass = ConfirmedNewPasswordTextBox.Text;

            if (newPass != confirmedNewPass)
            {
                NewPasswordMismatchTextBlock.Visibility = Visibility.Visible;
                return;
            }

            string user = read.ReadLine();
            string pass = read.ReadLine();
            string accessLevel = read.ReadLine();

            while ((user != null) && (pass != null) && (accessLevel != null))
            {

            }
        }
    }
}
