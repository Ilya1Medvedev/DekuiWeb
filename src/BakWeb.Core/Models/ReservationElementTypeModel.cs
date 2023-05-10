using Newtonsoft.Json;

namespace BakWeb.Core.Models
{
    public class ReservationElementTypeModel
    {
        [JsonProperty("member")]
        public string? Member { get; set; }
        [JsonProperty("uniqueCodeOut")]
        public string? UniqueCodeOut { get; set; }
        [JsonProperty("startDate")]
        public DateTime StartDate { get; set; }
        [JsonProperty("endDate")]
        public DateTime EndDate { get; set; }
    }
}
