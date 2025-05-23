using API.DataAccess;
using API.DTOs;
using Npgsql;

namespace API.Repositories;

public interface ITagRepository : IRepositoryBase
{
    IEnumerable<TagDto> GetAllTags(TagFilter? filter);
    bool AddTag(TagDto tag);
    void DeleteTags(TagFilter? filter);
}

public class TagRepository(ISqlDbAccess sqlDbAccess) : ITagRepository
{
    private readonly string _databaseName = "blank";

    public IEnumerable<TagDto> GetAllTags(TagFilter? filter = null)
    {
        var selectSql = @"
            SELECT
                t.tag_id AS TagId,
                t.name AS TagName";

        var countSql = "SELECT COUNT(*)";

        var fromWhereSql = @"
            FROM tags t
            WHERE 1=1";

        var parameters = new Dictionary<string, object>();

        if (filter != null)
        {
            if (filter.Ids != null && filter.Ids.Any())
            {
                parameters.Add("@ids", filter.Ids);
                fromWhereSql += " AND t.tag_id = ANY(@ids)";
            }

            if (filter.Names != null && filter.Names.Any())
            {
                parameters.Add("@names", filter.Names);
                fromWhereSql += " AND t.name = ANY(@names)";
            }
        }

        var orderBy = " ORDER BY t.tag_id";

        if (filter is { PageSize: not null, PageNumber: not null })
        {
            var totalCount = sqlDbAccess.ExecuteQuery<int>(
                _databaseName,
                countSql,
                fromWhereSql,
                "",
                parameters).FirstOrDefault();

            var pagedTags = sqlDbAccess.GetPagedResult<TagDto>(
                _databaseName,
                selectSql,
                fromWhereSql,
                orderBy,
                parameters,
                filter.PageNumber.Value,
                filter.PageSize.Value).ToList();

            return new PagedResult<TagDto>
            {
                Items = pagedTags,
                TotalCount = totalCount,
                PageSize = filter.PageSize.Value,
                CurrentPage = filter.PageNumber.Value,
                TotalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize.Value)
            };
        }

        var allTags = sqlDbAccess.ExecuteQuery<TagDto>(
            _databaseName,
            selectSql,
            fromWhereSql,
            orderBy,
            parameters).ToList();

        return new PagedResult<TagDto>
        {
            Items = allTags,
            TotalCount = allTags.Count,
            PageSize = allTags.Count,
            CurrentPage = 1,
            TotalPages = 1
        };
    }

    public bool AddTag(TagDto tag)
    {
        if (string.IsNullOrWhiteSpace(tag.TagName))
            throw new ArgumentException("Tag name cannot be null or empty.", nameof(tag));

        var insertSql = @"
        INSERT INTO tags (name)
        VALUES (@name);";

        var parameters = new Dictionary<string, object>
        {
            { "@name", tag.TagName }
        };

        try
        {
            sqlDbAccess.ExecuteNonQuery(_databaseName, insertSql, parameters);
            return true;
        }
        catch (PostgresException ex) when (ex.SqlState == "23505") // unique_violation
        {
            return false;
        }
    }

    public void DeleteTags(TagFilter? filter)
    {
        if (filter == null ||
            ((filter.Ids == null || !filter.Ids.Any()) &&
             (filter.Names == null || !filter.Names.Any())))
            throw new ArgumentException("At least one filter (Ids or Names) must be provided to delete tags.");

        var deleteSql = "DELETE FROM tags WHERE 1=1";
        var parameters = new Dictionary<string, object>();

        if (filter.Ids != null && filter.Ids.Any())
        {
            deleteSql += " AND tag_id = ANY(@ids)";
            parameters.Add("@ids", filter.Ids);
        }

        if (filter.Names != null && filter.Names.Any())
        {
            deleteSql += " AND name = ANY(@names)";
            parameters.Add("@names", filter.Names);
        }

        sqlDbAccess.ExecuteNonQuery(_databaseName, deleteSql, parameters);
    }
}

public class TagFilter : PaginationFilter
{
    public TagFilter()
    {
        Ids = null;
        Names = null;
    }

    public TagFilter(int pageNumber, int pageSize) : base(pageNumber, pageSize)
    {
        Ids = null;
        Names = null;
    }

    public int[]? Ids { get; set; }
    public string[]? Names { get; set; }

    public int? PageSize { get; set; }
    public int? PageNumber { get; set; }
}