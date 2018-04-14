using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ServerApplication
{
    /// <summary>
    /// Interaction logic for ManageSwitchgearsWindow.xaml
    /// </summary>
    public partial class ManageSwitchgearsWindow : Window
    {
        public ManageSwitchgearsWindow()
        {
            InitializeComponent();
            AppBrain.brain.Msgw = this;
            LoggedInAsLabel.Text = "Current User: " + AppBrain.brain.Username;
            AccessLevelTextBlock.Text = "Access Level: " + AppBrain.brain.AccessLevel;
            SiteConfigurationTreeView = AppBrain.brain.PopulateSiteConfiguration(SiteConfigurationTreeView);
        }

        // Return back to the control panel window
        private void ControlPanelWindow(object sender, RoutedEventArgs e)
        {
            MainControlWindow controlPanelWindow = new MainControlWindow();
            controlPanelWindow.Show();
            AppBrain.brain.Msgw = null;
            this.Close();
        }

        // Log out from current user
        private void Logout(object sender, RoutedEventArgs e)
        {
            MainWindow loginWindow = new MainWindow();
            loginWindow.Show();
            AppBrain.brain.Msgw = null;
            this.Close();
        }

        // Finds the parents of the provided selected item from the treeview
        private List<TreeViewItem> GetParentsOfSelected(List<TreeViewItem> parents, TreeViewItem selected)
        {
            DependencyObject selectedParent = VisualTreeHelper.GetParent(selected);

            ItemsControl currentParent;

            TreeViewItem parent = selected;

            parents.Add(selected);

            while (parent.Header.ToString() != "Site")
            {
                while (!(selectedParent is TreeViewItem || selectedParent is TreeView))
                {
                    selectedParent = VisualTreeHelper.GetParent(selectedParent);
                }

                currentParent = selectedParent as ItemsControl;

                parent = currentParent as TreeViewItem;

                selectedParent = VisualTreeHelper.GetParent(parent);

                parents.Add(parent);
            }

            parents.Reverse();

            return parents;
        }

        // Reacts to the send command button press and sends the command of whatever radio button was selected.
        private void SendCommand(object sender, RoutedEventArgs e)
        {
            string command = "";

            if ((bool)TestBreakerRadioButton.IsChecked)
            {
                command = AppBrain.brain.CloseBreaker;
            }
            if ((bool)RackInRadioButton.IsChecked)
            {
                command = AppBrain.brain.Rackin;
            }
            if ((bool)RackOutRadioButton.IsChecked)
            {
                command = AppBrain.brain.Rackout;
            }

            SendCommands(command);
        }

        // Helper method to send the commands
        private async void SendCommands(string command)
        {
            List<string[]> breakerParents = BreakdownBreakerString();

            List<string> IP = GetIPs(breakerParents);

            foreach (string ip in IP)
            {
                var responseString = await AppBrain.brain.HttpClient.GetStringAsync("http://" + ip + ":8000/" + command);
                ResponseTextBox.Text = responseString;

                if ((bool)TestBreakerRadioButton.IsChecked)
                {
                    var secondResponse = await AppBrain.brain.HttpClient.GetStringAsync(ip + AppBrain.brain.OpenBreaker);
                }
            }
        }

        // Breaks the string in the list box that shows which breakers will have the command sent to into a
        // format that can be used to retrieve the IP
        private List<string[]> BreakdownBreakerString()
        {
            List<string[]> breakers = new List<string[]>();

            for (int i = 0; i < BreakersToCommandListBox.Items.Count; i++)
            {
                string currentBreaker = BreakersToCommandListBox.Items[i].ToString();

                string[] splitBreaker = currentBreaker.Split('-');

                for (int j = 0; j < splitBreaker.Length; j++)
                {
                    splitBreaker[j] = splitBreaker[j].TrimEnd(' ');
                    splitBreaker[j] = splitBreaker[j].TrimStart(' ');
                }

                breakers.Add(splitBreaker);
            }

            return breakers;
        }

        // Using the broken down strings from the list box finds the IP addresses of the breakers
        // that are listed there.
        private List<string> GetIPs(List<string[]> breakers)
        {
            List<string> ipAddresses = new List<string>();

            TreeViewItem selected = SiteConfigurationTreeView.SelectedItem as TreeViewItem;

            foreach (string[] breaker in breakers)
            {
                foreach (Substation ss in AppBrain.brain.SubStations)
                {
                    if (ss.SubstationName == breaker[2])
                    {
                        foreach (Switchgear sg in ss.Switchgears)
                        {
                            if (sg.SwitchgearName == breaker[3])
                            {
                                foreach (Frame f in sg.Frames)
                                {
                                    if (f.FrameName == breaker[4])
                                    {
                                        foreach (CircuitBreaker cb in f.CircuitBreakers)
                                        {
                                            if (cb.BreakerName == breaker[5])
                                            {
                                                ipAddresses.Add(cb.IpAddress);
                                                break;
                                            }
                                        }

                                        break;
                                    }
                                }

                                break;
                            }
                        }

                        break;
                    }
                }
            }

            return ipAddresses;
        }

        // Adds the breaker selected to the list of breakers to issue commands to
        private void AddBreakerButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedBreaker = "";
            List<TreeViewItem> parents = new List<TreeViewItem>();

            parents = GetParentsOfSelected(parents, SiteConfigurationTreeView.SelectedItem as TreeViewItem);

            if (parents.Count == 5)
            {
                foreach (TreeViewItem parent in parents)
                {
                    selectedBreaker = selectedBreaker + " - " + parent.Header;
                }

                BreakersToCommandListBox.Items.Add(selectedBreaker);
            }
        }

        // Removes a breaker from the list of breakers to issue commands to
        private void RemoveBreakerButton_Click(object sender, RoutedEventArgs e)
        {
            BreakersToCommandListBox.Items.Remove(BreakersToCommandListBox.SelectedItem);
        }

        // Activates Estop on all listed breakers
        private void Estop(object sender, RoutedEventArgs e)
        {
            SendCommands(AppBrain.brain.EStop);
        }
    }
}
