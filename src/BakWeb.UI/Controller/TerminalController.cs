using BakWeb.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Core.Enums;
using BakWeb.TerminalControllerClient;
using BakWeb.TerminalControllerClient.Models;

namespace BakWeb.Controller
{
    public class TerminalController : UmbracoApiController
    {
        private readonly IRecordReaderService _recordReaderService;
        private readonly IOptions<FormOptions> _formOptions;
        private readonly TerminalClient _terminalClient;

        public TerminalController(IRecordReaderService recordReaderService, IOptions<FormOptions> formOptions, TerminalClient terminalClient)
        {
            _recordReaderService = recordReaderService;
            _formOptions = formOptions;
            _terminalClient = terminalClient;
        }

        [HttpGet]
        public async Task<IActionResult> VerifyProduct([FromQuery] Guid productId)
        {
            var formRecords = _recordReaderService.GetRecordsFromForm(_formOptions.Value.AddProductFormId, 1, int.MaxValue).Items.ToList();

            var productRequestForm = formRecords.Where(x => x.State == FormState.Submitted &&
                x.RecordFields.FirstOrDefault(y => y.Value.Alias == "ProductId").Value.ValuesAsString() == productId.ToString());


            throw new NotImplementedException();
        }

        [HttpGet]
        public async Task<IActionResult> DeleteProduct([FromQuery] Guid productId)
        {
            throw new NotImplementedException();
        }

        [HttpPost("/add-product")]
        public async Task<HttpResponseMessage> AddProduct([FromBody] AddProductRequest addProductRequest)
        {
            return await _terminalClient.AddProduct(addProductRequest);
        }

        [HttpPost("/add-reservation")]
        public async Task<HttpResponseMessage> AddProduct([FromBody] AddReseravationRequest addReseravationRequest)
        {
            return await _terminalClient.AddReservation(addReseravationRequest);
        }
    }
}
