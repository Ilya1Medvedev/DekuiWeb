using Umbraco.Cms.Core.Models.PublishedContent;

namespace BakWeb.Dtos
{
    public class ProductDto 
	{
		public string? Name { get; set; }
        public IPublishedContent? Image { get; set; }
        public string? Description { get; set; }
        public string? Url { get; set; }
    }
}
