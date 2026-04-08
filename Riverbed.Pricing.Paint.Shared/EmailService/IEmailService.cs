namespace Riverbed.Pricing.Paint.Shared.EmailService
{
    public interface IEmailService
    {
        string SendEmail(string toAddress, string subject, string body, string senderName, string senderEmail, string ccEmail, string bccEmail, bool isHtml = true);
    }
}