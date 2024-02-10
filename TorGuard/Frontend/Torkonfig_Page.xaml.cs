using System.Text.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
using System.Runtime.ConstrainedExecution;

//Mögliche Fehler und Verbesserungsvorschläge:
//Parameterloser Konstruktor: Der parameterlose Konstruktor lädt auch die Konfigurationsdatei, aber ohne eine torVerwaltung-Instanz zu initialisieren oder zu übergeben. Dies könnte zu einer NullReferenceException führen, wenn torVerwaltung verwendet wird. Stellen Sie sicher, dass torVerwaltung in jedem Fall korrekt initialisiert wird.

//Doppelte Initialisierung in Konstruktoren: Die Logik zum Laden der Konfigurationsdatei ist in beiden Konstruktoren dupliziert. Es wäre besser, diese Logik in eine separate Methode auszulagern und von beiden Konstruktoren aus aufzurufen.

//Speichern-Logik: In der btnSpeichern_Click-Methode wird ein neues Tor-Objekt erstellt, auch wenn ein existierendes Tor ausgewählt und bearbeitet wird. Dies könnte zu unbeabsichtigten Ergebnissen führen, besonders wenn Sie versuchen, ein existierendes Tor zu aktualisieren. Stattdessen sollten Sie nur ein neues Tor-Objekt erstellen, wenn wirklich ein neues Tor hinzugefügt wird.

//Fehlerbehandlung: Es ist gut, dass Sie Fehlermeldungen anzeigen, wenn beim Laden oder Speichern ein Fehler auftritt. Es könnte jedoch hilfreich sein, zusätzliche Validierungen für Benutzereingaben hinzuzufügen, um zu verhindern, dass ungültige Daten gespeichert werden.

//UI-Feedback: Nachdem ein neues Tor hinzugefügt oder Änderungen gespeichert wurden, könnte es nützlich sein, dem Benutzer ein visuelles Feedback zu geben, z.B. durch das Leeren der Textfelder oder das Aktualisieren der Auswahl in der ListBox, um die Änderungen zu reflektieren.

//Verbesserung der Benutzerfreundlichkeit: Für eine bessere Benutzererfahrung könnten Sie Labels neben den Textfeldern hinzufügen, um klarzustellen, was in jedes Feld eingegeben werden soll.


namespace TorGuard
{
    public partial class Torkonfig_Page : Page 
    {
        private Dictionary<string, HC_Tore> torDictionary = new Dictionary<string, HC_Tore>();


        private const string ConfigFilePath_Tor = "Tor_config.json"; //SpeicherName
        string filePath_Tor = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFilePath_Tor);



        

        public Torkonfig_Page() //TorVerwaltung torVerwaltung
        {
            InitializeComponent();
            
            LoadConfigurationFromFile_Tor(); // Beim Initialisieren Konfiguration laden
        }



        //public TorConfigWindow()
        //{
        //    InitializeComponent();
        //    LoadConfigurationFromFile_Tor(); // Beim Initialisieren Konfiguration laden
        //}

        //Methode um ein Neu Tor hinzuzufügen
        
            public void AddNewTor()
            {
                var neuesTor = new HC_Tore(
                    txtTorName.Text,
                    Convert.ToInt32(txtGpioPinIndoor.Text),
                    Convert.ToInt32(txtGpioPinOutdoor.Text),
                    txtKameraIndoorUrl.Text,
                    txtKameraOutdoorUrl.Text,
                    txtSpeicherortKameraIndoor.Text,
                    txtSpeicherortKameraOutdoor.Text,
                    txtSpeicherortCrash.Text);

            // Füge das neue Tor zur Liste hinzu
            torVerwaltung.Tore.Add(neuesTor);

            // Füge das neue Tor auch zum Dictionary hinzu
            torDictionary.Add(neuesTor.TorName, neuesTor);

            RefreshTorList(); // Methode zur Aktualisierung der UI, muss implementiert werden
        }



