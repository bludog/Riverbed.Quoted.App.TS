using Riverbed.Pricing.Paint.Shared.Entities.Emails;
using static Riverbed.Pricing.Paint.Client.Pages.Components.EmailEditor.EmailEditorTemplateManger;

namespace Riverbed.Pricing.Paint.Client.Pages.Components.EmailEditor
{
    public interface IEmailEditorTemplateManger
    {
        public EmailTemplate GetEmailTemplateById(EmailType templateId);
    }
}