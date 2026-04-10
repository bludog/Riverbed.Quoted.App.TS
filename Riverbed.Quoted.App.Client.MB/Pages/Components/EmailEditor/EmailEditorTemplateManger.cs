using Riverbed.Pricing.Paint.Shared.Entities.Emails;

namespace Riverbed.Pricing.Paint.Client.Pages.Components.EmailEditor
{
    public partial class EmailEditorTemplateManger : IEmailEditorTemplateManger
    {
        // Return the email template by template id
        public EmailTemplate GetEmailTemplateById(EmailType templateId)
        {
            return CreateEmailTemplate(templateId);
        }

        // Create a new email template for each type of email: Customer link to quote, Company Notification of new quote,
        // Company Notification of Accepted Quote, Customer Notification of Accepted Quote
        public EmailTemplate CreateEmailTemplate(EmailType emailType)
        {
            var emailTemplate = new EmailTemplate
            {
                Name = emailType.ToString(),
                Subject = emailType.ToString(),
                Body = GetEmailTemplateByType(emailType),
                CcEmail = "",
                IsHtml = true
            };

            return emailTemplate;
        }

        // Return email template by email type
        public string GetEmailTemplateByType(EmailType emailType)
        {
            switch (emailType)
            {
                case EmailType.CustomerLinkToQuote:
                    return GetCustomerLinkToQuoteEmailTemplate();
                case EmailType.CompanyNotificationOfNewQuote:
                    return GetCompanyNotificationOfNewQuoteEmailTemplate();
                case EmailType.CompanyNotificationOfAcceptedQuote:
                    return GetCompanyNotificationOfAcceptedQuoteEmailTemplate();
                case EmailType.CustomerNotificationOfAcceptedQuote:
                    return GetCustomerNotificationOfAcceptedQuoteEmailTemplate();
                default:
                    return GetCustomerLinkToQuoteEmailTemplate();
            }
        }

        // Return string email template for Customer link to quote
        public string GetCustomerLinkToQuoteEmailTemplate()
        {
            return @"
                    <p>Dear {0},</p>
                    <p>Thank you so much for considering {1} for your painting project.</p>
                    <p>We appreciate this opportunity to earn your business. With {1} you can be assured the project will be done right the first time without hidden charges or hassles.</p>
                    <p>Your quote can be viewed by going to the following link: <a href='{2}'>Quote Link</a></p>
                    <p>Please look it over and let us know how you would like to proceed.</p>
                    <p>If you have any questions please feel free to contact us at {4} or <a href='mailto:{5}'>{5}</a>.</p>     
                ";
        }

        // Return string email template for Company Notification of new quote
        public string GetCompanyNotificationOfNewQuoteEmailTemplate()
        {
            var createdDate = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");
            return $@"
                A new Client {{0}} created a new Quote.<p/>                
                Time: {createdDate}<p/>
                Quote link: <a href='{{2}}'>Quote Link</a></p>             
                ";
        }

        // Return string email template for Company Notification of Accepted Quote
        public string GetCompanyNotificationOfAcceptedQuoteEmailTemplate()
        {
            var createdDate = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");
            return $@"
                Client {{0}} just Accepted their Quote.<p/>                
                Time: {createdDate}<p/>
                Quote link: <a href='{{2}}'>Quote Link</a></p>             
                ";
        }

        // Return string email template for Customer Notification of Accepted Quote
        public string GetCustomerNotificationOfAcceptedQuoteEmailTemplate()
        {
            var createdDate = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");
            return $@"
                Thank-you {{0}}.<p/>
                You should receive a call from our office to schedule your project.<p/>             
                Quote link: <a href='{{2}}'>Quote Link</a></p>            
                ";
        }
    }
}
