using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Blazorise.Extensions;

namespace Riverbed.Pricing.Paint.Shared.EmailService
{
    // Example usage:
    //  var emailService = new EmailService("m07.internetmailserver.net", 25, "steve@bludog-software.com", "E@*************1");
    //  emailService.SendEmail("noreply@bludog-software.com", "stucker@360painting.com", "Test Email", "This is a test email sent from C#.");
    public class EmailService : IEmailService
    {
        private string smtpHost;
        private int smtpPort;
        private string smtpUser;
        private string smtpPass;
        private string fromAddress = "quotes@riverbed-software.com";

        public EmailService()
        {
            this.smtpHost = "mail.smtp2go.com";
            this.smtpPort = 587;
            this.smtpUser = "notifications@riverbed-software.com";
            this.smtpPass = "1WIjo1G8iblBK689";
        }

        public EmailService(string host, int port, string username, string password)
        {
            smtpHost = host;
            smtpPort = port;
            smtpUser = username;
            smtpPass = password;

        }

        public string SendEmail(string toAddress, string subject, string body, string senderName, string senderEmail, string ccEmail, string bccEmail, bool isHtml = true)
        {
            try
            {
                using (var client = new SmtpClient(smtpHost, smtpPort))
                {
                    client.Credentials = new NetworkCredential(smtpUser, smtpPass);
                    client.EnableSsl = true;
                    var fromMailAddress = new MailAddress(fromAddress, senderName);

                    // Sanitize any escaped quotes and ensure HTML document/head
                    var sanitizedBody = NormalizeQuoteEscapes(body);
                    var finalBody = isHtml ? EnsureHtmlDocumentWithHead(sanitizedBody) : sanitizedBody;

                    var mailMessage = new MailMessage
                    {
                        From = fromMailAddress,
                        Subject = subject,
                        Body = finalBody,
                        IsBodyHtml = isHtml
                    };

                    // Support multiple recipients separated by ; or ,
                    AddAddresses(mailMessage.To, toAddress);
                    AddAddresses(mailMessage.CC, ccEmail, "New Quote Notification");
                    AddAddresses(mailMessage.Bcc, bccEmail, "New Quote Notification");

                    if (!senderEmail.IsNullOrEmpty())
                        AddAddresses(mailMessage.ReplyToList, senderEmail, senderName);

                    client.Send(mailMessage);
                    return "Email sent successfully.";      
                }
            }
            catch (Exception ex)
            {
                return $"Error sending email: {ex.Message}";
            }
        }

        /// <summary>
        /// Parses a semicolon or comma-delimited string of email addresses and adds each to the collection.
        /// </summary>
        private static void AddAddresses(MailAddressCollection collection, string addresses, string displayName = "")
        {
            if (string.IsNullOrWhiteSpace(addresses))
                return;

            var separators = new[] { ';', ',' };
            foreach (var part in addresses.Split(separators, StringSplitOptions.RemoveEmptyEntries))
            {
                var trimmed = part.Trim();
                if (trimmed.Length > 0)
                {
                    collection.Add(string.IsNullOrEmpty(displayName)
                        ? new MailAddress(trimmed)
                        : new MailAddress(trimmed, displayName));
                }
            }
        }

        private static string NormalizeQuoteEscapes(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            // Replace escaped quotes (\") with a plain double-quote (")
            return input.Replace("\\\"", "\"");
        }

        private static string EnsureHtmlDocumentWithHead(string body)
        {
            if (string.IsNullOrWhiteSpace(body)) body = string.Empty;
            var trimmed = body.TrimStart();
            trimmed = trimmed.Replace("\r\n", "").Replace("\n", "").Replace("\r", "");

            // If not an HTML document, wrap with full HTML including the proper head
            if (!(trimmed.StartsWith("<!DOCTYPE", StringComparison.OrdinalIgnoreCase) ||
                  trimmed.IndexOf("<html", StringComparison.OrdinalIgnoreCase) >= 0))
            {
                var sb = new StringBuilder();
                sb.Append("<!DOCTYPE html>");
                sb.Append("<html><head>");
                sb.Append(GetHeadTags());
                sb.Append("</head><body>");
                sb.Append(body);
                sb.Append("</body></html>");
                return sb.ToString();
            }

            return trimmed;
        }

        private static string GetHeadTags()
        {
           var injection = @"<meta charset=""UTF-8""><title></title><meta name=""x-apple-disable-message-reformatting""><meta name=""viewport"" content=""width=device-width, initial-scale=1"">";

            // Fallback: prepend a full head block
            return $"{injection}";
        }
       
    }
}
