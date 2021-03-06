﻿using System.IO;
using System.Windows;

namespace ServerApplication
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AppBrain.brain.Mw = this;
            InvalidCredentialsText.Visibility = Visibility.Hidden;
        }

        private void LoginAttempt(object sender, RoutedEventArgs e)
        {
            // Grab entered credentials to verify
            string username = UsernameTextBox.Text;
            string password = PasswordTextBox.Text;

            // Read in our credentials file
            var credentials = File.ReadAllLines(AppBrain.brain.UserCredentialsFile);

            // Go through each user in the file and check entered credentials against it, log in if found throw error if not
            for (int i = 0; i < credentials.Length; i++)
            {
                string[] userCredentials = credentials[i].Split(',');

                if ((userCredentials[0] == username) && (userCredentials[1] == password))
                {
                    AppBrain.brain.Username = userCredentials[0];
                    AppBrain.brain.AccessLevel = userCredentials[2];
                    MainControlWindow controlWindow = new MainControlWindow();
                    controlWindow.Show();
                    AppBrain.brain.Mw = null;
                    this.Close();
                    AppBrain.brain.LoadSiteConfiguration();
                    break;
                }
                else
                {
                    InvalidCredentialsText.Visibility = Visibility.Visible;
                }
            }
        }
    }
}
