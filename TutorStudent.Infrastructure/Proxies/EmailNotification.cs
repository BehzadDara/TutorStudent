using MailKit;
using MailKit.Net.Proxy;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TutorStudent.Domain.Models;
using TutorStudent.Domain.ProxyServices;
using TutorStudent.Domain.ProxyServices.Dto;

namespace TutorStudent.Infrastructure.Proxies
{
    public class EmailNotification<T> : INotification<T> where T : EmailContextDto
    {
        private readonly MailSettings _mailSettings;
        public EmailNotification(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task<bool> Send(T context)
        {

            var email = new MimeMessage
            {
                Sender = MailboxAddress.Parse(_mailSettings.Mail),
                Subject = context.Subject
            };
            email.To.Add(MailboxAddress.Parse(context.To));
            var multipart = new Multipart("mixed")
            {
                new TextPart("plain")
                {
                    Text = context.Body
                }
            };

            // attachment
            if (context.Attachment != null && context.Attachment.Length > 0)
            {
                var ms = new MemoryStream(context.Attachment, 0, context.Attachment.Length);

                var content = new MimeContent(ms); 

                var contentDisposition = new ContentDisposition(ContentDisposition.Attachment);
                var contentTransferEncoding = ContentEncoding.Base64;
                var fileName = "Attachment";

                var attachment = new MimePart("ics")
                {
                    Content = new MimeContent(ms),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = "Attachment"
                };

                multipart.Add(attachment);
            }

            email.Body = multipart;

            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);

            return await Task.FromResult(true);
        }
    }
}
