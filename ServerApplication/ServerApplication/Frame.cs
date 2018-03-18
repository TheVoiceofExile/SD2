using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApplication
{
    public class Frame
    {
        // Frames hold circuit breakers, sometimes
        List<CircuitBreaker> circuitBreakers = new List<CircuitBreaker>();
        string frameName = null;

        public Frame(string name)
        {
            frameName = name;
        }

        public List<CircuitBreaker> CircuitBreakers { get => circuitBreakers; set => circuitBreakers = value; }
        public string FrameName { get => frameName; set => frameName = value; }
    }
}
