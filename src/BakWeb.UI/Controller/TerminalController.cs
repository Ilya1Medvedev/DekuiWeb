using BakWeb.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Forms.Core.Services;
using BakWeb.TerminalControllerClient;
using Umbraco.Cms.Core.Services;

namespace BakWeb.Controller
{
    public class TerminalController : UmbracoApiController
    {
        private readonly IRecordReaderService _recordReaderService;
        private readonly IOptions<FormOptions> _formOptions;
        private readonly TerminalClient _terminalClient;
        private readonly IContentService _contentService;

        public TerminalController(IRecordReaderService recordReaderService, IOptions<FormOptions> formOptions,
            TerminalClient terminalClient, IContentService contentService)
        {
            _recordReaderService = recordReaderService;
            _formOptions = formOptions;
            _terminalClient = terminalClient;
            _contentService = contentService;
        }

        [HttpGet]
        public IActionResult VerifyProduct([FromQuery] Guid productId)
        {
            var product = _contentService.GetById(productId);

            if (product == null)
            {
                return NotFound("Product not found");
            }

            product.SetValue("Status", "Approved");

            _contentService.SaveAndPublish(product);
            return Ok();
        }

        [HttpGet]
        public IActionResult DeleteProduct([FromQuery] Guid productId)
        {
            var product = _contentService.GetById(productId);

            if (product == null)
            {
                return NotFound("Product not found");
            }

            _contentService.Delete(product);

            return Ok();
        }
    }
}
