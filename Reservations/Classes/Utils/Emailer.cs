using Reservations.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace Reservations.Classes.Utils
{
    public class Emailer
    {

        string smtpHost, smtpOutgoing;
        SmtpClient smtpClient;
        string templatePath;
        string EmailFrom;

        public Emailer()
        {
            smtpOutgoing = ConfigurationManager.AppSettings["smtpOutgoing"].ToString(); // ползва се само за WelcomeMail
            smtpHost = ConfigurationManager.AppSettings["smtpHost"].ToString();

            templatePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + ConfigurationManager.AppSettings["emailTemplates"].ToString();

            EmailFrom = ConfigurationManager.AppSettings["EmailFrom"].ToString();
        }

        public void SendMailForDateToDateRequests(DateTime? BegDate, DateTime? EndDate, string mailTo, List<Room> roomList, decimal? price)
        {
            string emailBody = GetTemplate("RequestFromDateToDate.txt");

            MailMessage mailMsg = new MailMessage();

            mailMsg.From = new MailAddress(EmailFrom);

            mailMsg.To.Add(new MailAddress(mailTo));

            mailMsg.IsBodyHtml = true;

            mailMsg.Subject = "Потвърждение на резервация";

            List<int?> roomNumbers = new List<int?>();
            foreach (var room in roomList)
            {
                roomNumbers.Add(room.RoomID);
            }

            DateTime _begDate = (DateTime)BegDate;
            string sBegDate = _begDate.ToString("dd/M/yyyy", CultureInfo.InvariantCulture);
            DateTime _endDate = (DateTime)EndDate;
            string sEndDate = _endDate.ToString("dd/M/yyyy", CultureInfo.InvariantCulture);

            mailMsg.Body = string.Format(emailBody,
                                         sBegDate,
                                         sEndDate,
                                         string.Join(", ", roomNumbers),
                                         price
                                        );

            

            SendMail(mailMsg);
        }

        public void SendMailForPeriodRequests(DateTime? BegDate, DateTime? EndDate, string mailTo, List<Room> roomList, decimal? price)
        {
            string emailBody = GetTemplate("RequestForFullPeriods.txt");

            MailMessage mailMsg = new MailMessage();

            mailMsg.From = new MailAddress(EmailFrom);

            mailMsg.To.Add(new MailAddress(mailTo));

            mailMsg.IsBodyHtml = true;

            mailMsg.Subject = "Потвърждение на резервация";

            List<int?> roomNumbers = new List<int?>();
            foreach (var room in roomList)
            {
                roomNumbers.Add(room.RoomID);
            }

            DateTime _begDate = (DateTime)BegDate;
            string sBegDate = _begDate.ToString("dd/M/yyyy", CultureInfo.InvariantCulture);
            DateTime _endDate = (DateTime)EndDate;
            string sEndDate = _endDate.ToString("dd/M/yyyy", CultureInfo.InvariantCulture);


            mailMsg.Body = string.Format(emailBody,
                                         sBegDate,
                                         sEndDate,
                                         string.Join(", ", roomNumbers),
                                         price
                                        );



            SendMail(mailMsg);
        }

        private void SendMail(MailMessage mailMsg, bool useSMTP = true)
        {
            if (mailMsg == null ||
                mailMsg.To.Count == 0 ||
                mailMsg.From == null
                )
            {
                return;
            }


            if (useSMTP)
                smtpClient = new SmtpClient(smtpHost);
            else
                smtpClient = new SmtpClient(smtpOutgoing);

            smtpClient.Credentials = CredentialCache.DefaultNetworkCredentials;

            try
            {
                smtpClient.Send(mailMsg);
            }
            catch (Exception e)
            {
                string err = e.Message;

                if (e.InnerException != null)
                    err += e.InnerException.Message;

                err += string.Format("Mail from {0} to {1}", EmailFrom, mailMsg.To.FirstOrDefault().Address);
            }
        }

        private string GetTemplate(string template)
        {
            string content = File.ReadAllText(string.Format("{0}\\{1}", templatePath, template));

            return content;
        }

        private void AddToMails(MailAddressCollection toMails, string mails)
        {
            if (string.IsNullOrWhiteSpace(mails))
                return;

            string[] split = mails.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string mail in split)
                toMails.Add(mail);
        }
    }
}