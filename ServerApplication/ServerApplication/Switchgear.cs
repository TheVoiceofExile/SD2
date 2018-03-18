using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApplication
{
    public class Switchgear
    {
        // Switchgears are made of frames
        List<Frame> frames = new List<Frame>();
        string switchgearName = null;

        public Switchgear(string name)
        {
            switchgearName = name;
        }

        public List<Frame> Frames { get => frames; set => frames = value; }
    }
}
