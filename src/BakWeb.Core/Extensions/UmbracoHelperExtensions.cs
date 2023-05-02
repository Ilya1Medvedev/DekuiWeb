using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco.Extensions;

namespace BakWeb.Core.Extensions
{
    public static class UmbracoHelperExtensions
    {
        public static Home? Home(this UmbracoHelper umbracoHelper) => umbracoHelper.AssignedContentItem.Root().FirstChild<Home>();
    }
}
