using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorGuard
{
    internal class HC_Tore
    {
        public string TorName { get; set; }
        public int GpioPinIndoor { get; set; }
        public int GpioPinOutdoor { get; set; }
        public DateTime LastHigh { get; set; } = DateTime.MinValue; // Standardwert
        public bool IsHigh { get; set; } = false; // Standardwert
        public string KameraIndoorUrl { get; set; }
        public string KameraOutdoorUrl { get; set; }
        public string SpeicherortKameraIndoor { get; set; }
        public string SpeicherortKameraOutdoor { get; set; }
        public string SpeicherortCrash { get; set; }
        public bool IsHighIndoor { get; set; }
        public bool IsHighOutdoor { get; set; }
        public DateTime LastHighTimeIndoor { get; set; }
        public DateTime LastHighTimeOutdoor { get; set; }

        public HC_Tore(string torName, int gpioPinIndoor, int gpioPinOutdoor, string kameraIndoorUrl, string kameraOutdoorUrl, string speicherortKameraIndoor, string speicherortKameraOutdoor, string speicherortCrash)
        {
            TorName = torName;
            GpioPinIndoor = gpioPinIndoor;
            GpioPinOutdoor = gpioPinOutdoor;
            KameraIndoorUrl = kameraIndoorUrl;
            KameraOutdoorUrl = kameraOutdoorUrl;
            SpeicherortKameraIndoor = speicherortKameraIndoor;
            SpeicherortKameraOutdoor = speicherortKameraOutdoor;
            SpeicherortCrash = speicherortCrash;
        }
    }
}
