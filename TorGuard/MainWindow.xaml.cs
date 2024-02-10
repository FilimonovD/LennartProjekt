using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Net;

using MailKit.Net.Smtp;
using MimeKit;
using System.Diagnostics;
using Org.BouncyCastle.Crypto;
using System.Runtime.Intrinsics.X86;
using System.IO;
using System.Text.Json;
using System.Xml.Linq;

// Test Email MentkenL061@sus-am-bktm.de

//Bei Mail.de

//Login Benutzername: Tor_Guard@web.de
//Login Passwort:TorGuard

//Posteingang(IMAP)
//Server: imap.web.de
//Port: 993
//Verschlüsselung: SSL oder Verschlüsselung
//Postausgang(SMTP)
//Server: smtp.web.de
//Port: 587
//Verschlüsselung: STARTTLS, TLS oder Verschlüsselung

//Steht in einem Programm "STARTTLS" nicht zur Verfügung, nutzen Sie bitte das Protokoll "TLS". Existiert auch hierfür keine Option, genügt es, die Option "Verschlüsselung" zu aktivieren. Alternativ können Sie für den Postausgangsserver auch Port 465 mit der Verschlüsselung "SSL" nutzen.

//Bitte beachten Sie: Ein TLS-Protokoll können Sie nur dann verwenden, wenn Ihr E-Mail-Programm über die aktuellen TLS-Versionen 1.2 oder 1.3 verfügt. Die TLS-Versionen 1.0 und 1.1 werden aus Sicherheitsgründen nicht mehr von uns unterstützt.

namespace TorGuard
{
    public partial class MainWindow : Window
    {
        // Liste zum Speichern der E-Mail-Adressen
        private List<string> emailAddresses = new List<string>();

        //Datei Name für Backup des Layouts 
        private const string ConfigFilePath = "Empfängerlist_config.json";

        public MainWindow()
        {
            InitializeComponent();
        }


