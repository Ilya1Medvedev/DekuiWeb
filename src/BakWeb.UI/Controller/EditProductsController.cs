using BakWeb.Core.Extensions;
using BakWeb.Dtos;
using BakWeb.Reservation.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.Filters;
using Umbraco.Cms.Web.Common.PublishedModels;
using X.PagedList;

namespace BakWeb.Controller;

public class EditProductsController : RenderController
{
    private readonly IVariationContextAccessor _variationContextAccessor;
    private readonly ServiceContext _context;

    public EditProductsController(ILogger<RenderController> logger, ICompositeViewEngine compositeViewEngine,
        IUmbracoContextAccessor umbracoContextAccessor, IVariationContextAccessor variationContextAccessor, ServiceContext context) : base(logger, compositeViewEngine, umbracoContextAccessor)
    {
        _variationContextAccessor = variationContextAccessor;
        _context = context;
    }

    public override IActionResult Index()
    {
        var products = CurrentPage.Siblings<Products>()
            .FirstOrDefault()
            .Children<Product>()?.Where(x => string.Equals(x.Status, "New", StringComparison.InvariantCultureIgnoreCase));

        var result = new ProductsViewModel(CurrentPage, new PublishedValueFallback(_context, _variationContextAccessor));

        if (products != null)
        {
            var paginationMetadata = Request.ExtractPaginationMetadata();
            result.Products = products.Select(product => new ProductViewModel(product, new PublishedValueFallback(_context, _variationContextAccessor))
            {
                DisplayName = product.Name,
                Image = product.Photo,
                Description = product.Description,
                Url = product.Url(),
                IsReserved = false,
            }).ToPagedList(paginationMetadata.Page, paginationMetadata.ItemsPerPage);
        }

        return CurrentTemplate(result);
    }
}

public class EditProductsApiController : UmbracoApiController
{
    [UmbracoMemberAuthorize]
    [HttpGet]
    public IActionResult Delete([FromQuery] Guid productId)
    {
        return Ok();
    }
}
