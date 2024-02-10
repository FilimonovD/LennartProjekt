


using System;

using System.Diagnostics;

using System.IO;
using System.Threading;

//using Iot.Device.Gpio;

using System.Device.Gpio;

using System.Collections.Generic;
using Microsoft.VisualBasic;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace TorGuard
{
    class Aufnahme
    {
        private Dictionary<string, Tor> torDictionary;

        public Aufnahme(Dictionary<string, Tor> torDictionary)
        {
            this.torDictionary = torDictionary;
        }

        //erstellen eines Datei Namens
        private static string GenerateFileName(Tor tor, bool isIndoor)//Erstelle Dateiname TorNummer seite des Tors Crash? 
        {
            string seite = isIndoor ? "Indoor" : "Outdoor";
            string fileName = $"Tor{tor.TorName}_{seite}_{DateTime.Now:yyyyMMddHHmmss}.mp4";


            return (fileName);// Datei Name 

            // ergänzen von wann bis wann
        }
        //private static string GeneratestorgePath(Tor tor, bool isIndoor, bool Crash)//Erstelle Dateiname TorNummer seite des Tors Crash? 
        //{
        //    string storagePath = isIndoor ? tor.SpeicherortKameraIndoor : tor.SpeicherortKameraOutdoor;

        //    //Directory.CreateDirectory(storagePath); // Erstellen des Ordners, falls nicht vorhanden
        //    return (storagePath); //Speicher Ort
        //}



        // Starte/Stopen der aufnahme u. voreinstellungen
        private static async Task ContinuousRecordingAsync()
        {
            while (true) //(wenn kamera Angeschlossen und erreichbar ) Ersetze true durch eine geeignete Bedingung, um die Schleife zu beenden, falls nötig.
            {
                foreach (var tor in tore)
                {

                    string fileNameIndoor = GenerateFileName(tor, true); //Datenpfad für Indoor Video
                    string fileNameOutdoor = GenerateFileName(tor, false); // Outdoor für Datenpfad  Video

                    Task indoorRecordingTask = StartFfmpegRecordingAsync(tor.KameraIndoorUrl, fileNameIndoor, tor.SpeicherortKameraIndoor);
                    Task outdoorRecordingTask = StartFfmpegRecordingAsync(tor.KameraOutdoorUrl, fileNameOutdoor,tor.SpeicherortKameraOutdoor);

                    await Task.WhenAll(indoorRecordingTask, outdoorRecordingTask);

                }

                // Wartezeit zwischen den Aufnahmen, falls erforderlich.
                await Task.Delay(TimeSpan.FromMinutes(0.5)); // Beispielsweise 10 sekunden Wartezeit.

            }

        }


        //Aufnahme u.Speichern
        private static async Task StartFfmpegRecordingAsync(string cameraUrl, string aktuellefileName, string storagePath)
        {

            string fileName = Path.Combine(storagePath, aktuellefileName); // Kombiniert Speicherort mit dem Dateinamen

            string ffmpegArgs = $"-rtsp_transport tcp -i {cameraUrl} -c copy -map 0 {fileName}";

            using (Process ffmpegProcess = new Process())
            {
                //Konfiguartion der aufnaheme

                ffmpegProcess.StartInfo.FileName = "ffmpeg";
                ffmpegProcess.StartInfo.Arguments = ffmpegArgs;
                ffmpegProcess.StartInfo.UseShellExecute = false;
                ffmpegProcess.StartInfo.CreateNoWindow = true;
                ffmpegProcess.StartInfo.RedirectStandardOutput = true;
                ffmpegProcess.StartInfo.RedirectStandardError = true;

                ffmpegProcess.Start();

                // Warte die konfigurierte Dauer, bevor der Prozess gestoppt wird.
                await Task.Delay(TimeSpan.FromHours(konfig.StundenProDatei));

                // Stoppe den ffmpeg-Prozess sanft, um die Aufnahme ordnungsgemäß zu beenden.
                if (!ffmpegProcess.HasExited)
                {
                    ffmpegProcess.Kill();
                }

                DeleteOldRecordings(storagePath);

            }
            
        }
        //Löschen von alten Datein     
        private static void DeleteOldRecordings(string storagePath)
        //alte Aufnahmen löschen, um Speicherplatz freizugeben und die Einhaltung der Backup-Strategie
        {

            try
            {
                var directoryInfo = new DirectoryInfo(storagePath);

                if (directoryInfo.Exists)
                {
                    


                    // Verwenden Sie backupDays anstelle von AufnahmeKonfig.BackupDays in Ihrem Code
                    var oldFiles = directoryInfo.GetFiles("*.mp4")
                                                .Where(file => file.CreationTime < DateTime.Now.AddDays(-konfig.BackupDays))
                                                .ToList();

                    foreach (var file in oldFiles)
                    {

                        file.Delete();

                        Console.WriteLine($"Gelöschte alte Aufnahme: {file.Name}");

                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Löschen alter Aufnahmen: {ex.Message}");
            }
        }
        //Reaktion auf einene Unfall
        private static void HandleCrashDetection(Tor tor, bool isIndoor) //für speichern auf PC änderung nötig !!!!
        {
            // Speichern des Indoor-Crash-Videos
            SaveVideoToCrashFolder(tor.SpeicherortCrash, tor.SpeicherortKameraIndoor);
            // Speichern des Outdoor-Crash-Videos
            SaveVideoToCrashFolder(tor.SpeicherortCrash, tor.SpeicherortKameraOutdoor);

            string seite = isIndoor ? "Innenseite" : "Aussenseite";

            // Optional: Logik zum Umgang mit weiteren Aktionen nach der Erkennung eines Crashes
            Console.WriteLine($"Crash erkannt am Tor {tor.TorName}, Typ: {seite}");
        }


        //Video in den Unfallordner kopieren
        private static void SaveVideoToCrashFolder(string crashVideoPath, string sourceVideoPath)
        {

            try
            {
                // Prüfen, ob die Quelldatei existiert
                if (File.Exists(sourceVideoPath))
                {
                    // Kopieren der Quelldatei in den Crash-Ordner
                    File.Copy(sourceVideoPath, crashVideoPath, overwrite: false);
                    Console.WriteLine($"Video wurde erfolgreich kopiert: {crashVideoPath}, Video wurde von {sourceVideoPath} Kopiert gespeichert in: {crashVideoPath}");
                }
                else
                {
                    // Quelldatei nicht gefunden
                    Console.WriteLine($"Speicherpfad {sourceVideoPath} wurde nicht gefunden");
                }
            }
            catch (Exception ex)
            {
                // Fehler beim Kopieren der Datei
                Console.WriteLine($"Fehler beim Kopieren der der Aufnahme in den Crash-Ordner: {ex.Message}");
            }
        }




        //private static void ÜberwacheUndBehandleFehler()
        //{
        //    // Beispiel: Überwachung des Prozessstatus
        //    // Angenommen, wir haben eine Methode, die den Status des ffmpeg-Prozesses überprüft:

        //    bool aufnahmeIstAktiv = ÜberprüfeAufnahmeStatus();



        //    if (!aufnahmeIstAktiv)

        //    {

        //        // Logik zum Neustarten der Aufnahme

        //        Console.WriteLine("Aufnahme wurde unerwartet beendet. Versuche neu zu starten...");

        //        // Neustart der Aufnahme für alle Tore
        //        try
        //        {
        //            foreach (var tor in tore)
        //            {
        //                ContinuousRecordingAsync();
        //            }
        //        }
        //        catch (Exception ex) 
        //        {
        //            Console.WriteLine($"Aufname konnte nicht Neu gestartet werden. {ex.Message}");
        //        }

        //    }

        //}

        //private static bool ÜberprüfeAufnahmeStatus()

        //{

        //    // Implementierung der Statusüberprüfung

        //    return true; // Beispielrückgabe

        //}

        // Eine einfache Validierungsmethode könnte so aussehen
        //private static bool ValidateCameraUrl(string url)
        //{
        //    return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)

        //        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps || uriResult.Scheme == "rtsp");

        //}

        //private static bool ValidateGpioPin(int pinNumber)
        //{
        //    // Beispielhafte Überprüfung: GPIO-Pins liegen oft zwischen 0 und 27 für Raspberry Pi

        //    return pinNumber >= 0 && pinNumber <= 27;
        //}

        //bool ValidateAufnahmeKonfiguration(AufnahmeKonfiguration konfig)
        //{
        //    // Überprüfe, ob die Stunden pro Datei, Anzahl Tage Backup und Speicherintervallstunden innerhalb akzeptabler Bereiche liegen
        //    return konfig.StundenProDatei > 0 && konfig.backupDays > 0 && konfig.SpeicherIntervallStunden > 0;
        //}

    }
}

