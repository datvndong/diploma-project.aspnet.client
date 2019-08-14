using System;
using System.Net.Mail;

namespace CentralizedDataSystem.Utils {
    public sealed class EmailUtil {
        public static EmailUtil Instance { get; } = new EmailUtil();
        private static SmtpClient smtpClient;
        private static MailMessage mailMessage;

        static EmailUtil() { }

        private EmailUtil() {
            string host = ConfigUtil.GetKeyConfig("SMTPHost");
            int port = int.Parse(ConfigUtil.GetKeyConfig("SMTPPort"));
            string fromEmail = ConfigUtil.GetKeyConfig("FromEmailAddress");
            string password = ConfigUtil.GetKeyConfig("FromEmailPassword");
            string fromName = ConfigUtil.GetKeyConfig("FromName");

            smtpClient = new SmtpClient(host, port) {
                // No using defaut
                UseDefaultCredentials = false,
                // Create a new credential
                Credentials = new System.Net.NetworkCredential(fromEmail, password),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = true,
                Timeout = 100000
            };

            mailMessage = new MailMessage {
                From = new MailAddress(fromEmail, fromName),
                BodyEncoding = System.Text.Encoding.UTF8,
                IsBodyHtml = true,
                Priority = MailPriority.High
            };
        }

        public void AddMailInfo(string subject, string content) {
            mailMessage.Subject = subject;
            mailMessage.Body = content;
        }

        public bool ProcessSend(string address) {
            try {
                mailMessage.To.Clear();
                mailMessage.To.Add(address);

                smtpClient.Send(mailMessage);
                return true;
            } catch (Exception) {
                return false;
            }
        }
    }
}