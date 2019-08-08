using CentralizedDataSystem.Services.Interfaces;
using CentralizedDataSystem.Utils;
using System;
using System.Net.Mail;

namespace CentralizedDataSystem.Services.Implements {
    public class SendEmailService : ISendEmailService {
        private bool ProcessSend(string targetEmail, string subject, string content) {
            try {
                string host = ConfigUtil.GetKeyConfig("SMTPHost");
                int port = int.Parse(ConfigUtil.GetKeyConfig("SMTPPort"));
                string fromEmail = ConfigUtil.GetKeyConfig("FromEmailAddress");
                string password = ConfigUtil.GetKeyConfig("FromEmailPassword");
                string fromName = ConfigUtil.GetKeyConfig("FromName");

                SmtpClient smtpClient = new SmtpClient(host, port) {
                    // No using defaut
                    UseDefaultCredentials = false,
                    // Create a new credential
                    Credentials = new System.Net.NetworkCredential(fromEmail, password),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    EnableSsl = true,
                    Timeout = 100000
                };

                MailMessage mail = new MailMessage {
                    Body = content,
                    Subject = subject,
                    From = new MailAddress(fromEmail, fromName)
                };

                mail.To.Add(new MailAddress(targetEmail));
                mail.BodyEncoding = System.Text.Encoding.UTF8;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;

                smtpClient.Send(mail);

                return true;
            } catch (SmtpException) {
                return false;
            }
        }

        public void SendEmail(string email, string nameForm, string content) {
            try {
                content = content.Replace("{{title}}", "Notification from C.D System");

                ProcessSend(email, "You've got new report named \"" + nameForm + "\"", content);
            } catch (Exception ex) {
                throw ex;
            }
        }
    }
}