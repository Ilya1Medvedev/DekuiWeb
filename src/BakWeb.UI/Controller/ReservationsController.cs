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
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.Filters;

namespace BakWeb.Controller
{
    public class ReservationsController : UmbracoApiController
    {
        private readonly IMemberService _memberService;
        private readonly IUmbracoHelperAccessor _umbracoHelperAccessor;
        private readonly SendGridService _sendGridService;
        private readonly IOptions<SendGridOptions> _sendGridOptions;
        private readonly TerminalClient _terminalClient;
        private KonstruktRepository<ReservationEntity, int> _reservationsRepository;

        private readonly int ReservationLimit = 3;
        private readonly int ReservationCodeLength = 6;

        public ReservationsController(IKonstruktRepositoryFactory repositoryFactory, IMemberService memberService,
            IUmbracoHelperAccessor umbracoHelperAccessor, SendGridService sendGridService, IOptions<SendGridOptions> sendGridOptions,
            TerminalClient terminalClient)
        {
            _reservationsRepository = repositoryFactory.GetRepository<ReservationEntity, int>();
            _memberService = memberService;
            _umbracoHelperAccessor = umbracoHelperAccessor;
            _sendGridService = sendGridService;
            _sendGridOptions = sendGridOptions;
            _terminalClient = terminalClient;
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

            var currentUser = User.Identity;
            var member = _memberService.GetById(currentUser!.GetUserId<int>());

            // Check if member has reservations. If more then 3, 403 - Forbidden (Too many reservations)
            var memberActiveReservations = _reservationsRepository.GetAll(x => x.MemberId == member!.Key && x.ReservationEndDate > DateTime.Now);
            if (memberActiveReservations.Success && memberActiveReservations.Model.Count() >= ReservationLimit)
            {
                return Forbid("Too many reservations");
            }

            var product = umbracoHelper!.Content(reservationRequest.ProductId);
            if (product == null)
            {
                return NotFound("Product does not exists");
            }

            var productActiveReservation = _reservationsRepository.GetAll(x => x.ProductId == reservationRequest.ProductId
                && x.ReservationEndDate > DateTime.Now).Model.FirstOrDefault();

            // Check if current product not reserved. If reserved 403 - Forbidden (Already reserved)
            if (productActiveReservation != null)
            {
                return Forbid("Product is already reserved");
            }

            var reservation = new ReservationEntity
            {
                MemberId = member!.Key,
                ProductId = reservationRequest.ProductId,
                Name = $"{product.Name} for '{member.Email}'",
                ReservationDate = DateTime.Now,
                ReservationEndDate = DateTime.Now.AddDays(1),
                UniqueNumber = GenerateUniqueCode()
            };

            var reservationResult = _reservationsRepository.Insert(reservation);
            if (!reservationResult.Success)
            {
                return BadRequest("Failed to create a reservation");
            }

            await _sendGridService.SendEmailWithTemplateAsync(member.Email,
                _sendGridOptions.Value.Templates!.ReservationConfirmationTemplateId!,
                new
                {
                    SenderName = member.Name,
                    ProductName = product.Name,
                    UniqueCode = reservation.UniqueNumber,
                    ReservationEndDate = reservation.ReservationEndDate.ToString("U")
                });

            var terminalReservationRequest = new AddReseravationRequest
            {
                MemberId = member.Key,
                ProductId = reservation.ProductId,
                PhotoUrl = product.Value<string>("Photo"),
                UniqueCode = reservation.UniqueNumber
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
