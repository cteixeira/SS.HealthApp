
using System;
using System.Configuration;
using System.Net.Mail;

namespace SS.HealthApp.Core.Utils {
    public class ErrorLog {
        
        public static void Add(string source, string error) {
            WriteTableError(1, source, error, CoreContext.CurrentUser);
            string msg = String.Format("CompanyId: {0} | UserName: {1} | {2}", 
                CoreContext.CurrentUser.CompanyId, 
                CoreContext.CurrentUser.Username,
                error);
            SendMailError(msg.Replace("\r\n", ""));
        }

        private static void WriteTableError(short priority, string source, string description, Models.AuthenticatedUser user) {
            try {
                using (Models.Context ctx = new Models.Context()) {
                    ctx.ErrorLog.Add(new Models.ErrorLog() {
                        Priority = priority,
                        Source = source,
                        Description = description,
                        Moment = DateTime.Now,
                        CompanyID = user.CompanyId,
                        UserID = user.Id
                    });
                    ctx.SaveChanges();
                }
            }
            catch {}
        }

        private static void SendMailError(string msg) {
            try {
                MailMessage ErrorMail = new MailMessage(ConfigurationManager.AppSettings["AppEmail"],
                   ConfigurationManager.AppSettings["SysAdminEmail"],
                   ConfigurationManager.AppSettings["ErrorEmailSubject"],
                   msg);

                ErrorMail.IsBodyHtml = false;
                SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"]);
                smtp.Port = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
                AuthenticateSMTP(smtp);
                smtp.Send(ErrorMail);
            }
            catch {}
        }

        private static void AuthenticateSMTP(SmtpClient smtp) {
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["AutenticaSMTP"])) {
                SimpleSolutions.wcSecure.Decifra decifra = new SimpleSolutions.wcSecure.Decifra();
                smtp.Credentials = new System.Net.NetworkCredential(decifra.Decifrar(ConfigurationManager.AppSettings["SmtpUser"]), decifra.Decifrar(ConfigurationManager.AppSettings["SmtpPassword"]));
            }
        }

    }
}
