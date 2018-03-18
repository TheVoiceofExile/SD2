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

        public CircuitBreaker()
        {

        }

        public bool IsRackedIn { get => isRackedIn; set => isRackedIn = value; }
        public bool IsOpen { get => isOpen; set => isOpen = value; }
        public bool IsTopComponent { get => isTopComponent; set => isTopComponent = value; }
    }
}
