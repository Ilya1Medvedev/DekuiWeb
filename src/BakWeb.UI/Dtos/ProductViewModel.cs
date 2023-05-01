using Umbraco.Cms.Core.Models.PublishedContent;

namespace BakWeb.Dtos
{
	public class ProductViewModel : PublishedContentWrapped
	{
		public ProductViewModel(IPublishedContent content, IPublishedValueFallback publishedValueFallback) : base(content, publishedValueFallback)
		{
			Products = new List<ProductDto>();
		}

		public List<ProductDto> Products { get; set; }
    }
}
