using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;
using Umbraco.Forms.Core.Persistence.Repositories;
using Umbraco.Forms.Core.Services;

namespace BakWeb.Controller
{
    public class RecordDataController : SurfaceController
    {
        private readonly IRecordReaderService _recordReaderService;
        private readonly IFormRepository _formRepository;

        public RecordDataController(IUmbracoContextAccessor umbracoContextAccessor,
            IUmbracoDatabaseFactory databaseFactory, ServiceContext services, AppCaches appCaches,
            IProfilingLogger profilingLogger, IPublishedUrlProvider publishedUrlProvider, IRecordReaderService recordReaderService, IFormRepository formRepository)
            : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            _recordReaderService = recordReaderService;
            _formRepository = formRepository;
        }

        public IActionResult GetProductData()
        {
            var formId = new Guid("ff472c1e-b986-4fc1-b585-eff84cd7c1b6");
            //_formRepository.GetMany()
            var records = _recordReaderService.GetApprovedRecordsFromForm(formId, 1, int.MaxValue);
            return Ok();
        }
    }
}
