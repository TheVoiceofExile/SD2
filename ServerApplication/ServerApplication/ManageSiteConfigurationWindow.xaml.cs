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
    /// <summary>
    /// Interaction logic for ManageSiteConfigurationWindow.xaml
    /// </summary>
    public partial class ManageSiteConfigurationWindow : Window
    {
        private List<TreeViewItem> substations = new List<TreeViewItem>();
        private List<TreeViewItem> switchgears = new List<TreeViewItem>();
        private List<TreeViewItem> frames = new List<TreeViewItem>();

        public ManageSiteConfigurationWindow()
        {
            InitializeComponent();
            PopulateSiteConfiguration();
        }

        private void PopulateSiteConfiguration()
        {
            int currentLine = 0;

            var siteConfiguration = File.ReadAllLines(AppBrain.brain.SiteConfigurationFile);

            string[] numSubStations = siteConfiguration[currentLine].Split(',');

            TreeViewItem substationMainHeader = new TreeViewItem
            {
                Header = numSubStations[0]
            };
            SiteConfigurationTreeView.Items.Add(substationMainHeader);

            currentLine++;

            for (int i = 0; i < Convert.ToInt32(numSubStations[1]); i++)
            {
                string[] substation = siteConfiguration[currentLine++].Split(',');

                TreeViewItem substationHeader = new TreeViewItem
                {
                    Header = substation[0]
                };
                substationMainHeader.Items.Add(substationHeader);

                substations.Add(substationHeader);

                for (int j = 0; j < Convert.ToInt32(substation[1]); j++)
                {
                    string[] switchgear = siteConfiguration[currentLine++].Split(',');

                    TreeViewItem switchgearHeader = new TreeViewItem
                    {
                        Header = switchgear[0]
                    };
                    substationHeader.Items.Add(switchgearHeader);

                    switchgears.Add(switchgearHeader);

                    for (int k = 0; k < Convert.ToInt32(switchgear[1]); k++)
                    {
                        string[] frame = siteConfiguration[currentLine++].Split(',');

                        TreeViewItem frameHeader = new TreeViewItem
                        {
                            Header = frame[0] + ":" + frame[1]
                        };
                        switchgearHeader.Items.Add(frameHeader);
                    }
                }
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

        private void RemoveSiteComponent(object sender, RoutedEventArgs e)
        {
            var siteConfiguration = File.ReadAllLines(AppBrain.brain.SiteConfigurationFile);


        }
    }
}
