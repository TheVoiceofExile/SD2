using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Timers;

namespace ServerApplication
{
    public sealed class AppBrain
    {
        public static AppBrain brain = new AppBrain();

        private string username = null;
        private string accessLevel = null;

        private List<Substation> listOfSubstations = new List<Substation>();

        private string userCredentialsFile = "Q:\\GitHub\\SD2\\ServerApplication\\ServerApplication\\ValidCredentials.csv";
        private string siteConfigurationFile = "Q:\\GitHub\\SD2\\ServerApplication\\ServerApplication\\SiteConfiguration.csv";

        private HttpClient httpClient = new HttpClient();

        private Timer refreshRate = new Timer();

        private string rackin = "rackin/";
        private string rackout = "rackout/";
        private string openBreaker = "openBreaker/";
        private string closeBreaker = "closeBreaker/";
        private string eStop = "estop/";



        private AppBrain()
        {
            // Hey don't add stuff here because race condition
            refreshRate.Elapsed += new ElapsedEventHandler(RefreshSite);
            refreshRate.Interval = 1000;
            refreshRate.Enabled = true;
        }

        private static async void RefreshSite(object source, ElapsedEventArgs e)
        {
            foreach (Substation ss in brain.listOfSubstations)
            {
                foreach (Switchgear sg in ss.Switchgears)
                {
                    foreach (Frame f in sg.Frames)
                    {
                        foreach (CircuitBreaker cb in f.CircuitBreakers)
                        {
                            //string open = await brain.HttpClient.GetStringAsync("http://127.0.0.1:8000/state/");
                            //string status = await brain.HttpClient.GetStringAsync("http://127.0.0.1:8000/");
                        }
                    }
                }
            }
        }

        // Populates the site configuration on the left side of the window
        public TreeView PopulateSiteConfiguration(TreeView SiteConfigurationTreeView)
        {
            var siteConfiguration = AppBrain.brain.SubStations;

            List<TreeViewItem> substations = new List<TreeViewItem>();
            List<TreeViewItem> switchgears = new List<TreeViewItem>();
            List<TreeViewItem> frames = new List<TreeViewItem>();

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

            substationMainHeader.ExpandSubtree();

            return SiteConfigurationTreeView;
        }

        public void LoadSiteConfiguration()
        {
            int currentLine = 0;

            var siteConfiguration = File.ReadAllLines(siteConfigurationFile);

            string[] substations = siteConfiguration[currentLine++].Split(',');
            string numSubstations = substations[1];

            for (int i = 0; i < Convert.ToInt32(numSubstations); i++)
            {
                string[] substation = siteConfiguration[currentLine++].Split(',');
                Substation ss = new Substation(substation[0]);

                for (int j = 0; j < Convert.ToInt32(substation[1]); j++)
                {
                    string[] switchgear = siteConfiguration[currentLine++].Split(',');
                    Switchgear sg = new Switchgear(switchgear[0]);
                    ss.Switchgears.Add(sg);

                    for (int k = 0; k < Convert.ToInt32(switchgear[1]); k++)
                    {
                        string[] frame = siteConfiguration[currentLine++].Split(',');
                        Frame f = new Frame(frame[0]);
                        sg.Frames.Add(f);

                        for (int w = 0; w < Convert.ToInt32(frame[1]); w++)
                        {
                            string[] breaker = siteConfiguration[currentLine++].Split(',');
                            CircuitBreaker cb = new CircuitBreaker(breaker[0], breaker[1], breaker[2]);
                            f.CircuitBreakers.Add(cb);
                        }
                    }
                }
                listOfSubstations.Add(ss);
            }
        }

        public string Username { get => username; set => username = value; }
        public string AccessLevel { get => accessLevel; set => accessLevel = value; }
        public List<Substation> SubStations { get => listOfSubstations; set => listOfSubstations = value; }
        public string UserCredentialsFile { get => userCredentialsFile; set => userCredentialsFile = value; }
        public string SiteConfigurationFile { get => siteConfigurationFile; set => siteConfigurationFile = value; }
        public HttpClient HttpClient { get => httpClient; set => httpClient = value; }
        public string EStop { get => eStop; set => eStop = value; }
        public string Rackout { get => rackout; set => rackout = value; }
        public string Rackin { get => rackin; set => rackin = value; }
        public string CloseBreaker { get => closeBreaker; set => closeBreaker = value; }
        public string OpenBreaker { get => openBreaker; set => openBreaker = value; }
    }
}
