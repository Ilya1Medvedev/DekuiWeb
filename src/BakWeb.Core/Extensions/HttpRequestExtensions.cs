using BakWeb.Core.Models;
using Microsoft.AspNetCore.Http;

namespace BakWeb.Core.Extensions;

public static class HttpRequestExtensions
{
    public static PaginationMetadata ExtractPaginationMetadata(this HttpRequest request)
    {
        var paginationMetadata = new PaginationMetadata();

        var pageRequestQuery = request.Query["page"];

        if (int.TryParse(pageRequestQuery, out var pageNumber))
        {
            paginationMetadata.Page = pageNumber;
        }

        var itemsPerPageQuery = request.Query["itemsPerPage"];
        if (int.TryParse(itemsPerPageQuery, out var itemsPerPageNumber))
        {
            paginationMetadata.ItemsPerPage = itemsPerPageNumber;
        }

        return paginationMetadata;
    }
}
