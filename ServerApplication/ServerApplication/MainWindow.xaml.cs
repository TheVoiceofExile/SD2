﻿using System;
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

            StreamReader read = new StreamReader("S:\\Repos\\SD2\\ServerApplication\\ServerApplication\\ValidCredentials.txt");

            string validUser = read.ReadLine();
            string validPass = read.ReadLine();
            string accessLevel = read.ReadLine();

            while ((validUser != null) && (validPass != null) && (accessLevel != null))
            {
                if ((validUser == username) && (validPass == password))
                {
                    MainControlWindow controlWindow = new MainControlWindow();
                    controlWindow.Show();
                    read.Close();
                    AppBrain.brain.Username = validUser;
                    AppBrain.brain.AccessLevel = accessLevel;
                    this.Close();
                    break;
                }
                else
                {
                    InvalidCredentialsText.Visibility = Visibility.Visible;
                    validUser = read.ReadLine();
                    validPass = read.ReadLine();
                    accessLevel = read.ReadLine();
                }
            }
        }
    }
}
