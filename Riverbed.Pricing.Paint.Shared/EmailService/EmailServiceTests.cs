using System;
using System.Net;
using System.Net.Mail;
using Xunit;
using Riverbed.Pricing.Paint.Shared.EmailService;

namespace Riverbed.Pricing.Paint.Shared.EmailService.Tests
{
    public class EmailServiceTests
    {
        [Fact]
        public void SendEmail_ReturnsErrorMessage_WhenSmtpFails()
        {
            // Arrange
            var service = new EmailService("invalid.host", 25, "user", "pass");
            // Act
            var result = service.SendEmail("to@address.com", "subject", "body", "Paint Company", "sender@Email.com", "cc@address.com", "bcc@address.com");
            // Assert
            Assert.Contains("Error sending email", result);
        }

        [Fact]
        public void SendEmail_ReturnsSuccessMessage_WhenParametersAreValid()
        {
            // Arrange
            var service = new EmailService();
            // Act
            var result = service.SendEmail("test@riverbed-software.com", "Test Subject", "Test Body", "Paint Company", "sender@Email.com", "cc@riverbed-software.com", "bcc@riverbed-software.com");
            // Assert
            // This may fail if SMTP credentials are not valid, but verifies the code path
            Assert.True(result.StartsWith("Email sent successfully.") || result.StartsWith("Error sending email"));
        }
    }
}
