using API.DataAccess;

namespace Api.Controllers;

public class TagOptionsFilter : PaginationFilter
{
    public TagOptionsFilter()
    {
    }

    public TagOptionsFilter(int pageNumber, int pageSize, int userId, string searchText)
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