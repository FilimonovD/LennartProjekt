using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorGuard
{
    public class AufnahmeKonfig
    // Konfigurationsvariablen
    {
        public double StundenProDatei { get; set; } // Länge eines Aufnahmezyklus in Stunde
        public double BackupDays { get; set; } // Anzahl der Tage, die als Backup gehalten werden sollen
        public double SpeicherIntervallStunden { get; set; } // Speicherintervall für eine Bakup speichung (Server)in Stunden, hier halbstündlich

        public AufnahmeKonfig(double stundenProDatei, double backupDays, double speicherIntervallStunden)
        {
            StundenProDatei = stundenProDatei;
            BackupDays = backupDays;
            SpeicherIntervallStunden = speicherIntervallStunden;
        }
    }


}