        // Event Handler für den "Hinzufügen"-Button
        private void btnAddEmail_Click(object sender, RoutedEventArgs e)
        {
            // Überprüfen, ob die Textbox für E-Mail-Adressen nicht leer ist
            if (!string.IsNullOrWhiteSpace(txtEmailAddress.Text))
            {
                // E-Mail-Adresse zur Liste hinzufügen
                emailAddresses.Add(txtEmailAddress.Text);

                // E-Mail-Adressen in der Listbox aktualisieren
                RefreshEmailList();
            }
            else
            {
                MessageBox.Show("Bitte geben Sie eine gültige E-Mail-Adresse ein.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Textbox leeren
            txtEmailAddress.Clear();
        }

        // Event Handler für den "Löschen"-Button
        private void btnRemoveEmail_Click(object sender, RoutedEventArgs e)
        {
            // Überprüfen, ob mindestens eine E-Mail-Adresse ausgewählt ist
            if (lstEmailAddresses.SelectedItems.Count > 0)
            {
                // Ausgewählte E-Mail-Adressen aus der Liste entfernen
                foreach (var selectedEmail in lstEmailAddresses.SelectedItems.Cast<string>().ToList())
                {
                    emailAddresses.Remove(selectedEmail);
                }

                // E-Mail-Adressen in der Listbox aktualisieren
                RefreshEmailList();
            }
            else
            {
                MessageBox.Show("Bitte wählen Sie mindestens eine E-Mail-Adresse zum Löschen aus.", "Hinweis", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public async void btnTest_Click(object sender, RoutedEventArgs e)
        {
            // Überprüfen, ob mindestens eine E-Mail-Adresse vorhanden ist
            if (emailAddresses.Count > 0)
            {
                await EmailsAsync(emailAddresses);
            }
            else
            {
                MessageBox.Show("Bitte fügen Sie mindestens eine E-Mail-Adresse hinzu.", "Hinweis", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

       

        // Methode zur Aktualisierung der E-Mail-Liste in der Listbox
        private void RefreshEmailList()
        {
            // Listbox leeren und mit aktualisierten E-Mail-Adressen füllen
            lstEmailAddresses.Items.Clear();
            foreach (var empfängeradresse in emailAddresses)
            {
                lstEmailAddresses.Items.Add(empfängeradresse);
            }
        }

       

        //Methode zum Verbindung mit WLAN
        private void ConnectToWifi(string ssid, string password)
        {
            try
            {
                // Überprüfen, ob WLAN-SSID und Passwort nicht null sind
                if (!string.IsNullOrWhiteSpace(ssid) && !string.IsNullOrWhiteSpace(password))
                {
                    // WLAN-Verbindung herstellen
                    Process.Start("netsh", $"wlan connect name=\"{ssid}\" ssid=\"{ssid}\" keyMaterial=\"{password}\"");
                    MessageBox.Show($"Erfolgreich mit WLAN \"{ssid}\" verbunden.", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("WLAN-SSID und Passwort dürfen nicht leer sein.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Herstellen der WLAN-Verbindung: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Ereignishandler für den Klick auf den "Tor Konfiguration" Button
        private void btnTorConfig_Click(object sender, RoutedEventArgs e)
        {
            farame.Content = new Torkonfig_Page(); // Hier wird die TorConfigPage angezeigt
        }

        // Ereignishandler für den Klick auf den "Netzwerk Konfiguration" Button
        private void btnNetworkConfig_Click(object sender, RoutedEventArgs e)
        {
            frame.Content = new Netzwerkkonfig_Page(); // Hier wird die NetworkConfigPage angezeigt
        }

        //.
        //Sichern des Layouts
        //.

        private void btnSpeichern_Click(object sender, RoutedEventArgs e)
        { //Wenn der Butten betätigt wird soll abgespeichert werden 
            SaveConfigurationToFile_Mail();

            MessageBox.Show("Die Eintragungen der Anwendung wurden gespeichert.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);

        }

        //Methode zum speichern von eingaben im Layout              
        private void SaveConfigurationToFile_Mail()
        {
            try
            {
                var configData_Email = new
                {
                    
                    EmailAddresses = emailAddresses,
                    SMTP = new
                    {
                        Email = txtsEmail.Text,
                        Password = txtEmailpassword.Password,
                        Address = txtsmtpadre.Text,
                        Port = txtSmtpNr.Text
                    }
                };

                string json = JsonSerializer.Serialize(configData_Email, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFilePath), json);

                MessageBox.Show("Die Eintragungen der Anwendung wurden gespeichert.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Speichern der Konfiguration: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Methode zum Laden der Labels und ListBox-Inhalte aus einer Datei
        private void LoadConfigurationFromFile_Mail()
        {
            MessageBox.Show("Die Anwendung wurde geöffnet und initialisiert.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                try
                {
                    string filePath_Mail = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFilePath);
                    if (File.Exists(filePath_Mail))
                    {
                        string json = File.ReadAllText(filePath_Mail);
                        var configData = JsonSerializer.Deserialize<dynamic>(json);

                        

                        emailAddresses = ((JsonElement)configData?.EmailAddresses).EnumerateArray().Select(email => email.GetString()).ToList();

                        txtsEmail.Text = configData?.SMTP?.Email;
                        txtEmailpassword.Password = configData?.SMTP?.Password;
                        txtsmtpadre.Text = configData?.SMTP?.Address;
                        txtSmtpNr.Text = configData?.SMTP?.Port;

                        RefreshEmailList();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Laden der Konfiguration: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            

           
        }

        //


        // Überschreiben der OnClosing-Methode, um die Konfiguration beim Schließen der Anwendung zu speichern
        protected override void OnClosing_Mail(System.ComponentModel.CancelEventArgs e)
        {
            SaveConfigurationToFile_Mail();
            SaveConfigurationToFile_Netzwerk();//Wenn Fenster direkt geschlossen wird
            SaveConfigurationToFile_Tor();
            base.OnClosing(e);
        }

        // Überschreiben der OnLoaded-Methode, um die Konfiguration beim Laden der Anwendung zu laden
        protected override void OnContentRendered_Mail(EventArgs e)
        {
            LoadConfigurationFromFile_Mail ();
           
            base.OnContentRendered(e);
        }


    }
}












//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.NetworkInformation;
//using System.Net.Sockets;
//using System.Windows;
//using System.Windows.Controls;
//using System.Net;

//using MailKit.Net.Smtp;
//using MimeKit;
//using System.Diagnostics;
//using Org.BouncyCastle.Crypto;
//using System.Runtime.Intrinsics.X86;
//using System.IO;
//using MailKit;

//// Test Email MentkenL061@sus-am-bktm.de

////Bei Mail.de

////Login Benutzername: Tor_Guard@web.de
////Login Passwort:TorGuard

////Posteingang(IMAP)
////Server: imap.web.de
////Port: 993
////Verschlüsselung: SSL oder Verschlüsselung
////Postausgang(SMTP)
////Server: smtp.web.de
////Port: 587
////Verschlüsselung: STARTTLS, TLS oder Verschlüsselung

////Steht in einem Programm "STARTTLS" nicht zur Verfügung, nutzen Sie bitte das Protokoll "TLS". Existiert auch hierfür keine Option, genügt es, die Option "Verschlüsselung" zu aktivieren. Alternativ können Sie für den Postausgangsserver auch Port 465 mit der Verschlüsselung "SSL" nutzen.

////Bitte beachten Sie: Ein TLS-Protokoll können Sie nur dann verwenden, wenn Ihr E-Mail-Programm über die aktuellen TLS-Versionen 1.2 oder 1.3 verfügt. Die TLS-Versionen 1.0 und 1.1 werden aus Sicherheitsgründen nicht mehr von uns unterstützt.

//namespace TorGuard
//{
//    public partial class MainWindow : Window
//    {
//        // Liste zum Speichern der E-Mail-Adressen
//        private List<string> emailAddresses = new List<string>();

//        //Datei Name für Backup des Layouts 
//        private const string ConfigFilePath = "app_config.txt";
//        public MainWindow()
//        {
//            InitializeComponent();
//        }


//        // Event Handler für den "Hinzufügen"-Button
//        private void btnAddEmail_Click(object sender, RoutedEventArgs e)
//        {
//            // Überprüfen, ob die Textbox für E-Mail-Adressen nicht leer ist
//            if (!string.IsNullOrWhiteSpace(txtEmailAddress.Text))
//            {
//                // E-Mail-Adresse zur Liste hinzufügen
//                emailAddresses.Add(txtEmailAddress.Text);

//                // E-Mail-Adressen in der Listbox aktualisieren
//                RefreshEmailList();
//            }
//            else
//            {
//                MessageBox.Show("Bitte geben Sie eine gültige E-Mail-Adresse ein.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
//            }

//            // Textbox leeren
//            txtEmailAddress.Clear();
//        }

//        // Event Handler für den "Löschen"-Button
//        private void btnRemoveEmail_Click(object sender, RoutedEventArgs e)
//        {
//            // Überprüfen, ob mindestens eine E-Mail-Adresse ausgewählt ist
//            if (lstEmailAddresses.SelectedItems.Count > 0)
//            {
//                // Ausgewählte E-Mail-Adressen aus der Liste entfernen
//                foreach (var selectedEmail in lstEmailAddresses.SelectedItems.Cast<string>().ToList())
//                {
//                    emailAddresses.Remove(selectedEmail);
//                }

//                // E-Mail-Adressen in der Listbox aktualisieren
//                RefreshEmailList();
//            }
//            else
//            {
//                MessageBox.Show("Bitte wählen Sie mindestens eine E-Mail-Adresse zum Löschen aus.", "Hinweis", MessageBoxButton.OK, MessageBoxImage.Information);
//            }
//        }

//        private async void btnTest_Click(object sender, RoutedEventArgs e)
//        {
//            // Überprüfen, ob mindestens eine E-Mail-Adresse vorhanden ist
//            if (emailAddresses.Count > 0)
//            {
//                await EmailsAsync(emailAddresses);
//            }
//            else
//            {
//                MessageBox.Show("Bitte fügen Sie mindestens eine E-Mail-Adresse hinzu.", "Hinweis", MessageBoxButton.OK, MessageBoxImage.Information);
//            }
//        }

//        // Methode zum asynchronen Versenden von E-Mails
//        private async Task<bool> EmailsAsync(List<string> empfängeradressen)
//        {
//            bool allSuccess = true;

//            foreach (string empfängeradresse in empfängeradressen)
//            {
//                bool success = await SendEmailAsync(empfängeradresse);
//                allSuccess &= success; // Setzt allSuccess auf false, wenn irgendein Aufruf von SendTestEmailAsync false zurückgibt
//            }
//            return allSuccess;
//        }

//        //Methode zum asynchronen Versenden einer E-Mail
//        private async Task<bool> SendEmailAsync(string empfängeradresse)
//        {

//            try
//            {   //Email Kopf angaben 

//                MimeMessage email = new MimeMessage();

//                //email.From.Add(new MailboxAddress("Absender", "Tor_Guard@web.de"));
//                //email.To.Add(new MailboxAddress("Empfänger", "lementken@gmail.com"));

//                email.From.Add(new MailboxAddress("Absender", txtsEmail.Text));
//                email.To.Add(new MailboxAddress("Empfänger", empfängeradresse));


//                email.Subject = $" Unfallberricht Tor_Guard vom {DateTime.Now:dd.MM.yyyy um HH.mm.ss}";


//                email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
//                {
//                    Text = "<b>$@\"Marvin kasst du was für den Unfall berichtstextst formulieren ? Mit bedauern müssen wir ihnen mitteilen das in ihrer Firmer das Torxxx von xxx  am xx.xxx.xx um xx.xx beschädigt wurde bitte überprüfen Sie die Video aufzeichnung  im besagten Zeitpunkt . Sie finden Die aufzeichungen unter xxxxxxx \" };\r\n.</b>"
//                };

//                using (var smtp = new SmtpClient())// Email versant 
//                {
//                    //smtp.Connect("smtp.web.de", 587, true); //smtp-serveradresse,Port Nr., gesicherteverbindung erzwinge = true#
//                    //smtp.Connect("smtp.web.de", 465, MailKit.Security.SecureSocketOptions.SslOnConnect);
//                    //smtp.Connect("smtp.web.de", 587, MailKit.Security.SecureSocketOptions.StartTls);
//                    smtp.Connect(txtsmtpadre.Text, Convert.ToInt32(txtSmtpNr.Text), MailKit.Security.SecureSocketOptions.SslOnConnect);

//                    // smtp.Authenticate("Tor_Guard@web.de", "TorGuard");
//                    smtp.Authenticate(txtsEmail.Text, txtEmailpassword.Password);//Daten aus der Maske  // Nur erforderlich, wenn der SMTP-Server eine Authentifizierung erfordert

//                    await smtp.SendAsync(email);//await verwendet, um asynchrone Operationen zu kennzeichnen
//                    smtp.Disconnect(true);

//                    return true; // Erfolg
//                }
//            }
//            catch (MailKit.Security.AuthenticationException)
//            {
//                MessageBox.Show($"Login fehlgeschlagen. Überprüfen Sie die ihre Anmelde Informationene für {txtsEmail.Text}.Wenn das nicht hilft überprüfe ob du dich Einloggen kannst, eventull wegen nicht vertrauenswürdig gesperrt.", "Authentifizierungsfehler", MessageBoxButton.OK, MessageBoxImage.Error);
//                return false; // Authentifizierungsfehler
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Fehler beim Versenden der E-Mail an {empfängeradresse}: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
//                return false; // Sonstiger Fehler
//            }
//        }

//        // Methode zur Aktualisierung der E-Mail-Liste in der Listbox
//        private void RefreshEmailList()
//        {
//            // Listbox leeren und mit aktualisierten E-Mail-Adressen füllen
//            lstEmailAddresses.Items.Clear();
//            foreach (var empfängeradresse in emailAddresses)
//            {
//                lstEmailAddresses.Items.Add(empfängeradresse);
//            }
//        }

//        // Event Handler für den "WLAN Verbinden"-Button
//        private void btnWLAN_Click(object sender, RoutedEventArgs e)
//        {
//            // Überprüfen, ob WLAN-SSID und Passwort nicht leer sind
//            if (!string.IsNullOrWhiteSpace(txtWlan.Text) && !string.IsNullOrWhiteSpace(txtWLANpassword.Password))
//            {
//                // WLAN-Verbindung herstellen
//                ConnectToWifi(txtWlan.Text, txtWLANpassword.Password);
//            }
//            else
//            {
//                MessageBox.Show("Bitte geben Sie WLAN-SSID und Passwort ein.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }

//        //Methode zum Verbindung mit WLAN
//        private void ConnectToWifi(string ssid, string password)
//        {
//            try
//            {
//                // Überprüfen, ob WLAN-SSID und Passwort nicht null sind
//                if (!string.IsNullOrWhiteSpace(ssid) && !string.IsNullOrWhiteSpace(password))
//                {
//                    // WLAN-Verbindung herstellen
//                    Process.Start("netsh", $"wlan connect name=\"{ssid}\" ssid=\"{ssid}\" keyMaterial=\"{password}\"");
//                    MessageBox.Show($"Erfolgreich mit WLAN \"{ssid}\" verbunden.", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
//                }
//                else
//                {
//                    MessageBox.Show("WLAN-SSID und Passwort dürfen nicht leer sein.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Fehler beim Herstellen der WLAN-Verbindung: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }

//        //.
//        //Sichern des Layouts
//        //.

//        private void btnSpeichern_Click(object sender, RoutedEventArgs e)
//        { //Wenn der Butten betätigt wird soll abgespeichert werden 
//            SaveConfigurationToFile();

//            MessageBox.Show("Die Eintragungen der Anwendung wurden gespeichert.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);

//        }

//        //Methode zum speichern von eingaben im Layout              
//        private void SaveConfigurationToFile()
//        {
//            try
//            {
//                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFilePath);

//                using (StreamWriter writer = new StreamWriter(filePath))
//                {
//                    // WLAN-SSID und Passwort speichern
//                    writer.WriteLine("[SECTION_START][WLAN]");
//                    writer.WriteLine(txtWlan.Text);
//                    writer.WriteLine(txtWLANpassword.Password);
//                    writer.WriteLine("[SECTION_END]");

//                    // E-Mail-Adressen speichern
//                    writer.WriteLine("[SECTION_START][EmailAdressen]");
//                    foreach (var empfängeradresse in emailAddresses)
//                    {
//                        writer.WriteLine(empfängeradresse);
//                    }
//                    writer.WriteLine("[SECTION_END]");

//                    // Smtp Email Daten und Passwort speichern
//                    writer.WriteLine("[SECTION_START][SMTP]");
//                    writer.WriteLine(txtsEmail.Text);
//                    writer.WriteLine(txtEmailpassword.Password);
//                    writer.WriteLine(txtsmtpadre.Text);
//                    writer.WriteLine(txtSmtpNr.Text);
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
//                        // WLAN-SSID und Passwort laden
//                        ReadSection(reader, "[WLAN]");

//                        // E-Mail-Adressen laden
//                        ReadSection(reader, "[EmailAdressen]");

//                        // Smtp Email Daten und Passwort laden
//                        ReadSection(reader, "[SMTP]");

//                        // Aktualisieren der Listbox nach dem Laden
//                        RefreshEmailList();
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
//                            case "[WLAN]":
//                                txtWlan.Text = line;
//                                txtWLANpassword.Password = reader.ReadLine();
//                                break;

//                            case "[EmailAdressen]":
//                                emailAddresses.Add(line);
//                                break;

//                            case "[SMTP]":
//                                txtsEmail.Text = line;
//                                txtEmailpassword.Password = reader.ReadLine();
//                                txtsmtpadre.Text = reader.ReadLine();
//                                txtSmtpNr.Text = reader.ReadLine();
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
//            SaveConfigurationToFile();
//            base.OnClosing(e);
//        }

//        // Überschreiben der OnLoaded-Methode, um die Konfiguration beim Laden der Anwendung zu laden
//        protected override void OnContentRendered(EventArgs e)
//        {
//            LoadConfigurationFromFile();
//            base.OnContentRendered(e);
//        }


//    }
//}
