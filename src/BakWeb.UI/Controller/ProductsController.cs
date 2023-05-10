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
using Umbraco.Cms.Core.Security;
using Konstrukt.Persistence;
using BakWeb.Reservation.Entities;
using BakWeb.Reservation.Repositories;

namespace BakWeb.Controller
{
    public class ProductsController : RenderController
    {
        private readonly IVariationContextAccessor _variationContextAccessor;
        private readonly ServiceContext _context;

        public ProductsController(ILogger<RenderController> logger, ICompositeViewEngine compositeViewEngine,
            IUmbracoContextAccessor umbracoContextAccessor, IVariationContextAccessor variationContextAccessor, ServiceContext context) : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
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

            var result = new ProductsViewModel(currentPage, new PublishedValueFallback(_context, _variationContextAccessor));
            var products = currentPage.Children<Product>()?
                .Where(x => !string.Equals(x.Status, "New", StringComparison.InvariantCultureIgnoreCase));

            if (products != null)
            {
                var paginationMetadata = Request.ExtractPaginationMetadata();
                result.Products = products.Select(product => new ProductViewModel(product, new PublishedValueFallback(_context, _variationContextAccessor))
                {
                    DisplayName = product.Name,
                    Image = product.Photo,
                    Description = product.Description,
                    Url = product.Url(),
                    IsReserved = product.Reservations?.Cast<Umbraco.Cms.Web.Common.PublishedModels.Reservation>()
                    .Any(x => x.EndDate > DateTime.Now) ?? false
                }).ToPagedList(paginationMetadata.Page, paginationMetadata.ItemsPerPage);
            }

            return CurrentTemplate(result);
        }
    }
}
