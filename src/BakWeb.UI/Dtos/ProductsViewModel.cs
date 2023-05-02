using BakWeb.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using X.PagedList;

namespace BakWeb.Dtos
{
    public class ProductsViewModel : PublishedContentWrapped
    {
        public ProductsViewModel(IPublishedContent content, IPublishedValueFallback publishedValueFallback) : base(content, publishedValueFallback)
        {
            Products = new PagedList<ProductViewModel>(Enumerable.Empty<ProductViewModel>(), 1, 1);
        }

        public IPagedList<ProductViewModel> Products { get; set; }
    }
}
