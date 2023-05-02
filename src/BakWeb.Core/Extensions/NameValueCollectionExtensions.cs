using System.Collections.Specialized;
using System.Web;

namespace BakWeb.Core.Extensions;

public static class NameValueCollectionExtensions
{
    public static NameValueCollection AddParam(this NameValueCollection collection, string key, string value)
    {
        if (string.IsNullOrEmpty(value)) return collection;

        var queryString = HttpUtility.ParseQueryString(collection.ToString());
        queryString.Add(key, value);
        return queryString;
    }

}
