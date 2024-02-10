using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;



namespace TorGuard
{
    public partial class Netzwerkkonfig_Page : Page
    {
        // private const string ConfigFilePath = "Netzwerk_config.txt";//SpeicherName
        private const string ConfigFilePath_Netzwerk = "Netzwerk_config.json"; //SpeicherName
        string filePath_Netzwerk = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFilePath_Netzwerk);


        private void Netzwerkkonfig()
        {
            InitializeComponent();
            LoadConfigurationFromFile_Netzwerk(); // Beim Initialisieren Konfiguration laden
        }



            private void btnSpeichern_Click(object sender, RoutedEventArgs e)
            {
                try
                {
                    SaveConfigurationToFile_Netzwerk();

                    MessageBox.Show("Tor Konfiguration gespeichert.", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Speichern der Konfiguration: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        private void SaveConfigurationToFile_Netzwerk()
        {
            try
            {
                var configData_Netzwerk = new
                {
                    WLAN_SSID = txtWlan.Text,
                    WLAN_Password = txtWLANpassword.Password
                };

                string json = JsonSerializer.Serialize(configData_Netzwerk, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath_Netzwerk, json); // Verwenden Sie ConfigFilePath_Netzwerk für den Dateinamen

                MessageBox.Show("Die Eintragungen der Anwendung wurden gespeichert.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Speichern der Konfiguration: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadConfigurationFromFile_Netzwerk()
        {
            try
            {
                string filePath_Netzwerk = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFilePath_Netzwerk);
                if (File.Exists(filePath_Netzwerk))
                {
                    string json = File.ReadAllText(filePath_Netzwerk);
                    var configData = JsonSerializer.Deserialize<dynamic>(json);

                    txtWlan.Text = configData?.WLAN_SSID;
                    txtWLANpassword.Password = configData?.WLAN_Password;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der Konfiguration: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }




        // Event Handler für den "WLAN Verbinden"-Button
        private void btnWLAN_Click(object sender, RoutedEventArgs e)
        {
            // Überprüfen, ob WLAN-SSID und Passwort nicht leer sind
            if (!string.IsNullOrWhiteSpace(txtWlan.Text) && !string.IsNullOrWhiteSpace(txtWLANpassword.Password))
            {
                // WLAN-Verbindung herstellen
                ConnectToWifi(txtWlan.Text, txtWLANpassword.Password);
            }
            else
            {
                MessageBox.Show("Bitte geben Sie WLAN-SSID und Passwort ein.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        // Überschreiben der OnLoaded-Methode, um die Konfiguration beim Laden der Anwendung zu laden
        private void Netzwerkkonfig_Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadConfigurationFromFile_Netzwerk();

                MessageBox.Show("Tor Konfiguration gespeichert.", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Speichern der Konfiguration: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveConfigurationToFile_Netzwerk();
               
                MessageBox.Show("Tor Konfiguration gespeichert.", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Speichern der Konfiguration: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
    
        

}
    



