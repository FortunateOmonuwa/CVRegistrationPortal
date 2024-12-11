﻿using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using Utilities.Configs;
using Utilities.Services.MailService;
using Utilities.Services.MailService.DTO;

namespace UserManagement.API.Service
{
    public class MailService : IMailService
    {
      
       private readonly AppSettings _settings;
        public MailService (IOptions<AppSettings> settings)
        {
            _settings = settings.Value;

           

        }
        public async Task<ResponseDetail<string>> SendMail(MailRequest mail)
        {
            try
            {
                var newBody = $"<h1>{mail.Body}</h1>";
                var response = new ResponseDetail<string>();

               

                var message = new MimeMessage
                {
                    To = { MailboxAddress.Parse(mail.Receiver) },
                    Sender = MailboxAddress.Parse(_settings.Sender),
                    Subject = mail.Subject,
                    Body = new TextPart(TextFormat.Html)
                    {
                        Text = newBody
                    },
                };

                using var client = new SmtpClient();
                client.Connect(_settings.Server, _settings.Port, SecureSocketOptions.StartTls);
                client.Authenticate(_settings.Sender, _settings.Password);
                await client.SendAsync(message);
                client.Disconnect(true);
                return response.SuccessResultData("Mail was successfully sent");
            }
            catch
            {
                throw;
            }
        }
    }
}