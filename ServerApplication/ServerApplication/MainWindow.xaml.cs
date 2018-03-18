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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ServerApplication
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InvalidCredentialsText.Visibility = Visibility.Hidden;
        }

        private void LoginAttempt(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordTextBox.Text;

            StreamReader read = new StreamReader("S:\\Repos\\SD2\\ServerApplication\\ServerApplication\\ValidCredentials.csv");

            var credentials = read.ReadLine().Split(',');

            while ((credentials[0] != null) && (credentials[1] != null) && (credentials[2] != null))
            {
                if ((credentials[0] == username) && (credentials[1] == password))
                {
                    MainControlWindow controlWindow = new MainControlWindow();
                    controlWindow.Show();
                    read.Close();
                    AppBrain.brain.Username = credentials[0];
                    AppBrain.brain.AccessLevel = credentials[2];
                    this.Close();
                    break;
                }
                else
                {
                    InvalidCredentialsText.Visibility = Visibility.Visible;
                    credentials[0] = read.ReadLine();
                    credentials[1] = read.ReadLine();
                    credentials[2] = read.ReadLine();
                }
            }
        }
    }
}
