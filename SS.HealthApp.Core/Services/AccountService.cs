using System.Collections.Generic;
using System.Linq;
using SS.HealthApp.Model.AccountModels;
using SS.HealthApp.ClientConnector.Interfaces;
using System.IO;
using System.Configuration;
using System.Net.Mail;
using System;

namespace SS.HealthApp.Core.Services
{
    public class AccountService {

        public List<AccountStatement> GetAccountStatment() {

            string cacheKey = GetCacheKey();

            //check cache
            List<AccountStatement> accountStatment = CacheManager<List<AccountStatement>>.Get(cacheKey);
            if (accountStatment != null)
                return accountStatment;

            IAccountClientConnector service =
                ClientConnector.ClientConnectorManager.Instance.Create<IAccountClientConnector>(CoreContext.CurrentUser.CompanyId);
            accountStatment = service.GetAccountStatement(CoreContext.CurrentUser.ConvertToClientConnectorModel());

            //save on cache
            if (accountStatment != null)
            {
                accountStatment = accountStatment.OrderByDescending(d => d.Date).ToList();
                CacheManager<List<AccountStatement>>.Save(accountStatment, cacheKey, GetCacheExpiration());
            }

            return accountStatment;
        }

        public bool SendAccountStatmentFileByEmail(string documentID) {

            Utils.UsageMetric.Add(GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, null);

            AccountStatement doc = GetAccountStatment().Find(f => f.ID == documentID);

            if (doc == null) throw new System.Exception("Document does not exist or user has no access.");

            IAccountClientConnector service =
                ClientConnector.ClientConnectorManager.Instance.Create<IAccountClientConnector>(CoreContext.CurrentUser.CompanyId);

            byte[] file = service.GetAccountStatementFile(CoreContext.CurrentUser.ConvertToClientConnectorModel(), documentID);

            if (file == null) throw new System.Exception("No file was returned.");

            using (MemoryStream pdfStream = new MemoryStream(file)) {

                Attachment attachment = new Attachment(pdfStream, string.Format("{0}.pdf", doc.Description));

                string userEmail = new UserService().GetPersonalData().Email;
                if (string.IsNullOrEmpty(userEmail))
                    return false;

                //Enviar por e-mail para o utente
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(Utils.Resources.GetResource("EmailSender"));
                msg.To.Add(new MailAddress(userEmail));
                msg.Subject = Utils.Resources.GetResource("AccountSendDocumentSubject");
                msg.Body = Utils.Resources.GetResource("AccountSendDocumentBody");
                msg.IsBodyHtml = true;
                msg.Attachments.Add(attachment);

                using (SmtpClient smtp = Utils.Email.GetSmtp())
                {
                    smtp.Send(msg);
                }
            }

            return true;
        }

        public byte[] GetAccountStatmentFile(string documentID) {

            Utils.UsageMetric.Add(GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, null);

            AccountStatement doc = GetAccountStatment().Find(f => f.ID == documentID);

            if (doc == null) throw new System.Exception("Document does not exist or user has no access.");

            IAccountClientConnector service =
                ClientConnector.ClientConnectorManager.Instance.Create<IAccountClientConnector>(CoreContext.CurrentUser.CompanyId);

            byte[] file = service.GetAccountStatementFile(CoreContext.CurrentUser.ConvertToClientConnectorModel(), documentID);

            if (file == null) throw new System.Exception("No file was returned.");

            return file;

        }

        private string GetCacheKey()
        {
            return string.Format("AccountStatment_Company_{0}_User_{1}", CoreContext.CurrentUser.CompanyId, CoreContext.CurrentUser.Id);
        }

        private DateTimeOffset GetCacheExpiration()
        {
            return DateTimeOffset.Now.AddMinutes(Int32.Parse(ConfigurationManager.AppSettings["NewsCacheExpirationMinutes"]));
        }

    }
}
