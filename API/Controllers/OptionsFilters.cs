using API.DataAccess;

namespace Api.Controllers;

public class BaseOptionsFilter : PaginationFilter
{
    public BaseOptionsFilter()
    {
    }

    public BaseOptionsFilter(int pageNumber, int pageSize, int userId, string searchText)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        UserId = userId;
        SearchText = searchText;
    }

    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
    public int? UserId { get; set; }
    public string? SearchText { get; set; }
}