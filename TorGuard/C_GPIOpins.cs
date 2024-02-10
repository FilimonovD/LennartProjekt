using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorGuard
{

    public class C_GPIOpins()
    {
        private static GpioController gpioController;


        private static void InitializeGPIO()
        //Es ist wichtig, die GPIO-Pins effizient zu verwalten. Anstatt den Pin jedes Mal neu zu öffnen, sollte er einmalig geöffnet und dann durchgehend abgefragt werden
        {
            gpioController = new GpioController();

            foreach (var tor in tore)
            {
                gpioController.OpenPin(tor.GpioPinIndoor, PinMode.InputPullUp);
                gpioController.OpenPin(tor.GpioPinOutdoor, PinMode.InputPullUp);

                Console.WriteLine($"GPIO-Pins für Tor{tor.TorName}  {tor.GpioPinIndoor} (Innen) und {tor.GpioPinOutdoor} (Außen)  konfiguriert.");
            }
        }

        //Auslesen der Pine -> WAGO signal
        public static void CheckPinState(Tor tor, PinValue pinValue, bool isIndoor)
        {
            // Aktueller Zustand des Pins: Hoch oder Niedrig
            bool currentPinIsHigh = pinValue == PinValue.High;

            // Erwarteter Zustand des Pins basierend auf Indoor oder Outdoor
            // bool expectedHighState = isIndoor ? tor.IsHighIndoor : tor.IsHighOutdoor;

            // Zeitpunkt des letzten High-Zustands des Pins
            DateTime lastHighTime = isIndoor ? tor.LastHighTimeIndoor : tor.LastHighTimeOutdoor;

            // Überprüfung auf positive Flanke (niedrig zu hoch)
            if (currentPinIsHigh)// && !expectedHighState)
            {
                // Wenn der Pin von Niedrig auf Hoch geht

                if (isIndoor)
                {
                    // Für Indoor-Pins
                    tor.LastHighTimeIndoor = DateTime.Now; // Aktualisiere Zeitpunkt des letzten High-Zustands
                    tor.IsHighIndoor = true; // Setze den Zustand des Indoor-Pins auf Hoch
                }
                else
                {
                    // Für Outdoor-Pins
                    tor.LastHighTimeOutdoor = DateTime.Now; // Aktualisiere Zeitpunkt des letzten High-Zustands
                    tor.IsHighOutdoor = true; // Setze den Zustand des Outdoor-Pins auf Hoch
                }
            }

            //Überprüfung auf Zustandsdauer von mindestens 3 Sekunden
            else if (currentPinIsHigh && (DateTime.Now - lastHighTime).TotalSeconds >= 3)// && expectedHighState&& lastHighTime.HasValue)
            {
                // Wenn der Pin mindestens 3 Sekunden lang im High-Zustand ist
                HandleCrashDetection(tor, isIndoor);
            }

            // Überprüfung auf negative Flanke (hoch zu niedrig)
            else if (!currentPinIsHigh)//&& expectedHighState)
            {
                // Wenn der Pin von Hoch auf Niedrig geht
                if (isIndoor)
                {
                    // Für Indoor-Pins
                    tor.IsHighIndoor = false; // Setze den Zustand des Indoor-Pins auf Niedrig

                    tor.LastHighTimeIndoor = DateTime.MinValue; // Zurücksetzen des Indoor-Zeitstempels
                }
                else
                {
                    // Für Outdoor-Pins
                    tor.IsHighOutdoor = false; // Setze den Zustand des Outdoor-Pins auf Niedrig

                    tor.LastHighTimeOutdoor = DateTime.MinValue; // Zurücksetzen des Outdoor-Zeitstempels
                }

                //// Überprüfe, ob der High-Zustand mindestens 3 Sekunden lang war
                //if (lastHighTime.HasValue && (DateTime.Now - lastHighTime).TotalSeconds >= 3)
                //{
                //    // Wenn der High-Zustand mindestens 3 Sekunden lang war
                //    HandleCrashDetection(tor, isIndoor); // Behandle Crash-Erkennung
                //}

                // Zurücksetzen des Zeitstempels auf 0
                //if (isIndoor)
                //{
                //    tor.LastHighTimeIndoor = DateTime.MinValue; // Zurücksetzen des Indoor-Zeitstempels
                //}
                //else
                //{
                //    tor.LastHighTimeOutdoor = DateTime.MinValue; // Zurücksetzen des Outdoor-Zeitstempels
                //}
            }
        }
    }
}

