using BakWeb.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace BakWeb.Controller
{
    public class ProductController : RenderController
    {
        private readonly IVariationContextAccessor _variationContextAccessor;
        private readonly ServiceContext _context;

        public ProductController(ILogger<RenderController> logger, ICompositeViewEngine compositeViewEngine, 
            IUmbracoContextAccessor umbracoContextAccessor, IVariationContextAccessor variationContextAccessor, ServiceContext context) : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            _variationContextAccessor = variationContextAccessor;
            _context = context;
        }

        public override IActionResult Index()
        {
            var currentPage = CurrentPage as Product;

            if (currentPage == null)
            {
                return NotFound();
            }

            var productViewModel = new ProductViewModel(currentPage, new PublishedValueFallback(_context, _variationContextAccessor))
            {
                DisplayName = currentPage.Name,
                Image = currentPage.Photo?.Content,
                Description = currentPage.Description,
                Url = currentPage.Url()
            };

            return CurrentTemplate(productViewModel);
        }
    }
}
