﻿namespace BakWeb.Core.Models;
public class PaginationMetadata
{
    public PaginationMetadata()
    {
        Page = 1;
        ItemsPerPage = 5;
    }

    public int ItemsPerPage { get; set; }
    public int Page { get; set; }
    public int TotalCount { get; set; }
}