        private void btnSpeichern_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Überprüfen, ob ein Tor ausgewählt ist
                if (listBoxTore.SelectedItem != null)
                {
                    // Hole das ausgewählte Tor aus dem Dictionary
                    HC_Tore ausgewähltesTor = torDictionary[listBoxTore.SelectedItem.ToString()];

                    // Aktualisiere die Eigenschaften des ausgewählten Tors
                    ausgewähltesTor.TorName = txtTorName.Text;
                    ausgewähltesTor.GpioPinIndoor = Convert.ToInt32(txtGpioPinIndoor.Text);
                    ausgewähltesTor.GpioPinOutdoor = Convert.ToInt32(txtGpioPinOutdoor.Text);
                    ausgewähltesTor.KameraIndoorUrl = txtKameraIndoorUrl.Text;
                    ausgewähltesTor.KameraOutdoorUrl = txtKameraOutdoorUrl.Text;
                    ausgewähltesTor.SpeicherortKameraIndoor = txtSpeicherortKameraIndoor.Text;
                    ausgewähltesTor.SpeicherortKameraOutdoor = txtSpeicherortKameraOutdoor.Text;
                    ausgewähltesTor.SpeicherortCrash = txtSpeicherortCrash.Text;
                }
                else
                {
                    // Wenn kein Tor ausgewählt ist, erstelle ein neues Tor
                    HC_Tore neuesTor = new HC_Tore(
                        txtTorName.Text,
                        Convert.ToInt32(txtGpioPinIndoor.Text),
                        Convert.ToInt32(txtGpioPinOutdoor.Text),
                        txtKameraIndoorUrl.Text,
                        txtKameraOutdoorUrl.Text,
                        txtSpeicherortKameraIndoor.Text,
                        txtSpeicherortKameraOutdoor.Text,
                        txtSpeicherortCrash.Text);

                    // Füge das neue Tor zum Dictionary hinzu
                    torDictionary.Add(neuesTor.TorName, neuesTor);
                }

                // Speichere die Konfiguration
                SaveConfigurationToFile_Tor();
                RefreshTorList();

