using Newtonsoft.Json;

namespace BakWeb.Core.Models
{
    public class ElementTypeModel<T> where T : class
    {
        [JsonProperty("elementType")]
        public string ElementType { get; set; } = null!;
        [JsonProperty("key")]
        public string Key { get; set; } = null!;

        [JsonProperty("value")]
        public T? Value { get; set; }
    }
}
