using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ServerApplication
{
    public sealed class AppBrain
    {
        public static AppBrain brain = new AppBrain();

        private string username = null;
        private string accessLevel = null;

        private List<Substation> listOfSubstations = new List<Substation>();

        private string userCredentialsFile = "S:\\Repos\\SD2\\ServerApplication\\ServerApplication\\ValidCredentials.csv";
        private string siteConfigurationFile = "S:\\Repos\\SD2\\ServerApplication\\ServerApplication\\SiteConfiguration.csv";

        private AppBrain()
        {
            // Hey don't add stuff here because race condition
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
    }
}
