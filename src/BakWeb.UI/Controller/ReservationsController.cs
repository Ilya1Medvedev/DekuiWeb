using BakWeb.Reservation.Entities;
using Konstrukt.Persistence;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Controllers;

namespace BakWeb.Controller
{
    public class ReservationsController : UmbracoApiController
    {
        private KonstruktRepository<ReservationEntity, int> _reservationsRepository;

        public ReservationsController(IKonstruktRepositoryFactory repositoryFactory)
        {
            _reservationsRepository = repositoryFactory.GetRepository<ReservationEntity, int>();
        }

        public IActionResult MakeReservation()
        {
            // Check if member is logged in, if not, then 401 - Unauthorized 
            // Check if member has reservations. If more then 3, 403 - Forbidden (Too many reservations)
            // Check if current product not reserved. If reserved 403 - Forbidden (Already reserved)
            // Reserve a product for this member (Create reservation with unique code)
            // Send unique code to member EMAIL

            return Ok();
        }
    }
}
