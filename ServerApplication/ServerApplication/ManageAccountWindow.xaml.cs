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

            // Hide all of the error messages
            IncorrectPasswordTextBlock.Visibility = Visibility.Hidden;
            NewPasswordMismatchTextBlock.Visibility = Visibility.Hidden;
            NoUserEnteredError.Visibility = Visibility.Hidden;
            TryingToModifySelfError.Visibility = Visibility.Hidden;
            NoModificationsMadeTextBlock.Visibility = Visibility.Hidden;

            // Users without access level 3 can't modify other users, hide all that content
            if (AppBrain.brain.AccessLevel != "3")
            {
                ModifyAddUserButton.Visibility = Visibility.Hidden;
                ModifyAddUserPasswordTextBlock.Visibility = Visibility.Hidden;
                ModifyAddUserPasswordTextBox.Visibility = Visibility.Hidden;
                ModifyAddUserTextBlock.Visibility = Visibility.Hidden;
                UserModifyAddAccessLevelTextBlock.Visibility = Visibility.Hidden;
                UserModifyAddAccessLevelTextBox.Visibility = Visibility.Hidden;
                UserToModifyAddTextBox.Visibility = Visibility.Hidden;
                UserToAdjustAccessLevelTextBlock.Visibility = Visibility.Hidden;
            }
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
            // Retrieve information entered in the fields
            string currentPass = CurrentPasswordTextBox.Text;
            string newPass = NewPasswordTextBox.Text;
            string confirmedNewPass = ConfirmedNewPasswordTextBox.Text;

            // Validate new passwords match, show error if they don't
            if (newPass != confirmedNewPass)
            {
                NewPasswordMismatchTextBlock.Visibility = Visibility.Visible;
                return;
            }

            // Grab our user credentials file
            var credentials = File.ReadAllLines(AppBrain.brain.UserCredentialsFile);

            // Go through each user in the file, find the one that matches this user, validate their entered current
            // password actually matches their current password, if it does then update their password.
            for (int i = 0; i < credentials.Length; i++)
            {
                string[] userCredentials = credentials[i].Split(',');

                if ((userCredentials[0] == AppBrain.brain.Username) && (userCredentials[1] == currentPass))
                {
                    userCredentials[1] = confirmedNewPass;
                    string newCredentials = string.Join(",", userCredentials);
                    credentials[i] = newCredentials;
                    break;
                }
            }

            // Save changes to credentials file
            File.WriteAllLines(AppBrain.brain.UserCredentialsFile, credentials);
        }

        private void ModifyOrAddUser(object sender, RoutedEventArgs e)
        {
            // Grab information entered about the user
            string user = UserToModifyAddTextBox.Text;
            string accessLevel = UserModifyAddAccessLevelTextBox.Text;
            string password = ModifyAddUserPasswordTextBox.Text;

            // Clear errors because we haven't checked yet
            NoUserEnteredError.Visibility = Visibility.Hidden;
            TryingToModifySelfError.Visibility = Visibility.Hidden;
            NoModificationsMadeTextBlock.Visibility = Visibility.Hidden;

            // Make sure we're actually trying to modify or add someone and that it isn't ourself
            if (user == null)
            {
                NoUserEnteredError.Visibility = Visibility.Visible;
            }
            if (user == AppBrain.brain.Username)
            {
                TryingToModifySelfError.Visibility = Visibility.Visible;
            }
            if ((password == null) || (accessLevel == null))
            {
                NoModificationsMadeTextBlock.Visibility = Visibility.Visible;
            }

            // Grab our user credentials file
            var credentials = File.ReadAllLines(AppBrain.brain.UserCredentialsFile);
            bool userExists = false;

            // Check if the user exists, if they do update the appropriate values
            for (int i = 0; i < credentials.Length; i++)
            {
                string[] userCredentials = credentials[i].Split(',');

                if (userCredentials[0] == user)
                {
                    userExists = true;

                    if (password != null)
                    {
                        userCredentials[1] = password;
                    }
                    if (accessLevel != null)
                    {
                        userCredentials[2] = accessLevel;
                    }

                    string newCredentials = string.Join(",", userCredentials);
                    credentials[i] = newCredentials;
                    break;
                }
            }

            // User doesn't exist, create them
            if (!userExists)
            {
                var listOfCredentials = credentials.ToList();
                listOfCredentials.Add(user + "," + password + "," + accessLevel);
                credentials = listOfCredentials.ToArray();
            }

            // Save the changes
            File.WriteAllLines(AppBrain.brain.UserCredentialsFile, credentials);
        }
    }
}
