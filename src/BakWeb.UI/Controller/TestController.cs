using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common.Controllers;

namespace BakWeb.Controller
{
    public class TestController : UmbracoApiController
    {
        private readonly IContentService _contentService;

        public TestController(IContentService contentService)
        {
            _contentService = contentService;
        }

        [HttpGet]
        public IActionResult Content([FromQuery] int id)
        {
            var test = _contentService.GetById(id);
            return Ok();
        }
    }
}