                MessageBox.Show("Tor Konfiguration gespeichert.", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Speichern der Konfiguration: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void btnAddTor_Click(object sender, RoutedEventArgs e)
        {
            AddNewTor();
        }

        //Methode zum speichern von eingaben im Layout              
        private void SaveConfigurationToFile_Tor()
        {
            try
            {
                
                // Annahme: torVerwaltung hält eine Liste von Toren oder ähnliches
                string json = JsonSerializer.Serialize(torDictionary, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath_Tor, json);

                MessageBox.Show("Tor Konfiguration gespeichert.", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Speichern der Konfiguration: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ListBoxTore_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxTore.SelectedItem != null)
            {
                // Hole das ausgewählte Tor aus dem Dictionary
                var ausgewähltesTor = torDictionary[listBoxTore.SelectedItem.ToString()];

                // Aktualisiere die UI mit den Eigenschaften des ausgewählten Tors
                txtTorName.Text = ausgewähltesTor.TorName;
                txtGpioPinIndoor.Text = ausgewähltesTor.GpioPinIndoor.ToString();
                txtGpioPinOutdoor.Text = ausgewähltesTor.GpioPinOutdoor.ToString();
                txtKameraIndoorUrl.Text = ausgewähltesTor.KameraIndoorUrl;
                txtKameraOutdoorUrl.Text = ausgewähltesTor.KameraOutdoorUrl;
                txtSpeicherortKameraIndoor.Text = ausgewähltesTor.SpeicherortKameraIndoor;
                txtSpeicherortKameraOutdoor.Text = ausgewähltesTor.SpeicherortKameraOutdoor;
                txtSpeicherortCrash.Text = ausgewähltesTor.SpeicherortCrash;
            }
        }




        // Methode zum Laden der Labels und ListBox-Inhalte aus einer Datei
        private void LoadConfigurationFromFile_Tor()
        {
            try
            {
                if (File.Exists(filePath_Tor))
                {
                    string json = File.ReadAllText(filePath_Tor);
                    torVerwaltung = JsonSerializer.Deserialize<TorVerwaltung>(json);
                    RefreshTorList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der Konfiguration: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void RefreshTorList()
        {
            listBoxTore.ItemsSource = null;
            listBoxTore.ItemsSource = torVerwaltung.Tore.Select(t => t.TorName).ToList();
        }




        // Überschreiben der OnClosing-Methode, um die Konfiguration beim Schließen der Anwendung zu speichern
        private void Tor_Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadConfigurationFromFile_Tor();


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
                SaveConfigurationToFile_Tor();

                MessageBox.Show("Tor Konfiguration gespeichert.", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Speichern der Konfiguration: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        // Hier könnten Sie die Tor-Instanz in einer Liste speichern, an eine Datenbank senden oder eine andere Aktion ausführen


    }

}


//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
//using System.Windows.Shapes;



//namespace TorGuard
//{
//    public partial class TorConfigWindow : Window
//    {
//        private const string ConfigFilePath = "Tor_config.txt";//SpeicherName

        
//            private TorVerwaltung torVerwaltung;

//            public TorConfigWindow(C_Tor_Liste torVerwaltung)
//            {
//                InitializeComponent();
//                this.torVerwaltung = torVerwaltung;
//                // Jetzt können Sie torVerwaltung nutzen, um auf die Liste der Tore zuzugreifen
//            }
        

//        public TorConfigWindow()
//        {
//            InitializeComponent();
//            LoadConfigurationFromFile(); // Beim Initialisieren Konfiguration laden
//        }

//        //var tor = new Tor(
//        //      int.Parse(txtTorNummer.Text),
//        //      int.Parse(txtGpioPinIndoor.Text),
//        //      int.Parse(txtGpioPinOutdoor.Text),
//        //      txtKameraIndoorUrl.Text,
//        //      txtKameraOutdoorUrl.Text,
//        //      txtSpeicherortKameraIndoor.Text,
//        //      txtSpeicherortKameraOutdoor.Text,
//        //      txtSpeicherortCrash.Text);

//        //private static AufnahmeKonfig konfig = new AufnahmeKonfig(8, 2, 0.5); // 24 Stunden pro Datei, 1 Tag Backup, halbstündige Segmente


//        private void btnSpeichern_Click(object sender, RoutedEventArgs e)
//        {
//            try
//            {
//                SaveConfigurationToFile();

//                MessageBox.Show("Tor Konfiguration gespeichert.", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Fehler beim Speichern der Konfiguration: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }


//        private void btnAddTor_Click(object sender, RoutedEventArgs e)
//        {

//            var neuesTor = new Tor(txtTorName.Text,
//                                    Convert.ToInt16(txtGpioPinIndoor.Text),
//                                    Convert.ToInt16(txtGpioPinOutdoor.Text),
//                                    txtKameraIndoorUrl.Text,
//                                    txtKameraOutdoorUrl.Text,
//                                    txtSpeicherortKameraIndoor.Text,
//                                    txtSpeicherortKameraOutdoor.Text,
//                                    txtSpeicherortCrash.Text);
//            Tore.Add(neuesTor);
//            // Aktualisieren Sie die Benutzeroberfläche, z.B. die ListBox, um das neue Tor anzuzeigen
//        }

//        //Methode zum speichern von eingaben im Layout              
//        private void SaveConfigurationToFile()
//        {
//            try
//            {
//                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFilePath);

//                using (StreamWriter writer = new StreamWriter(filePath))
//                {
//                    // Torname speichern
//                    writer.WriteLine("[SECTION_START][TorNummer]");
//                    foreach (var Torname in Tor)
//                    {
//                        writer.WriteLine(txtTorName); ;
//                    }
//                    writer.WriteLine("[SECTION_END]");

//                    // GPIO pin Nummern speichern
//                    writer.WriteLine("[SECTION_START][GPIO-PINS]");
//                    writer.WriteLine(txtGpioPinIndoor);
//                    writer.WriteLine(txtGpioPinOutdoor);
//                    writer.WriteLine("[SECTION_END]");

//                    //Kamera URLs speichern
//                    writer.WriteLine("[SECTION_START][Kamera_URLs]");
//                    writer.WriteLine(txtKameraIndoorUrl);
//                    writer.WriteLine(txtKameraOutdoorUrl);
//                    writer.WriteLine("[SECTION_END]");

//                    //Speicherort speichern
//                    writer.WriteLine("[SECTION_START][Saveplaces]");
//                    writer.WriteLine(txtSpeicherortKameraIndoor);
//                    writer.WriteLine(txtSpeicherortKameraOutdoor);
//                    writer.WriteLine(txtSpeicherortCrash);
//                    writer.WriteLine("[SECTION_END]");


//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Fehler beim Speichern der Konfiguration: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }

//        // Methode zum Laden der Labels und ListBox-Inhalte aus einer Datei
//        private void LoadConfigurationFromFile()
//        {
//            MessageBox.Show("Die Anwendung wurde geöffnet und initialisiert.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
//            try
//            {
//                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFilePath);

//                if (File.Exists(filePath))
//                {
//                    using (StreamReader reader = new StreamReader(filePath))
//                    {
//                        // Tornummer speichern laden
//                        ReadSection(reader, "[TorNummer]");

//                        // GPIO pin Nummern laden
//                        ReadSection(reader, "[GPIO-PINS]");

//                        // amera URLs laden
//                        ReadSection(reader, "[Kamera_URLs]");

//                        ////Speicherort Laden
//                        ReadSection(reader, "[Saveplaces]");

//                        // Aktualisieren der Listbox nach dem Laden
//                        RefreshTorlist();
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Fehler beim Laden der Konfiguration: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }

//        // Hilfsmethode zum Lesen von Daten aus einer bestimmten Sektion
//        private void ReadSection(StreamReader reader, string sectionHeader)
//        {
//            string line;
//            while ((line = reader.ReadLine()) != null)
//            {
//                if (line.StartsWith("[SECTION_START]") && line.Contains(sectionHeader))
//                {
//                    // Beginn der Sektion gefunden
//                    while ((line = reader.ReadLine()) != null)
//                    {
//                        if (line.StartsWith("[SECTION_END]"))
//                        {
//                            // Ein neuer Abschnitt wurde erreicht, die Schleife beenden
//                            break;
//                        }

//                        // Verarbeiten Sie die Daten innerhalb der Sektion entsprechend
//                        switch (sectionHeader)
//                        {

//                            case "[TorNummer]":
//                                Tore.Add(line);
//                                break;

//                            case "[GPIO-PINS]":
//                                txtGpioPinIndoor.Text = line;
//                                txtGpioPinOutdoor.Text = reader.ReadLine();
//                                break;

//                            case "[Kamera_URLs]":
//                                txtKameraIndoorUrl.Text = line;
//                                txtKameraOutdoorUrl.Text = reader.ReadLine();
//                                break;

//                            case "[Saveplaces]":
//                                txtSpeicherortKameraIndoor.Text = line;
//                                txtSpeicherortKameraOutdoor.Text = reader.ReadLine();
//                                txtSpeicherortCrash.Text = reader.ReadLine();
//                                break;
//                                // Fügen Sie bei Bedarf weitere Sektionen hinzu
//                        }
//                    }

//                    // Wichtig: Brechen Sie die äußere Schleife ab, nachdem die Sektion verarbeitet wurde
//                    break;
//                }
//            }
//        }


//        // Überschreiben der OnClosing-Methode, um die Konfiguration beim Schließen der Anwendung zu speichern
//        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
//        {
//            try
//            {
//                SaveConfigurationToFile();
//                base.OnClosing(e);

//                MessageBox.Show("Tor Konfiguration gespeichert.", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Fehler beim Speichern der Konfiguration: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
//            }

//        }

//        // Überschreiben der OnLoaded-Methode, um die Konfiguration beim Laden der Anwendung zu laden
//        protected override void OnContentRendered(EventArgs e)
//        {
//            try
//            {
//                LoadConfigurationFromFile();
//                base.OnContentRendered(e);

//                MessageBox.Show("Tor Konfiguration gespeichert.", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Fehler beim Speichern der Konfiguration: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
//            }

//        }
//        // Hier könnten Sie die Tor-Instanz in einer Liste speichern, an eine Datenbank senden oder eine andere Aktion ausführen


//    }
//}
//}
