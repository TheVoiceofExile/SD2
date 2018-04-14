using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ServerApplication
{
    public class CircuitBreaker
    {
        private bool isTopComponent = false;

        private string ipAddress = null;
        private int state = 0;
        private string breakerName = null;
        private bool isOpen = false;
        private bool isRacking = false;
        private bool isEStopped = false;
        private int prevState = 0;


        public CircuitBreaker(string name, string location, string ip)
        {
            this.BreakerName = name;
            IpAddress1 = ip;
            if (location == "Top")
            {
                IsTopComponent = true;
            }
            else
            {
                IsTopComponent = false;
            }
        }

        public string IpAddress { get => IpAddress1; set => IpAddress1 = value; }
        public bool IsTopComponent { get => isTopComponent; set => isTopComponent = value; }
        public string IpAddress1 { get => ipAddress; set => ipAddress = value; }
        public string BreakerName { get => breakerName; set => breakerName = value; }
        public int State { get => state; set => state = value; }
        public bool IsOpen { get => isOpen; set => isOpen = value; }
        public bool IsRacking { get => isRacking; set => isRacking = value; }
        public bool IsEStopped { get => isEStopped; set => isEStopped = value; }
        public int PrevState { get => prevState; set => prevState = value; }
    }

    [JsonObject]
    public class JsonCircuitBreaker
    {
        public string name { get; set; }
        public int frame { get; set; }
        public bool compartment { get; set; }
        public int state { get; set; }
        public bool breaker { get; set; }
        public bool EStop { get; set; }
        public bool racking { get; set; }
    }
}
