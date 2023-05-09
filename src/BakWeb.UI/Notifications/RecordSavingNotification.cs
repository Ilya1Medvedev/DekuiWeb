using BakWeb.Extensions;
using BakWeb.Options;
using BakWeb.Services;
using BakWeb.TerminalControllerClient;
using BakWeb.TerminalControllerClient.Models;
using Microsoft.Extensions.Options;
using System.Runtime.ConstrainedExecution;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Core.Services.Notifications;

namespace BakWeb.Notifications
{
    public class TestSiteComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.AddNotificationAsyncHandler<ContentPublishingNotification, ProductSavedNotificationHandler>();
        }
    }

    public class ProductSavedNotificationHandler : INotificationAsyncHandler<ContentPublishingNotification>
    {
        private readonly int ReservationCodeLength = 6;

        private readonly SendGridService _sendGridService;
        private readonly IMemberService _memberService;
        private readonly IRecordReaderService _recordReaderService;
        private readonly IOptions<SendGridOptions> _sendGridOptions;
        private readonly TerminalClient _terminalClient;
        private readonly IOptions<FormOptions> _formOptions;

        public ProductSavedNotificationHandler(SendGridService sendGridService, IMemberService memberService,
            IRecordReaderService recordReaderService, IOptions<SendGridOptions> sendGridOptions, TerminalClient terminalClient,
            IOptions<FormOptions> formOptions)
        {
            _sendGridService = sendGridService;
            _memberService = memberService;
            _recordReaderService = recordReaderService;
            _sendGridOptions = sendGridOptions;
            _terminalClient = terminalClient;
            _formOptions = formOptions;
        }

        public async Task HandleAsync(ContentPublishingNotification notification, CancellationToken cancellationToken)
        {
            foreach (var item in notification.PublishedEntities)
            {
                if (item.ContentType.Alias.Equals("product") && item.GetValue<string>("Status") == "New")
                {

                    var formId = _formOptions.Value.AddProductFormId;
                    var record = _recordReaderService.GetRecordsFromForm(formId, 1, int.MaxValue)
                        .Items.FirstOrDefault(x => x.RecordFields.Where(y => y.Value.Alias == "Name"
                        && y.Value.Values.FirstOrDefault().ToString() == item.Name) != null);

                    if (record != null)
                    {
                        var currentMember = _memberService.GetByKey(Guid.Parse(record.MemberKey));
                        var uniqueCode = CodeGenerator.RandomCode(ReservationCodeLength);

                        await _sendGridService.SendEmailWithTemplateAsync(currentMember.Email,
                                _sendGridOptions.Value.Templates!.AddProductConfirmationTemplateId!,
                                new
                                {
                                    SenderName = currentMember.Name,
                                    ProductName = item.Name,
                                    UniqueCode = uniqueCode
                                });

                        var addProductRequest = new AddProductRequest
                        {
                            MemberId = currentMember.Key,
                            PhotoUrl = item.GetValue<string>("Photo"),
                            ProductId = item.Key,
                            UniqueCode = uniqueCode
                        };

                        await _terminalClient.TryAddProduct(addProductRequest);
                    }
                }
            }
        }
    }
}
