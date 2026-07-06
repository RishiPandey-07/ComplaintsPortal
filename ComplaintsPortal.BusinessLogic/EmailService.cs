using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using ComplaintsPortal.Entities;

namespace ComplaintsPortal.BusinessLogic
{
    public class EmailService
    {
        private readonly bool _enabled;
        private readonly string _host;
        private readonly int _port;
        private readonly string _user;
        private readonly string _pass;
        private readonly bool _enableSsl;
        private readonly string _fromAddress;

        public EmailService()
        {
            _enabled = ConfigurationManager.AppSettings["EnableEmailNotifications"] == "true";
            _host = ConfigurationManager.AppSettings["SmtpHost"];
            int.TryParse(ConfigurationManager.AppSettings["SmtpPort"], out _port);
            if (_port == 0) _port = 587; // default
            _user = ConfigurationManager.AppSettings["SmtpUser"];
            _pass = ConfigurationManager.AppSettings["SmtpPass"];
            _enableSsl = ConfigurationManager.AppSettings["SmtpEnableSsl"] == "true";
            _fromAddress = ConfigurationManager.AppSettings["SmtpFromAddress"] ?? "noreply@complaintsportal.local";
        }

        public void SendEmailAsync(EmailMessage message)
        {
            if (!_enabled || string.IsNullOrEmpty(_host) || string.IsNullOrEmpty(message.To))
            {
                return;
            }

            Task.Run(() =>
            {
                try
                {
                    using (var client = new SmtpClient(_host, _port))
                    {
                        if (!string.IsNullOrEmpty(_user))
                        {
                            client.Credentials = new NetworkCredential(_user, _pass);
                        }
                        client.EnableSsl = _enableSsl;

                        var mailMessage = new MailMessage
                        {
                            From = new MailAddress(_fromAddress, "IT Services Portal"),
                            Subject = message.Subject,
                            Body = message.Body,
                            IsBodyHtml = message.IsHtml
                        };

                        // Support multiple recipients separated by comma or semicolon
                        foreach (var to in message.To.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            mailMessage.To.Add(to.Trim());
                        }

                        client.Send(mailMessage);
                    }
                }
                catch (Exception ex)
                {
                    // In a production app, log this error using a proper logging framework or to the Event Log.
                    // E.g., LogError(ex);
                    System.Diagnostics.Debug.WriteLine("Email sending failed: " + ex.Message);
                }
            });
        }
    }
}
