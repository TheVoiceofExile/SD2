using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
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
        private string logFile = "Q:\\GitHub\\SD2\\ServerApplication\\ServerApplication\\Log.csv";

        private HttpClient httpClient = new HttpClient();

        private Timer refreshRate = new Timer();

        private string rackin = "rackin/";
        private string rackout = "rackout/";
        private string openBreaker = "breakerOpen/";
        private string closeBreaker = "breakerClose/";
        private string eStop = "estop/";

        private MainControlWindow mcw;
        private MainWindow mw;
        private ManageAccountWindow maw;
        private ManageSiteConfigurationWindow mscw;
        private ManageSwitchgearsWindow msgw;

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
                        for (int i = 0; i < f.CircuitBreakers.Count; i++)
                        {
                            if (f.CircuitBreakers[i].IpAddress != "169.254.130.91")
                            {
                                continue;
                            }

                            string response = await brain.httpClient.GetStringAsync("http://" + f.CircuitBreakers[i].IpAddress + ":8000/rackingsystem/");

                            List<JsonCircuitBreaker> jsonCircuitBreakers = JsonConvert.DeserializeObject<List<JsonCircuitBreaker>>(response);

                            f.CircuitBreakers[i].PrevState = f.CircuitBreakers[i].State;
                            f.CircuitBreakers[i].State = jsonCircuitBreakers[0].state;
                            f.CircuitBreakers[i].IsOpen = jsonCircuitBreakers[0].breaker;
                            f.CircuitBreakers[i].IsEStopped = jsonCircuitBreakers[0].EStop;
                            f.CircuitBreakers[i].IsRacking = jsonCircuitBreakers[0].racking;
                        }
                    }
                }
            }

            //if (statusUpdated)
            //{
            //    statusUpdated = false;
            //
            //    if (AppBrain.brain.mscw != null)
            //    {
            //        Application.Current.Dispatcher.Invoke((Action)delegate
            //        {
            //            AppBrain.brain.mscw.SiteConfigurationTreeView = new TreeView();
            //            AppBrain.brain.mscw.SiteConfigurationTreeView = AppBrain.brain.PopulateSiteConfiguration(AppBrain.brain.mscw.SiteConfigurationTreeView);
            //        });
            //    }
            //    else if (AppBrain.brain.msgw != null)
            //    {
            //        Application.Current.Dispatcher.Invoke((Action)delegate
            //        {
            //            AppBrain.brain.msgw.SiteConfigurationTreeView = new TreeView();
            //            AppBrain.brain.msgw.SiteConfigurationTreeView = AppBrain.brain.PopulateSiteConfiguration(AppBrain.brain.msgw.SiteConfigurationTreeView);
            //        });
            //    }
            //}
        }

        // Populates the site configuration on the left side of the window
        public TreeView PopulateSiteConfiguration(TreeView SiteConfigurationTreeView)
        {
            List<Substation> siteConfiguration = AppBrain.brain.SubStations;

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

                            string status = "";

                            if (cb.IsRacking && !cb.IsEStopped)
                            {
                                status = "Open and Racking";
                            }
                            else if (!cb.IsRacking && !cb.IsEStopped)
                            {
                                status = cb.IsOpen ? "Breaker Open " : "Breaker Closed ";

                                switch(cb.State)
                                {
                                    case 0:
                                        status = status + "and in Racked Out Position";
                                        break;
                                    case 1:
                                        status = status + "and In Test Position";
                                        break;
                                    case 2:
                                        status = status + "and in Racked In Position";
                                        break;
                                }
                            }
                            else if (!cb.IsRacking && cb.IsEStopped)
                            {
                                switch(cb.State)
                                {
                                    case 0:
                                        status = "ESTOP Active in Racked Out Position";
                                        break;
                                    case 1:
                                        status = "ESTOP Active in Test Position";
                                        break;
                                    case 2:
                                        status = "ESTOP Active in Racked In Position";
                                        break;
                                }

                                status += cb.IsOpen ? " and Breaker Open" : "and Breaker Closed";
                            }
                            else if (cb.IsRacking && cb.IsEStopped)
                            {
                                switch(cb.State)
                                {
                                    case 0:
                                        status = "ESTOP Active";
                                        break;
                                    case 1:
                                        status = "ESTOP Active";
                                        break;
                                    case 2:
                                        status = "ESTOP Active";
                                        break;
                                }
                            }

                            TreeViewItem circuitBreakerStatusHeader = new TreeViewItem
                            {
                                Header = "Breaker Status: " + status
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
        public MainControlWindow Mcw { get => mcw; set => mcw = value; }
        public MainWindow Mw { get => mw; set => mw = value; }
        public ManageAccountWindow Maw { get => maw; set => maw = value; }
        public ManageSiteConfigurationWindow Mscw { get => mscw; set => mscw = value; }
        public ManageSwitchgearsWindow Msgw { get => msgw; set => msgw = value; }
        public string LogFile { get => logFile; set => logFile = value; }
    }
}
