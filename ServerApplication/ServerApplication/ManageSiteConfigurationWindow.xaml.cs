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
using System.IO;

namespace ServerApplication
{
    public partial class ManageSiteConfigurationWindow : Window
    {
        private List<TreeViewItem> substations = new List<TreeViewItem>();
        private List<TreeViewItem> switchgears = new List<TreeViewItem>();
        private List<TreeViewItem> frames = new List<TreeViewItem>();

        public ManageSiteConfigurationWindow()
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

        private void RemoveSiteComponent(object sender, RoutedEventArgs e)
        {
            List<TreeViewItem> parents = new List<TreeViewItem>();

            TreeViewItem selected = SiteConfigurationTreeView.SelectedItem as TreeViewItem;

            parents = GetParentsOfSelected(parents, selected);

            if (parents.Count == 0)
            {
                AppBrain.brain.SubStations = new List<Substation>();
                UpdateSiteConfiguration();
                return;
            }

            // Once we remove whatever we're after use this flag to break out of all the loops
            bool removedSelected = false;

            // Whoever finds this giant mess and has to work on it, I'm sorry.
            foreach (Substation ss in AppBrain.brain.SubStations)
            {
                if (removedSelected)
                    break;

                if (ss.SubstationName == selected.Header.ToString()) // If we're removing a substation and the name matches, remove it and break out
                {
                    AppBrain.brain.SubStations.Remove(ss);
                    removedSelected = true;
                    break;
                }

                if (ss.SubstationName == parents[1].Header.ToString()) // If we're not deleting a substation but it is a parent of what we're deleting, we must go deeper
                {
                    foreach (Switchgear sg in ss.Switchgears)
                    {
                        if (removedSelected)
                            break;

                        if (sg.SwitchgearName == selected.Header.ToString()) // If we're removing a switchgear from a substation and the name matches, remove it and break out
                        {
                            ss.Switchgears.Remove(sg);
                            removedSelected = true;
                            break;
                        }

                        if (sg.SwitchgearName == parents[2].Header.ToString()) // If we're not deleting a switchgear but it's a parent of what we're deleting, we must go deeper
                        {
                            foreach (Frame f in sg.Frames)
                            {
                                if (removedSelected)
                                    break;

                                if (f.FrameName == selected.Header.ToString()) // If we're removing a frame from a switchgear and the name matches, remove it and break out
                                {
                                    sg.Frames.Remove(f);
                                    removedSelected = true;
                                    break;
                                }

                                if (f.FrameName == parents[3].Header.ToString()) //  If we're not deleting a frame but it's a parent of what we're deleting, we must go deeper
                                {
                                    foreach (CircuitBreaker cb in f.CircuitBreakers)
                                    {
                                        if (cb.BreakerName == parents[4].Header.ToString()) // IF we're removing a circuit breaker from a frame and the name matches, remove it and break out
                                        {
                                            f.CircuitBreakers.Remove(cb);
                                            removedSelected = true;
                                            break;
                                        }
                                        // If we reach here and haven't deleted something/broken out, someone is probably hacking the application
                                    }
                                }
                            }
                        }
                    }
                }
            }

            UpdateSiteConfiguration();
            PopulateSiteConfiguration();
        }

