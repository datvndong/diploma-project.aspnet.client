using CentralizedDataSystem.Models;
using CentralizedDataSystem.Resources;
using CentralizedDataSystem.Services.Interfaces;
using CentralizedDataSystem.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace CentralizedDataSystem.Services.Implements {
    public class SendEmailService : ISendEmailService {
        private readonly IGroupService _groupService;
        private readonly IUserService _userService;

        public SendEmailService(IGroupService groupService, IUserService userService) {
            _groupService = groupService;
            _userService = userService;
        }

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
            } catch (Exception) {
                return false;
            }
        }

        private List<User> CreateListUserFromJson(List<User> users, string content) {
            JArray jArray = JArray.Parse(content);
            JObject dataObject = null;
            foreach (JObject jsonObject in jArray) {
                dataObject = (JObject)jsonObject.GetValue(Keywords.DATA);

                if ((int)dataObject.GetValue(Keywords.STATUS) == Configs.DEACTIVE_STATUS) {
                    continue;
                }

                string email = dataObject.GetValue(Keywords.EMAIL).ToString();

                users.Add(new User(email));
            }

            return users;
        }

        private async Task<List<User>> FindListUsersForSendEmailByAssign(string token, string assign, List<User> users) {
            string groupsData = await _groupService.FindAllGroupsByIdParent(token, assign);
            string usersData = await _userService.FindUsersByPageAndIdGroup(token, assign, 0);

            users = CreateListUserFromJson(users, usersData);

            string idGroup = string.Empty;
            JArray groupsArray = JArray.Parse(groupsData);
            JObject dataObject = null;
            foreach (JObject jObject in groupsArray) {
                dataObject = (JObject)jObject.GetValue(Keywords.DATA);
                idGroup = dataObject.GetValue(Keywords.ID_GROUP).ToString();
                users = await FindListUsersForSendEmailByAssign(token, idGroup, users);
            }

            return users;
        }

        public async Task<string> SendEmail(string token, string assign, string nameForm) {
            List<string> errorMails = new List<string>();
            List<User> users = null;
            string subject = Messages.MAIL_SUBJECT(nameForm);

            string content = System.IO.File.ReadAllText(HostingEnvironment.MapPath(ViewName.EMAIL_TEMPLATE));
            content = content.Replace("{{title}}", Messages.MAIL_TITLE);

            if (assign.Equals(Keywords.AUTHENTICATED)) {
                string res = await _userService.FindAllUsers(token);

                users = CreateListUserFromJson(new List<User>(), res);
            } else {
                users = await FindListUsersForSendEmailByAssign(token, assign, new List<User>());
            }

            bool sendResult = false;
            string email = string.Empty;
            foreach (User user in users) {
                email = user.Email;
                sendResult = ProcessSend(email, subject, content);
                if (!sendResult) {
                    errorMails.Add(email);
                }
            }

            return string.Join(Environment.NewLine, errorMails);
        }
    }
}