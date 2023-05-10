using BakWeb.Extensions;
using BakWeb.Options;
using BakWeb.Services;
using BakWeb.TerminalControllerClient;
using BakWeb.TerminalControllerClient.Models;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Core.Services.Notifications;

namespace BakWeb.Notifications
{
    public class TestSiteComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.AddNotificationAsyncHandler<RecordCreatingNotification, FormSavedNotificationHandler>();

        }
    }

    public class FormSavedNotificationHandler : INotificationAsyncHandler<RecordCreatingNotification>
    {
        private readonly int ReservationCodeLength = 6;

        private readonly SendGridService _sendGridService;
        private readonly IMemberService _memberService;
        private readonly IRecordReaderService _recordReaderService;
        private readonly IOptions<SendGridOptions> _sendGridOptions;
        private readonly TerminalClient _terminalClient;
        private readonly IOptions<FormOptions> _formOptions;
        private readonly IContentService _contentService;

        public FormSavedNotificationHandler(SendGridService sendGridService, IMemberService memberService,
            IRecordReaderService recordReaderService, IOptions<SendGridOptions> sendGridOptions, TerminalClient terminalClient,
            IOptions<FormOptions> formOptions, IContentService contentService)
        {
            _sendGridService = sendGridService;
            _memberService = memberService;
            _recordReaderService = recordReaderService;
            _sendGridOptions = sendGridOptions;
            _terminalClient = terminalClient;
            _formOptions = formOptions;
            _contentService = contentService;
        }
        public async Task HandleAsync(RecordCreatingNotification notification, CancellationToken cancellationToken)
        {
            foreach (var record in notification.SavedEntities)
            {
                var member = _memberService.GetByKey(Guid.Parse(record.MemberKey));
                var recordFields = new
                {
                    Name = record.GetFieldValueAsString("name"),
                    Status = "New",
                    Photo = record.GetFieldValueAsString("photo"),
                    Description = record.GetFieldValueAsString("description"),
                    Size = record.GetFieldValueAsString("size"),
                    Quality = record.GetFieldValueAsString("quality"),
                    Owner = member.GetUdi().ToString()
                };

                var uniqueCodeIn = CodeGenerator.RandomCode(ReservationCodeLength);
                var uniqueCodeOut = CodeGenerator.RandomCode(ReservationCodeLength);

                var newProduct = _contentService.Create(recordFields.Name, 1065, "Product");

                newProduct.SetValue("Status", recordFields.Status);
                newProduct.SetValue("Photo", recordFields.Photo.Replace("\\", string.Empty));
                newProduct.SetValue("Description", recordFields.Description);
                newProduct.SetValue("Size", $"[\"{recordFields.Size}\"]");
                newProduct.SetValue("Quality", $"[\"{recordFields.Quality}\"]");
                newProduct.SetValue("Owner", recordFields.Owner);
                newProduct.SetValue("UniqueCodeIn", uniqueCodeIn);
                newProduct.SetValue("UniqueCodeOut", uniqueCodeOut);

                _contentService.SaveAndPublish(newProduct);

                await _sendGridService.SendEmailWithTemplateAsync(member.Email,
                        _sendGridOptions.Value.Templates!.AddProductConfirmationTemplateId!,
                        new
                        {
                            SenderName = member.Name,
                            ProductName = newProduct.Name,
                            UniqueCode = uniqueCodeIn
                        });

                var addProductRequest = new AddProductRequest
                {
                    MemberId = member.Key,
                    PhotoUrl = recordFields.Photo.Replace("\\", string.Empty),
                    ProductId = newProduct.Key,
                    UniqueCode = uniqueCodeIn
                };

                await _terminalClient.TryAddProduct(addProductRequest);
            }
        }
    }
}
