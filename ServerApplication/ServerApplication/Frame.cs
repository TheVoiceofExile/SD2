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

        public Frame()
        {

        }
    }
}
