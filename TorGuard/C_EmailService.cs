using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MailKit.Net.Smtp;
using MimeKit;

namespace TorGuard
{
    public class C_EmailService
    {



        // Methode zum asynchronen Versenden von E-Mails
        public async Task<bool> EmailsAsync(List<string> empfängeradressen)
        {
            bool allSuccess = true;

            foreach (string empfängeradresse in empfängeradressen)
            {
                bool success = await SendEmailAsync(empfängeradresse);
                allSuccess &= success; // Setzt allSuccess auf false, wenn irgendein Aufruf von SendTestEmailAsync false zurückgibt
            }
            return allSuccess;
        }

        //Methode zum asynchronen Versenden einer E-Mail
        public async Task<bool> SendEmailAsync(string empfängeradresse)
        {

            try
            {   //Email Kopf angaben 

                MimeMessage email = new MimeMessage();

                //email.From.Add(new MailboxAddress("Absender", "Tor_Guard@web.de"));
                //email.To.Add(new MailboxAddress("Empfänger", "lementken@gmail.com"));

                email.From.Add(new MailboxAddress("Absender", configData?.SMTP?.Address));
                email.To.Add(new MailboxAddress("Empfänger", empfängeradresse));


                email.Subject = $" Unfallberricht Tor_Guard vom {DateTime.Now:dd.MM.yyyy um HH.mm.ss}";


                email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = "<b>$@\"Marvin kasst du was für den Unfall berichtstextst formulieren ? Mit bedauern müssen wir ihnen mitteilen das in ihrer Firmer das Torxxx von xxx  am xx.xxx.xx um xx.xx beschädigt wurde bitte überprüfen Sie die Video aufzeichnung  im besagten Zeitpunkt . Sie finden Die aufzeichungen unter xxxxxxx \" };\r\n.</b>"
                };

                using (var smtp = new SmtpClient())// Email versant 
                {
                    //smtp.Connect("smtp.web.de", 587, true); //smtp-serveradresse,Port Nr., gesicherteverbindung erzwinge = true#
                    //smtp.Connect("smtp.web.de", 465, MailKit.Security.SecureSocketOptions.SslOnConnect);
                    //smtp.Connect("smtp.web.de", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    smtp.Connect(configData?.SMTP?.Email, configData?.SMTP?.Port, MailKit.Security.SecureSocketOptions.SslOnConnect);

                    // smtp.Authenticate("Tor_Guard@web.de", "TorGuard");
                    smtp.Authenticate(configData?.SMTP?.Address, configData?.SMTP?.Password);//Daten aus der Maske  // Nur erforderlich, wenn der SMTP-Server eine Authentifizierung erfordert

                    await smtp.SendAsync(email);//await verwendet, um asynchrone Operationen zu kennzeichnen
                    smtp.Disconnect(true);

                    return true; // Erfolg
                }
            }
            catch (MailKit.Security.AuthenticationException)
            {
                MessageBox.Show($"Login fehlgeschlagen. Überprüfen Sie die ihre Anmelde Informationene für {configData?.SMTP?.Address}.Wenn das nicht hilft überprüfe ob du dich Einloggen kannst, eventull wegen nicht vertrauenswürdig gesperrt.", "Authentifizierungsfehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return false; // Authentifizierungsfehler
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Versenden der E-Mail an {empfängeradresse}: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return false; // Sonstiger Fehler
            }
        }
    }
}
