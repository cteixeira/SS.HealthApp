using System.Linq;
using System.Collections.Generic;
using SS.HealthApp.Model.DeclarationModels;
using SS.HealthApp.ClientConnector.Interfaces;
using System.IO;
using System.Net.Mail;
using System.Configuration;
using System.Net;

namespace SS.HealthApp.Core.Services {

    public class DeclarationService {

        public List<PresenceDeclaration> GetPresenceDeclaration()
        {
            IDeclarationClientConnector service = 
                ClientConnector.ClientConnectorManager.Instance.Create<IDeclarationClientConnector>(CoreContext.CurrentUser.CompanyId);
            return service.GetPresenceDeclaration(CoreContext.CurrentUser.ConvertToClientConnectorModel()).OrderByDescending(d => d.Moment).ToList();
        }

        public bool SendPresenceDeclarationFileByEmail(string PresenceDeclarationID)
        {

            Utils.UsageMetric.Add(GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, null);

            PresenceDeclaration presenceDeclaration = GetPresenceDeclaration().Find(pd => pd.ID == PresenceDeclarationID);

            if (presenceDeclaration == null) throw new System.Exception("Presence Declaration does not exist or user has no access.");

            IDeclarationClientConnector service =
                ClientConnector.ClientConnectorManager.Instance.Create<IDeclarationClientConnector>(CoreContext.CurrentUser.CompanyId);

            byte[] file = service.GetPresenceDeclarationFile(CoreContext.CurrentUser.ConvertToClientConnectorModel(), PresenceDeclarationID);

            if (file == null) throw new System.Exception("No file was returned.");

            using (MemoryStream pdfStream = new MemoryStream(file))
            {

                Attachment attachment = new Attachment(pdfStream, string.Format("{0}.pdf", presenceDeclaration.ID));

                string userEmail = new UserService().GetPersonalData().Email;
                if (string.IsNullOrEmpty(userEmail))
                    return false;

                //Enviar por e-mail para o utente
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(Utils.Resources.GetResource("EmailSender"));
                msg.To.Add(new MailAddress(userEmail));
                msg.Subject = Utils.Resources.GetResource("PresenceDeclarationSendFileSubject");
                msg.Body = Utils.Resources.GetResource("PresenceDeclarationSendFileBody");
                msg.IsBodyHtml = true;

                string attachmentPath = ConfigurationManager.AppSettings["PastaImagens"] + @"\LogoSAMS.png";
                Attachment inline = new Attachment(attachmentPath);
                inline.ContentDisposition.Inline = true;
                inline.ContentDisposition.DispositionType = System.Net.Mime.DispositionTypeNames.Inline;
                inline.ContentId = "LogoSAMS";
                inline.ContentType.MediaType = "image/png";
                inline.ContentType.Name = Path.GetFileName(attachmentPath);

                msg.Attachments.Add(inline);

                msg.Attachments.Add(attachment);

                using (SmtpClient smtp = Utils.Email.GetSmtp())
                {
                    smtp.Send(msg);
                }
            }

            return true;
        }

        public byte[] GetPresenceDeclarationFile(string PresenceDelcarationID)
        {
            Utils.UsageMetric.Add(GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, null);

            PresenceDeclaration doc = GetPresenceDeclaration().Find(f => f.ID == PresenceDelcarationID);

            if (doc == null) throw new System.Exception("Document does not exist or user has no access.");

            IDeclarationClientConnector service =
                ClientConnector.ClientConnectorManager.Instance.Create<IDeclarationClientConnector>(CoreContext.CurrentUser.CompanyId);

            byte[] file = service.GetPresenceDeclarationFile(CoreContext.CurrentUser.ConvertToClientConnectorModel(), PresenceDelcarationID);

            if (file == null) throw new System.Exception("No file was returned.");

            return file;

        }

        public bool SendPresenceDeclarationFileByEmailByAppointmentId(string AppointmentId)
        {

            Utils.UsageMetric.Add(GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, null);
            IDeclarationClientConnector service =
                ClientConnector.ClientConnectorManager.Instance.Create<IDeclarationClientConnector>(CoreContext.CurrentUser.CompanyId);
            string declarationId = service.GetPresenceDeclarationIdByAppointmentID(CoreContext.CurrentUser.ConvertToClientConnectorModel(), AppointmentId);

            return SendPresenceDeclarationFileByEmail(declarationId);


        }

        public byte[] GetPresenceDeclarationFileAppointmentId(string AppointmentId)
        {
            Utils.UsageMetric.Add(GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, null);

            Utils.UsageMetric.Add(GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, null);
            IDeclarationClientConnector service =
                ClientConnector.ClientConnectorManager.Instance.Create<IDeclarationClientConnector>(CoreContext.CurrentUser.CompanyId);
            string declarationId = service.GetPresenceDeclarationIdByAppointmentID(CoreContext.CurrentUser.ConvertToClientConnectorModel(), AppointmentId);

            return GetPresenceDeclarationFile(declarationId);

        }

    }
}
