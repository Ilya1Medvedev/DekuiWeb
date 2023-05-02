using BakWeb.Dtos;
using BakWeb.Core.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco.Forms.Core.Services;
using X.PagedList;

namespace BakWeb.Controller
{
    public class ProductsController : RenderController
    {
        private readonly IRecordReaderService _recordReaderService;
        private readonly IVariationContextAccessor _variationContextAccessor;
        private readonly ServiceContext _context;

        public ProductsController(ILogger<RenderController> logger, ICompositeViewEngine compositeViewEngine,
            IUmbracoContextAccessor umbracoContextAccessor, IRecordReaderService recordReaderService,
            IVariationContextAccessor variationContextAccessor, ServiceContext context) : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            _recordReaderService = recordReaderService;
            _variationContextAccessor = variationContextAccessor;
            _context = context;
        }
        public override IActionResult Index()
        {
            var currentPage = CurrentPage as Products;

            if (currentPage == null)
            {
                return NotFound();
            }

            var result = new ProductViewModel(currentPage, new PublishedValueFallback(_context, _variationContextAccessor));
            var products = currentPage.Children<Product>();

            var paginationMetadata = Request.ExtractPaginationMetadata();

            result.Products = products.Select(x => new ProductDto
            {
                Name = x.Name,
                Image = x.Photo?.Content,
                Description = x.Description,
                Url = x.Url()
            }).ToPagedList(paginationMetadata.Page, paginationMetadata.ItemsPerPage);

            return CurrentTemplate(result);
        }
    }
}
