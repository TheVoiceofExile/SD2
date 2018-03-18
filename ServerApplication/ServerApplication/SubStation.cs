using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApplication
{
    public class Substation
    {
        // Substations have switchgears
        List<Switchgear> switchgears = new List<Switchgear>();
        string substationName = null;

        public Substation(string name)
        {
            substationName = name;
        }

        public List<Switchgear> Switchgears { get => switchgears; set => switchgears = value; }
    }
}
