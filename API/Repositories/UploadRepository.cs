using API.DataAccess;
using API.DTOs;

namespace API.Repositories;

public interface IUploadRepository : IRepositoryBase
{
    int AddUpload(UploadDto upload);
    UploadDto GetUploadById(int uploadId);
    PagedResult<UploadDto> GetAllUploads(UploadFilter? filter = null);
    void UpdateUpload(UploadDto upload);
    void DeleteUploadById(int uploadId);
}

public class UploadRepository(ISqlDbAccess sqlDbAccess) : IUploadRepository
{
    private readonly string _databaseName = "blank";

    public int AddUpload(UploadDto upload)
    {
        var parameters = new Dictionary<string, object>
        {
            { "@userId", upload.OwnerId },
            { "@uploadDate", upload.Date },
            { "@uploadType", upload.Type }
        };

        var insertSql = @"
            INSERT INTO uploads (user_id, upload_date, upload_type)
            VALUES (@userId, @uploadDate, @uploadType);
            SELECT CAST(SCOPE_IDENTITY() as int);";

        var uploadId = sqlDbAccess.ExecuteQuery<int>(
            _databaseName,
            insertSql,
            "",
            "",
            parameters).FirstOrDefault();

        // Association between upload and library
        var libraryParameters = new Dictionary<string, object>
        {
            { "@libraryId", upload.LibraryId },
            { "@uploadId", uploadId }
        };

        var libraryUploadSql = @"
            INSERT INTO library_uploads (library_id, upload_id)
            VALUES (@libraryId, @uploadId);";

        sqlDbAccess.ExecuteNonQuery(_databaseName, libraryUploadSql, libraryParameters);

        return uploadId;
    }

    public UploadDto GetUploadById(int uploadId)
    {
        var parameters = new Dictionary<string, object>
        {
            { "@uploadId", uploadId }
        };

        var selectSql = @"
            SELECT
                u.upload_id as Id,
                u.user_id as OwnerId,
                u.upload_date as Date,
                u.upload_type as Type,
                lu.library_id as LibraryId
            FROM uploads u
            JOIN library_uploads lu ON u.upload_id = lu.upload_id
            WHERE u.upload_id = @uploadId";

        var result = sqlDbAccess.ExecuteQuery<UploadDto>(
            _databaseName,
            selectSql,
            "",
            "",
            parameters).FirstOrDefault();

        if (result == null) throw new InvalidOperationException($"Upload with ID {uploadId} not found");

        return result;
    }

    public PagedResult<UploadDto> GetAllUploads(UploadFilter? filter = null)
    {
        var selectSql = @"
            SELECT
                u.upload_id as Id,
                u.user_id as OwnerId,
                u.upload_date as Date,
                u.upload_type as Type,
                lu.library_id as LibraryId ";

        var countSql = "SELECT COUNT(*)";

        var fromWhereSql = @"
            FROM uploads u
            JOIN library_uploads lu ON u.upload_id = lu.upload_id
            WHERE 1=1";

        var parameters = new Dictionary<string, object>();

        if (filter != null)
        {
            if (filter.UploadIds != null && filter.UploadIds.Any())
            {
                parameters.Add("@uploadIds", filter.UploadIds);
                fromWhereSql += $" AND u.upload_id = ANY(@uploadIds)";
            }

            if (filter.OwnerIds != null && filter.OwnerIds.Any())
            {
                parameters.Add("@ownerIds", filter.OwnerIds);
                fromWhereSql += $" AND u.user_id = ANY(@ownerIds)";
            }

            if (filter.LibraryIds != null && filter.LibraryIds.Any())
            {
                parameters.Add("@libraryIds", filter.LibraryIds);
                fromWhereSql += $" AND lu.library_id = ANY(@libraryIds)";
            }

            if (filter.UploadTypes != null && filter.UploadTypes.Any())
            {
                parameters.Add("@uploadTypes", filter.UploadTypes);
                fromWhereSql += $" AND u.upload_type = ANY(@uploadTypes)";
            }

            if (filter.DateFrom.HasValue)
            {
                parameters.Add("@dateFrom", filter.DateFrom.Value);
                fromWhereSql += " AND u.upload_date >= @dateFrom";
            }

            if (filter.DateTo.HasValue)
            {
                parameters.Add("@dateTo", filter.DateTo.Value);
                fromWhereSql += " AND u.upload_date <= @dateTo";
            }
        }

        var orderBy = " ORDER BY u.upload_id";

        if (filter is { PageSize: not null, PageNumber: not null })
        {
            var totalCount = sqlDbAccess.ExecuteQuery<int>(
                _databaseName,
                countSql,
                fromWhereSql,
                "",
                parameters).FirstOrDefault();

            var pagedUploads = sqlDbAccess.GetPagedResult<UploadDto>(
                _databaseName,
                selectSql,
                fromWhereSql,
                orderBy,
                parameters,
                filter.PageNumber.Value,
                filter.PageSize.Value).ToList();

            return new PagedResult<UploadDto>
            {
                Items = pagedUploads,
                TotalCount = totalCount,
                PageSize = filter.PageSize.Value,
                CurrentPage = filter.PageNumber.Value,
                TotalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize.Value)
            };
        }

