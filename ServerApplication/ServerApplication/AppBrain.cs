using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApplication
{
    public sealed class AppBrain
    {
        public static AppBrain brain = new AppBrain();

        private string username = null;
        private string accessLevel = null;

        private List<SubStation> subStations = new List<SubStation>();

        private string userCredentialsFile = "S:\\Repos\\SD2\\ServerApplication\\ServerApplication\\ValidCredentials.csv";

        private AppBrain()
        {

        }

        public string Username { get => username; set => username = value; }
        public string AccessLevel { get => accessLevel; set => accessLevel = value; }
        public List<SubStation> SubStations { get => subStations; set => subStations = value; }
        public string UserCredentialsFile { get => userCredentialsFile; set => userCredentialsFile = value; }
    }
}
