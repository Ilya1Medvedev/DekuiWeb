using BakWeb.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using X.PagedList;

namespace BakWeb.Dtos
{
    public class ProductViewModel : PublishedContentWrapped
    {
        public ProductViewModel(IPublishedContent content, IPublishedValueFallback publishedValueFallback) : base(content, publishedValueFallback)
        {
            Products = new PagedList<ProductDto>(Enumerable.Empty<ProductDto>(), 1, 1);
        }

        public IPagedList<ProductDto> Products { get; set; }
    }
}