        var allUploads = sqlDbAccess.ExecuteQuery<UploadDto>(
            _databaseName,
            selectSql,
            fromWhereSql,
            orderBy,
            parameters).ToList();

        return new PagedResult<UploadDto>
        {
            Items = allUploads,
            TotalCount = allUploads.Count,
            PageSize = allUploads.Count,
            CurrentPage = 1,
            TotalPages = 1
        };
    }

    public void UpdateUpload(UploadDto upload)
    {
        var parameters = new Dictionary<string, object>
        {
            { "@uploadId", upload.Id },
            { "@userId", upload.OwnerId },
            { "@uploadDate", upload.Date },
            { "@uploadType", upload.Type }
        };

        var updateSql = @"
            UPDATE uploads
            SET user_id = @userId,
                upload_date = @uploadDate,
                upload_type = @uploadType
            WHERE upload_id = @uploadId";

        sqlDbAccess.ExecuteNonQuery(_databaseName, updateSql, parameters);

        // Update library association
        var libraryParameters = new Dictionary<string, object>
        {
            { "@uploadId", upload.Id },
            { "@libraryId", upload.LibraryId }
        };

        var deleteLibraryAssocSql = @"DELETE FROM library_uploads WHERE upload_id = @uploadId";
        sqlDbAccess.ExecuteNonQuery(_databaseName, deleteLibraryAssocSql,
            new Dictionary<string, object> { { "@uploadId", upload.Id } });

        var libraryUploadSql = @"
            INSERT INTO library_uploads (library_id, upload_id)
            VALUES (@libraryId, @uploadId)";
        sqlDbAccess.ExecuteNonQuery(_databaseName, libraryUploadSql, libraryParameters);
    }

    public void DeleteUploadById(int uploadId)
    {
        var parameters = new Dictionary<string, object>
        {
            { "@uploadId", uploadId }
        };

        // Delete library association
        var deleteLibraryAssocSql = @"DELETE FROM library_uploads WHERE upload_id = @uploadId";
        sqlDbAccess.ExecuteNonQuery(_databaseName, deleteLibraryAssocSql, parameters);

        // Delete the upload
        var deleteUploadSql = @"DELETE FROM uploads WHERE upload_id = @uploadId";
        sqlDbAccess.ExecuteNonQuery(_databaseName, deleteUploadSql, parameters);
    }
}

public class UploadFilter : PaginationFilter
{
    public UploadFilter(List<int>? uploadIds,
        List<int>? ownerIds,
        List<int>? libraryIds,
        List<string>? uploadTypes,
        DateTime? dateFrom,
        DateTime? dateTo,
        int? pageNumber,
        int? pageSize) : base(pageNumber, pageSize)
    {
        UploadIds = uploadIds;
        OwnerIds = ownerIds;
        LibraryIds = libraryIds;
        UploadTypes = uploadTypes;
        DateFrom = dateFrom;
        DateTo = dateTo;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public UploadFilter()
    {
    }

    public List<int>? UploadIds { get; set; }
    public List<int>? OwnerIds { get; set; }
    public List<int>? LibraryIds { get; set; }
    public List<string>? UploadTypes { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
}