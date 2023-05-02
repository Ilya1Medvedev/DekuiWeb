using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.Security;

namespace BakWeb.Controller
{
    public class LogoutController : RenderController
    {
        private readonly IMemberSignInManager _memberSignInManager;

        public LogoutController(ILogger<RenderController> logger, ICompositeViewEngine compositeViewEngine,
            IUmbracoContextAccessor umbracoContextAccessor, IMemberSignInManager memberSignInManager) : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            _memberSignInManager = memberSignInManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var returnUri = "/";

            if (!User.Identity.IsAuthenticated)
                return Redirect(returnUri);

            await _memberSignInManager.SignOutAsync();

            return new RedirectResult(returnUri);
        }
    }
}
