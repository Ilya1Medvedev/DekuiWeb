using BakWeb.Dtos;
using BakWeb.Extensions;
using BakWeb.Reservation.Entities;
using Konstrukt.Persistence;
using Microsoft.AspNetCore.Mvc;
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
        private KonstruktRepository<ReservationEntity, int> _reservationsRepository;

        private readonly int ReservationLimit = 3;
        private readonly int ReservationCodeLength = 6;

        public ReservationsController(IKonstruktRepositoryFactory repositoryFactory, IMemberService memberService,
            IUmbracoHelperAccessor umbracoHelperAccessor)
        {
            _reservationsRepository = repositoryFactory.GetRepository<ReservationEntity, int>();
            _memberService = memberService;
            _umbracoHelperAccessor = umbracoHelperAccessor;
        }

        [UmbracoMemberAuthorize]
        [HttpPost]
        public IActionResult MakeReservation([FromBody] ReservationRequestDto reservationRequest)
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

            _reservationsRepository.Insert(reservation);
            // TODO : Send unique code to member EMAIL
            // TODO : Send details to LOCKER API

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
