using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace BakWeb.Reservation.Entities
{
    [TableName("Reservations")]
    [PrimaryKey("Id")]
    public class ReservationEntity
    {
        [PrimaryKeyColumn]
        public int Id { get; set; }

        public string Name { get; set; }
        public Guid MemberId { get; set; }
        public Guid ProductId { get; set; }
        public DateTime ReservationDate { get; set; }
        public DateTime ReservationEndDate { get; set; }
        public int UniqueNumber { get; set; }
        public bool IsReservationClosed { get; set; }
    }
}
