using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApplication
{
    public class CircuitBreaker
    {
        private bool isRackedIn = false;
        private bool isOpen = false;
        private bool isTopComponent = false;
        private string breakerName = null;
        private string ipAddress = null;

        public CircuitBreaker(string name, string location, string ip)
        {
            breakerName = name;
            ipAddress = ip;
            if (location == "Top")
            {
                isTopComponent = true;
            }
            else
            {
                isTopComponent = false;
            }
        }

        public bool IsRackedIn { get => isRackedIn; set => isRackedIn = value; }
        public bool IsOpen { get => isOpen; set => isOpen = value; }
        public bool IsTopComponent { get => isTopComponent; set => isTopComponent = value; }
        public string BreakerName { get => breakerName; set => breakerName = value; }
        public string IpAddress { get => ipAddress; set => ipAddress = value; }
    }
}
