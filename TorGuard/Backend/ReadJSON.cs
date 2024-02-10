
namespace TorGuard;
public class ReadJSON {

           // Methode zum Laden der Labels und ListBox-Inhalte aus einer Datei
        private static List LoadConfigurationFromFile_Mail() {

            //return Liste der methode
            List<List> returnList = new List<List>();

            MessageBox.Show("Die Anwendung wurde geöffnet und initialisiert.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                try
                {
                    string filePath_Mail = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFilePath);
                    if (File.Exists(filePath_Mail))
                    {
                        string json = File.ReadAllText(filePath_Mail);
                        var configData = JsonSerializer.Deserialize<dynamic>(json);

                        

                        emailAddresses = ((JsonElement)configData?.EmailAddresses).EnumerateArray().Select(email => email.GetString()).ToList();


                        //erstelle Liste für E-MailSenderDaten
                        List<string> senderEmailDaten = new List<string>();

                      

                        txtsEmail.Text = configData?.SMTP?.Email;
                        txtEmailpassword.Password = configData?.SMTP?.Password;
                        txtsmtpadre.Text = configData?.SMTP?.Address;
                        txtSmtpNr.Text = configData?.SMTP?.Port;

                        //Hinzufügen von EmailDaten
                        senderEmailDaten.Add(configData?.SMTP?.Email, 
                                                configData?.SMTP?.Password,
                                                configData?.SMTP?.Address,
                                                configData?.SMTP?.Port);

                        RefreshEmailList();


                        //Beide Listen zurückgeben in einer Liste
                        return returnList.Add(emailAddresses,senderEmailDaten);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Laden der Konfiguration: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            

           
        }


}