using Umbraco.Cms.Core.Models.PublishedContent;

namespace BakWeb.Dtos
{
    public class ProductViewModel : PublishedContentWrapped
    {
        public ProductViewModel(IPublishedContent content, IPublishedValueFallback publishedValueFallback)
            : base(content, publishedValueFallback)
        {
            IsReserved = false;
        }

        public string? DisplayName { get; set; }
        public IPublishedContent? Image { get; set; }
        public string? Description { get; set; }
        public string? Url { get; set; }

        public bool IsReserved { get; set; }
    }
}
