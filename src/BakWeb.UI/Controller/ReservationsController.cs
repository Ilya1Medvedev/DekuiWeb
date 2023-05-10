using BakWeb.Core.Models;
using BakWeb.Dtos;
using BakWeb.Extensions;
using BakWeb.Options;
using BakWeb.Reservation.Entities;
using BakWeb.Services;
using BakWeb.TerminalControllerClient;
using BakWeb.TerminalControllerClient.Models;
using Konstrukt.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.Filters;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace BakWeb.Controller
{
    public class ReservationsController : UmbracoApiController
    {
        private readonly IMemberService _memberService;
        private readonly IUmbracoHelperAccessor _umbracoHelperAccessor;
        private readonly SendGridService _sendGridService;
        private readonly IOptions<SendGridOptions> _sendGridOptions;
        private readonly TerminalClient _terminalClient;
        private readonly IContentService _contentService;
        private KonstruktRepository<ReservationEntity, int> _reservationsRepository;

        private readonly int ReservationLimit = 3;
        private readonly int ReservationCodeLength = 6;

        public ReservationsController(IKonstruktRepositoryFactory repositoryFactory, IMemberService memberService,
            IUmbracoHelperAccessor umbracoHelperAccessor, SendGridService sendGridService, IOptions<SendGridOptions> sendGridOptions,
            TerminalClient terminalClient, IContentService contentService)
        {
            _reservationsRepository = repositoryFactory.GetRepository<ReservationEntity, int>();
            _memberService = memberService;
            _umbracoHelperAccessor = umbracoHelperAccessor;
            _sendGridService = sendGridService;
            _sendGridOptions = sendGridOptions;
            _terminalClient = terminalClient;
            _contentService = contentService;
        }

        [UmbracoMemberAuthorize]
        [HttpPost]
        public async Task<IActionResult> MakeReservation([FromBody] ReservationRequestDto reservationRequest)
        {

            var umbracoHelperAttempt = _umbracoHelperAccessor.TryGetUmbracoHelper(out var umbracoHelper);
            if (umbracoHelperAttempt == false)
            {
                return NotFound("Internal error");
            }

            var product = umbracoHelper!.Content(reservationRequest.ProductId) as Product;
            if (product == null)
            {
                return NotFound("Product does not exists");
            }

            var currentUser = User.Identity;
            var member = _memberService.GetById(currentUser!.GetUserId<int>());

            var memberActiveReservations = product
                .Siblings<Product>()
                .Where(x => x.Reservations != null)
                .SelectMany(x => x.Reservations.Cast<Umbraco.Cms.Web.Common.PublishedModels.Reservation>()
                    .Where(y => y.Member?.Id == member?.Id && y.EndDate > DateTime.Now));

            // Check if member has reservations. If more then 3, 403 - Forbidden (Too many reservations)
            if (memberActiveReservations.Count() >= ReservationLimit)
            {
                return Forbid("Too many reservations");
            }

            var isProductReserved = product.Reservations?.Cast<Umbraco.Cms.Web.Common.PublishedModels.Reservation>()
                .Any(x => x.EndDate > DateTime.Now) ?? false;

            // Check if current product not reserved. If reserved 403 - Forbidden (Already reserved)
            if (isProductReserved)
            {
                return Forbid("Product is already reserved");
            }

            var uniqueNumberOut = CodeGenerator.RandomCode(ReservationCodeLength);

            var reservationStartDate = DateTime.Now;
            var reservationEndDate = DateTime.Now.AddDays(1);

            var reservationElementTypeModel = new ElementTypeModel<ReservationElementTypeModel>
            {
                Key = "cca5d485-5028-4342-a4fd-12ae6571d8e6",
                ElementType = "92b1a372-669e-4e3e-b767-847c10a2b136",
                Value = new ReservationElementTypeModel
                {
                    EndDate = reservationEndDate,
                    StartDate = reservationStartDate,
                    Member = member.GetUdi().ToString(),
                    UniqueCodeOut = uniqueNumberOut.ToString()
                }
            };

            var productContent = _contentService.GetById(product.Id);
            productContent!.SetValue("Reservations", JsonConvert.SerializeObject(reservationElementTypeModel.AsEnumerableOfOne()));

            _contentService.SaveAndPublish(productContent);

            await _sendGridService.SendEmailWithTemplateAsync(member.Email,
                _sendGridOptions.Value.Templates!.ReservationConfirmationTemplateId!,
                new
                {
                    SenderName = member.Name,
                    ProductName = product.Name,
                    UniqueCode = uniqueNumberOut,
                    ReservationEndDate = reservationEndDate.ToString("U")
                });

            var terminalReservationRequest = new AddReseravationRequest
            {
                MemberId = member.Key,
                ProductId = product.Key,
                PhotoUrl = product.Value<string>("Photo"),
                UniqueCode = uniqueNumberOut
            };

            await _terminalClient.TryAddReservation(terminalReservationRequest);

            return Ok();
        }

        private int GenerateUniqueCode()
        {
            var generateUniqueNumber = CodeGenerator.RandomCode(ReservationCodeLength);
            var isCodeUnique = false;
            while (isCodeUnique == false)
            {
                var productWithCode = _reservationsRepository.GetAll(x => x.UniqueNumber == generateUniqueNumber)
                    .Model.FirstOrDefault();

                if (productWithCode == null)
                {
                    isCodeUnique = true;
                }
            }

            return generateUniqueNumber;
        }
    }
}
