using BakWeb.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Forms.Core.Services;

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
			var result = new ProductViewModel(CurrentPage, new PublishedValueFallback(_context, _variationContextAccessor));

            var formId = new Guid("ff472c1e-b986-4fc1-b585-eff84cd7c1b6");
            var records = _recordReaderService.GetApprovedRecordsFromForm(formId, 1, int.MaxValue);
            foreach (var record in records.Items)
            {
                result.Products.Add(new ProductDto
                {
                    Name = record.ValueAsString("name"),
                    // TODO : Fix 
                    Image = record.ValueAsString("photo").Replace(@"\", string.Empty)
                });
            }
            return CurrentTemplate(result);
        }
    }
}
