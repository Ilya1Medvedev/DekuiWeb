using BakWeb.Options;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace BakWeb.Services
{
    public class SendGridService
    {
        private readonly ISendGridClient _sendGridClient;
        private readonly IOptions<SendGridOptions> _sendGridOptions;

        public SendGridService(ISendGridClient sendGridClient, IOptions<SendGridOptions> sendGridOptions)
        {
            _sendGridClient = sendGridClient;
            _sendGridOptions = sendGridOptions;
        }

        public async Task SendEmailWithTemplateAsync(string toEmail, string templateId, object templateData)
        {
            var from = new EmailAddress(_sendGridOptions.Value.FromEmail, _sendGridOptions.Value.CompanyName);
            var to = new EmailAddress(toEmail);
            var message = new SendGridMessage
            {
                From = from,
                TemplateId = templateId,
            };
            message.AddTo(to);
            message.SetTemplateData(templateData);

            await _sendGridClient.SendEmailAsync(message);
        }
    }
}
