using Dapper;
using Npgsql;

namespace API.DataAccess;

public class SqlDataAccess(string connectionString) : ISqlDbAccess
{
    public void ExecuteNonQuery(string databaseName, string query, Dictionary<string, object> parameters)
    {
        var newConnectionString = connectionString + $";Database={databaseName}";

        using var connection = new NpgsqlConnection(newConnectionString);
        connection.Open();

        connection.Execute(query, parameters);
    }

    public IEnumerable<T> GetPagedResult<T>(
        string databaseName,
        string selectSql,
        string fromWhereSql,
        string orderBy,
        Dictionary<string, object> parameters,
        int pageNumber,
        int pageSize)
    {
        var newConnectionString = connectionString + $";Database={databaseName}";
        var query =
            $"{selectSql} {fromWhereSql} {orderBy} OFFSET {(pageNumber - 1) * pageSize} ROWS FETCH NEXT {pageSize} ROWS ONLY";

        using var connection = new NpgsqlConnection(newConnectionString);
        connection.Open();

        return connection.Query<T>(query, parameters);
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
}