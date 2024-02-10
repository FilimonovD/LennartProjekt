using System;
// Weitere benötigte Using-Direktiven hier

namespace TorGuard
{
    public class Controller
    {
        private static AufnahmeKonfig konfig = new AufnahmeKonfig(8, 2, 0.5); // 24 Stunden pro Datei, 1 Tag Backup, halbstündige Segmente
        private static void InitializeTore()
        {
            tore.Add(new Tor(
            1
            , 11
            , 12
            , "rstp://192.168.1.100"
            , "rstp://192.168.1.100"
            , "/home/Tor.Guard/Videos/Tor.Guard/Tor_1/Cam_indoor"
            , "/home/Tor.Guard/Videos/Tor.Guard/Tor_1/Cam_Outdoor"
            , "/home/Tor.Guard/Videos/Tor.Guard/Crash"));

            //tore.Add(new Tor(2, 12, "camera_url_2.1", "camera_url_2.2", "/home/Tor.Guard/Videos/Tor.Guard/Tor_2/Cam_indoor", "/home/Tor.Guard/Videos/Tor.Guard/Tor_2/Cam_Outdor",false, DateTime.MinValue));

        }











        public EmailService EmailService { get; private set; }
        public WindowService WindowService { get; private set; }
        // Weitere Service-Instanzen hier, z.B. Konfig und TorListe

        public Controller()
        {
            //// Initialisieren Sie hier die Services
            //EmailService = new EmailService(/* Konfigurationsparameter falls benötigt */);
            //WindowService = new WindowService(/* Konfigurationsparameter falls benötigt */);
            //// Weitere Service-Initialisierungen hier
        }

        // Methode zum Versenden von E-Mails
        public async Task SendEmailsAsync(List<string> emailAddresses)
        {
            await EmailService.SendEmailsAsync(emailAddresses);
        }

        // Methode zum Laden des Zustands der Anwendung
        public void LoadApplicationState()
        {
            WindowService.LoadState();
            // Weitere Zustandsladungen hier
        }

        // Methode zum Speichern des Zustands der Anwendung
        public void SaveApplicationState()
        {
            WindowService.SaveState();
            // Weitere Zustandsspeicherungen hier
        }

        // Weitere Methoden zur Steuerung der Anwendung hier
    }
}
