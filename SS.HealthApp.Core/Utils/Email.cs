using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace SS.HealthApp.Core.Utils {
    internal class Email
    {

        public static SmtpClient GetSmtp()
        {
            SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"]);
            if (bool.Parse(ConfigurationManager.AppSettings["SmtpAuthentication"]))
            {
                var wcSec = new SimpleSolutions.wcSecure.Decifra();
                smtp.Credentials = new NetworkCredential(
                    wcSec.Decifrar(ConfigurationManager.AppSettings["SmtpUser"]),
                    wcSec.Decifrar(ConfigurationManager.AppSettings["SmtpPassword"]));
            }
            return smtp;
        }

       

    }
}