        ////////////////////////////////////////
        // Invalid if selected is Bruce Wayne //
        ////////////////////////////////////////
        private List<TreeViewItem> GetParentsOfSelected(List<TreeViewItem> parents, TreeViewItem selected)
        {
            DependencyObject selectedParent = VisualTreeHelper.GetParent(selected);

            ItemsControl currentParent;

            TreeViewItem parent = selected;

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

        private void UpdateSiteConfiguration()
        {
            List<string> newSiteConfiguration = new List<string>
            {
                "Substations," + AppBrain.brain.SubStations.Count
            };

            foreach (Substation ss in AppBrain.brain.SubStations)
            {
                newSiteConfiguration.Add(ss.SubstationName + "," + ss.Switchgears.Count);

                foreach (Switchgear sg in ss.Switchgears)
                {
                    newSiteConfiguration.Add(sg.SwitchgearName + "," + sg.Frames.Count);

                    foreach (Frame f in sg.Frames)
                    {
                        newSiteConfiguration.Add(f.FrameName + "," + f.CircuitBreakers.Count);

                        foreach (CircuitBreaker cb in f.CircuitBreakers)
                        {
                            if (cb.IsTopComponent)
                                newSiteConfiguration.Add(cb.BreakerName + ",Top," + cb.IpAddress);
                            else
                                newSiteConfiguration.Add(cb.BreakerName + ",Bottom," + cb.IpAddress);
                        }
                    }
                }
            }

            File.WriteAllLines(AppBrain.brain.SiteConfigurationFile, newSiteConfiguration);
        }

        private void AddSubstation(object sender, RoutedEventArgs e)
        {
            List<TreeViewItem> parents = new List<TreeViewItem>();

            TreeViewItem selected = SiteConfigurationTreeView.SelectedItem as TreeViewItem;

            parents = GetParentsOfSelected(parents, selected);

            Substation substation = new Substation(SubstationNameTextBox.Text);

            AddComponentToSite(substation, null, null, null, parents, selected);
        }

        private void AddSwitchgear(object sender, RoutedEventArgs e)
        {
            List<TreeViewItem> parents = new List<TreeViewItem>();

            TreeViewItem selected = SiteConfigurationTreeView.SelectedItem as TreeViewItem;

            parents = GetParentsOfSelected(parents, selected);

            parents.Add(SiteConfigurationTreeView.SelectedItem as TreeViewItem);

            Switchgear switchgear = new Switchgear(SwitchgearNameTextBox.Text);

            AddComponentToSite(null, switchgear, null, null, parents, selected);
        }

        private void AddFrame(object sender, RoutedEventArgs e)
        {
            List<TreeViewItem> parents = new List<TreeViewItem>();

            TreeViewItem selected = SiteConfigurationTreeView.SelectedItem as TreeViewItem;

            parents = GetParentsOfSelected(parents, selected);

            parents.Add(SiteConfigurationTreeView.SelectedItem as TreeViewItem);

            Frame frame = new Frame(FrameNameTextBox.Text);

            AddComponentToSite(null, null, frame, null, parents, selected);
        }

        private void AddCircuitBreaker(object sender, RoutedEventArgs e)
        {
            List<TreeViewItem> parents = new List<TreeViewItem>();

            TreeViewItem selected = SiteConfigurationTreeView.SelectedItem as TreeViewItem;

            parents = GetParentsOfSelected(parents, selected);

            parents.Add(SiteConfigurationTreeView.SelectedItem as TreeViewItem);

            CircuitBreaker circuitBreaker;

            if ((bool)(CircuitBreakerTopComponentCheckBox.IsChecked))
            {
                circuitBreaker = new CircuitBreaker(CircuitBreakerNameTextBox.Text, "Top", CircuitBreakerIPTextBox.Text);
            }
            else
            {
                circuitBreaker = new CircuitBreaker(CircuitBreakerNameTextBox.Text, "Bottom", CircuitBreakerIPTextBox.Text);
            }

            AddComponentToSite(null, null, null, circuitBreaker, parents, selected);
        }

        private void AddComponentToSite(
            Substation substation,
            Switchgear switchgear,
            Frame frame,
            CircuitBreaker circuitBreaker,
            List<TreeViewItem> parents,
            TreeViewItem selected)
        {
            bool addedComponent = false;

            // Whoever finds this giant mess and has to work on it, I'm sorry.
            foreach (Substation ss in AppBrain.brain.SubStations)
            {
                if (addedComponent)
                    break;

                if (substation != null) // If we're removing a substation and the name matches, remove it and break out
                {
                    AppBrain.brain.SubStations.Add(substation);
                    addedComponent = true;
                    break;
                }

                if (ss.SubstationName == parents[1].Header.ToString()) // If we're not deleting a substation but it is a parent of what we're deleting, we must go deeper
                {
                    if (ss.Switchgears.Count == 0)
                    {
                        ss.Switchgears.Add(switchgear);
                        addedComponent = true;
                        break;
                    }
                    foreach (Switchgear sg in ss.Switchgears)
                    {
                        if (addedComponent)
                            break;

                        if (switchgear != null) // If we're removing a switchgear from a substation and the name matches, remove it and break out
                        {
                            ss.Switchgears.Add(switchgear);
                            addedComponent = true;
                            break;
                        }

                        if (sg.SwitchgearName == parents[2].Header.ToString()) // If we're not deleting a switchgear but it's a parent of what we're deleting, we must go deeper
                        {
                            if (sg.Frames.Count == 0)
                            {
                                sg.Frames.Add(frame);
                                addedComponent = true;
                                break;
                            }
                            foreach (Frame f in sg.Frames)
                            {
                                if (addedComponent)
                                    break;

                                if (frame != null) // If we're removing a frame from a switchgear and the name matches, remove it and break out
                                {
                                    sg.Frames.Add(frame);
                                    addedComponent = true;
                                    break;
                                }

                                if (f.FrameName == parents[3].Header.ToString()) //  If we're not deleting a frame but it's a parent of what we're deleting, we must go deeper
                                {
                                    if (f.CircuitBreakers.Count == 0)
                                    {
                                        f.CircuitBreakers.Add(circuitBreaker);
                                        addedComponent = true;
                                        break;
                                    }
                                    foreach (CircuitBreaker cb in f.CircuitBreakers)
                                    {
                                        if (circuitBreaker != null) // IF we're removing a circuit breaker from a frame and the name matches, remove it and break out
                                        {
                                            f.CircuitBreakers.Add(circuitBreaker);
                                            addedComponent = true;
                                            break;
                                        }
                                        // If we reach here and haven't deleted something/broken out, someone is probably hacking the application
                                    }
                                }
                            }
                        }
                    }
                }
            }

            UpdateSiteConfiguration();
            PopulateSiteConfiguration();
        }
    }
}
