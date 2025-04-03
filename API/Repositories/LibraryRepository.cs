using API.DataAccess;
using API.DTOs;

namespace API.Repositories;

public interface ILibraryRepository : IRepositoryBase
{
    int AddLibrary(LibraryDto library);
    LibraryDto GetLibraryById(int libraryId);
    PagedResult<LibraryDto> GetAllLibraries(LibraryFilter? filter = null);
    void UpdateLibrary(LibraryDto library);
    void DeleteLibrary(int libraryId);
}

public class LibraryRepository(ISqlDbAccess sqlDbAccess) : ILibraryRepository
{
    private readonly string _databaseName = "blank";

    public int AddLibrary(LibraryDto library)
    {
        var parameters = new Dictionary<string, object>
        {
            { "@libraryName", library.Name }
        };

        var insertSql = @"
            INSERT INTO libraries (library_name)
            VALUES (@libraryName);
            SELECT CAST(SCOPE_IDENTITY() as int);";

        var libraryId = sqlDbAccess.ExecuteQuery<int>(
            _databaseName,
            insertSql,
            "",
            "",
            parameters).FirstOrDefault();

        return libraryId;
    }

    public LibraryDto GetLibraryById(int libraryId)
    {
        var parameters = new Dictionary<string, object>
        {
            { "@libraryId", libraryId }
        };

        var selectSql = @"
            SELECT
                library_id as Id,
                library_name as Name
            FROM libraries
            WHERE library_id = @libraryId";

        var result = sqlDbAccess.ExecuteQuery<LibraryDto>(
            _databaseName,
            selectSql,
            "",
            "",
            parameters).FirstOrDefault();

        if (result == null) throw new InvalidOperationException($"Library with ID {libraryId} not found");

        return new LibraryDto(result.Id, result.Name);
    }

    public PagedResult<LibraryDto> GetAllLibraries(LibraryFilter? filter = null)
    {
        var selectSql = @"
            SELECT
                library_id as Id,
                library_name as Name ";

        var countSql = "SELECT COUNT(*)";

        var fromWhereSql = @"FROM libraries WHERE 1=1";

        if (filter != null)
        {
            if (filter.LibraryIds != null && filter.LibraryIds.Any())
            {
                var ids = string.Join(",", filter.LibraryIds);
                fromWhereSql += $" AND library_id IN ({ids})";
            }

            if (filter.LibraryNames != null && filter.LibraryNames.Any())
            {
                var names = string.Join("','", filter.LibraryNames);
                fromWhereSql += $" AND library_name IN ('{names}')";
            }
        }

        var orderBy = " ORDER BY library_id";

        if (filter is { PageSize: not null, PageNumber: not null })
        {
            var totalCount = sqlDbAccess.ExecuteQuery<int>(
                _databaseName,
                countSql,
                fromWhereSql,
                "",
                new Dictionary<string, object>()).FirstOrDefault();

            var pagedLibraries = sqlDbAccess.GetPagedResult<LibraryDto>(
                _databaseName,
                selectSql,
                fromWhereSql,
                orderBy,
                new Dictionary<string, object>(),
                filter.PageNumber.Value,
                filter.PageSize.Value).ToList();

            return new PagedResult<LibraryDto>
            {
                Items = pagedLibraries,
                TotalCount = totalCount,
                PageSize = filter.PageSize.Value,
                CurrentPage = filter.PageNumber.Value,
                TotalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize.Value)
            };
        }

        var allLibraries = sqlDbAccess.ExecuteQuery<LibraryDto>(
            _databaseName,
            selectSql,
            fromWhereSql,
            orderBy,
            new Dictionary<string, object>()).ToList();

        return new PagedResult<LibraryDto>
        {
            Items = allLibraries,
            TotalCount = allLibraries.Count,
            PageSize = allLibraries.Count,
            CurrentPage = 1,
            TotalPages = 1
        };
    }

    public void UpdateLibrary(LibraryDto library)
    {
        var parameters = new Dictionary<string, object>
        {
            { "@libraryId", library.Id },
            { "@libraryName", library.Name }
        };

        var updateSql = @"
            UPDATE libraries
            SET library_name = @libraryName
            WHERE library_id = @libraryId";

        sqlDbAccess.ExecuteNonQuery(
            _databaseName,
            updateSql,
            parameters);
    }

    public void DeleteLibrary(int libraryId)
    {
        var parameters = new Dictionary<string, object>
        {
            { "@libraryId", libraryId }
        };

        // Delete user access to the library
        var deleteUserAccessSql = @"DELETE FROM user_library_access WHERE library_id = @libraryId";
        sqlDbAccess.ExecuteNonQuery(_databaseName, deleteUserAccessSql, parameters);

        // Delete library-upload relationships
        var deleteLibraryUploadsSql = @"DELETE FROM library_uploads WHERE library_id = @libraryId";
        sqlDbAccess.ExecuteNonQuery(_databaseName, deleteLibraryUploadsSql, parameters);

        // Delete the library
        var deleteLibrarySql = @"DELETE FROM libraries WHERE library_id = @libraryId";
        sqlDbAccess.ExecuteNonQuery(_databaseName, deleteLibrarySql, parameters);
    }
}

public class LibraryFilter : PaginationFilter
{
    public LibraryFilter(List<int>? libraryIds,
        List<string>? libraryNames,
        int? pageNumber,
        int? pageSize) : base(pageNumber, pageSize)
    {
        LibraryIds = libraryIds;
        LibraryNames = libraryNames;
        PageSize = pageSize;
        PageNumber = pageNumber;
    }

    public LibraryFilter()
    {
    }

    public List<int>? LibraryIds { get; set; }
    public List<string>? LibraryNames { get; set; }
    public int? PageSize { get; set; }
    public int? PageNumber { get; set; }
}