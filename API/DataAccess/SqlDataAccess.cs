using System.Data;
using Dapper;
using Npgsql;

namespace API.DataAccess;

public interface ISqlDbAccess
{
    public void ExecuteNonQuery(
        string databaseName,
        string query,
        Dictionary<string, object> parameters);

    public IEnumerable<T> ExecuteQuery<T>(
        string databaseName,
        string selectSql,
        string fromWhereSql,
        string orderBy,
        Dictionary<string, object> parameters);

    public PagedResult<T> GetPagedResult<T>(
        string databaseName,
        string selectSql,
        string fromWhereSql,
        string orderBy,
        Dictionary<string, object> parameters,
        int pageNumber,
        int pageSize);
}

public class SqlDataAccess(string connectionString) : ISqlDbAccess
{
    public void ExecuteNonQuery(string databaseName, string query, Dictionary<string, object> parameters)
    {
        var newConnectionString = connectionString + $";Database={databaseName}";

        using var connection = new NpgsqlConnection(newConnectionString);
        connection.Open();

        connection.Execute(query, parameters);
    }

    public IEnumerable<T> ExecuteQuery<T>(
        string databaseName,
        string selectSql,
        string fromWhereSql,
        string orderBy,
        Dictionary<string, object> parameters)
    {
        var newConnectionString = connectionString + $";Database={databaseName}";
        var query = $"{selectSql} {fromWhereSql} {orderBy}";

        using var connection = new NpgsqlConnection(newConnectionString);
        connection.Open();

        return connection.Query<T>(query, parameters);
    }

    public PagedResult<T> GetPagedResult<T>(
        string databaseName,
        string selectSql,
        string fromWhereSql,
        string orderBy,
        Dictionary<string, object> parameters,
        int pageNumber,
        int pageSize)
    {
        var newConnectionString = connectionString + $";Database={databaseName}";
        selectSql += ", COUNT(*) OVER() as TotalCount ";
        var query =
            $"{selectSql} {fromWhereSql} {orderBy} OFFSET {(pageNumber - 1) * pageSize} ROWS FETCH NEXT {pageSize} ROWS ONLY";

        using var connection = new NpgsqlConnection(newConnectionString);
        connection.Open();

        var result = connection.Query<T, long, (T IThreadPoolWorkItem, long TotalCount)>(
            query,
            (item, totalCount) => (item, totalCount),
            parameters,
            splitOn: "TotalCount").ToList();

        return new PagedResult<T>
        {
            Items = result.Select(r => r.Item1).ToList(),
            TotalCount = (int)result.FirstOrDefault().Item2,
            PageSize = pageSize,
            CurrentPage = pageNumber,
            TotalPages = (int)Math.Ceiling(result.FirstOrDefault().TotalCount / (double)pageSize)
        };
    }
}

public class GenericListHandler<T> : SqlMapper.TypeHandler<List<T>>
{
    public override void SetValue(IDbDataParameter parameter, List<T>? value)
    {
        parameter.Value = value;
    }

    public override List<T> Parse(object value)
    {
        if (value is DBNull)
            return new List<T>();

        var result = ((IEnumerable<T>)value).ToList();
        return result;
    }
}