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
using System.Net.Http;

namespace ServerApplication
{
    /// <summary>
    /// Interaction logic for ManageSwitchgearsWindow.xaml
    /// </summary>
    public partial class ManageSwitchgearsWindow : Window
    {
        private List<TreeViewItem> substations = new List<TreeViewItem>();
        private List<TreeViewItem> switchgears = new List<TreeViewItem>();
        private List<TreeViewItem> frames = new List<TreeViewItem>();

        public ManageSwitchgearsWindow()
        {
            InitializeComponent();
            LoggedInAsLabel.Text = "Current User: " + AppBrain.brain.Username;
            AccessLevelTextBlock.Text = "Access Level: " + AppBrain.brain.AccessLevel;
            PopulateSiteConfiguration();
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

        private void PopulateSiteConfiguration()
        {
            var siteConfiguration = AppBrain.brain.SubStations;

            TreeViewItem substationMainHeader = new TreeViewItem
            {
                Header = "Site"
            };
            SiteConfigurationTreeView.Items.Add(substationMainHeader);

            foreach (Substation ss in AppBrain.brain.SubStations)
            {
                TreeViewItem substationHeader = new TreeViewItem
                {
                    Header = ss.SubstationName
                };
                substationMainHeader.Items.Add(substationHeader);

                substations.Add(substationHeader);

                foreach (Switchgear sg in ss.Switchgears)
                {
                    TreeViewItem switchgearHeader = new TreeViewItem
                    {
                        Header = sg.SwitchgearName
                    };
                    substationHeader.Items.Add(switchgearHeader);

                    switchgears.Add(switchgearHeader);

                    foreach (Frame f in sg.Frames)
                    {
                        TreeViewItem frameHeader = new TreeViewItem
                        {
                            Header = f.FrameName
                        };
                        switchgearHeader.Items.Add(frameHeader);

                        foreach (CircuitBreaker cb in f.CircuitBreakers)
                        {
                            TreeViewItem circuitBreakerHeader = new TreeViewItem
                            {
                                Header = cb.BreakerName
                            };
                            frameHeader.Items.Add(circuitBreakerHeader);

                            TreeViewItem circuitBreakerStatusHeader = new TreeViewItem
                            {
                                Header = "Breaker Status: "
                            };

                            circuitBreakerHeader.Items.Add(circuitBreakerStatusHeader);

                            TreeViewItem circuitBreakerIPHeader = new TreeViewItem
                            {
                                Header = "IP Address: " + cb.IpAddress
                            };

                            circuitBreakerHeader.Items.Add(circuitBreakerIPHeader);

                            if (cb.IsTopComponent)
                            {
                                TreeViewItem circuitBreakerLocationHeader = new TreeViewItem
                                {
                                    Header = "Location: Top"
                                };
                                circuitBreakerHeader.Items.Add(circuitBreakerLocationHeader);
                            }
                            else
                            {
                                TreeViewItem circuitBreakerLocationHeader = new TreeViewItem
                                {
                                    Header = "Location: Bottom"
                                };
                                circuitBreakerHeader.Items.Add(circuitBreakerLocationHeader);
                            }
                        }
                    }
                }
            }
        }

        ////////////////////////////////////////
        // Invalid if selected is Bruce Wayne //
        ////////////////////////////////////////
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

        private async void SendCommand(object sender, RoutedEventArgs e)
        {
            string command = "";

            List<TreeViewItem> parents;

            if ((bool)TestBreakerRadioButton.IsChecked)
            {
                command = AppBrain.brain.OpenBreaker;
                var responseString = await AppBrain.brain.HttpClient.GetStringAsync(AppBrain.brain.Pi + AppBrain.brain.Rackin);
                ResponseTextBox.Text = responseString;
            }
            if ((bool)RackInRadioButton.IsChecked)
            {
                command = AppBrain.brain.Rackin;
                var responseString = await AppBrain.brain.HttpClient.GetStringAsync(AppBrain.brain.Pi + AppBrain.brain.Rackin);
                ResponseTextBox.Text = responseString;
            }
            if ((bool)RackOutRadioButton.IsChecked)
            {
                command = AppBrain.brain.Rackout;
                var responseString = await AppBrain.brain.HttpClient.GetStringAsync(AppBrain.brain.Pi + AppBrain.brain.Rackin);
                ResponseTextBox.Text = responseString;
            }

            for (int i = 0; i < BreakersToCommandListBox.Items.Count; i++)
            {
                //var responseString = await AppBrain.brain.HttpClient.GetStringAsync("http://127.0.0.1:8000/" +
            }

            if ((bool)TestBreakerRadioButton.IsChecked)
            {
                command = AppBrain.brain.CloseBreaker;

                for (int i = 0; i < BreakersToCommandListBox.Items.Count; i++)
                {
                    //var responseString = await AppBrain.brain.HttpClient.GetStringAsync("http://127.0.0.1:8000/" +);
                }
            }
        }

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

        private void RemoveBreakerButton_Click(object sender, RoutedEventArgs e)
        {
            BreakersToCommandListBox.Items.Remove(BreakersToCommandListBox.SelectedItem);
        }

        private async void sendrackin(object sender, RoutedEventArgs e)
        {
            var uri = new Uri("http://169.254.130.91:8000/rackin/");
            var responseString = await AppBrain.brain.HttpClient.GetStringAsync(uri);
            ResponseTextBox.Text = responseString;
        }

        private async void sendrackout(object sender, RoutedEventArgs e)
        {
            var uri = new Uri("http://169.254.130.91:8000/rackout/");
            var responseString = await AppBrain.brain.HttpClient.GetStringAsync(uri);
            ResponseTextBox.Text = responseString;
        }

        private async void sendopenbreaker(object sender, RoutedEventArgs e)
        {
            var uri = new Uri("http://169.254.130.91:8000/breakerOpen/");
            var responseString = await AppBrain.brain.HttpClient.GetStringAsync(uri);
            ResponseTextBox.Text = responseString;
        }

        private async void sendclosebreaker(object sender, RoutedEventArgs e)
        {
            var uri = new Uri("http://169.254.130.91:8000/breakerClose/");
            var responseString = await AppBrain.brain.HttpClient.GetStringAsync(uri);
            ResponseTextBox.Text = responseString;
        }
    }
}
