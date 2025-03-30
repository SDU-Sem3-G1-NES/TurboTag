using Microsoft.Data.SqlClient;

namespace API.DataAccess;

public class SqlDataAccess(string connectionString) : ISqlDbAccess
{
    public void ExecuteNonQuery(string databaseName, string query, Dictionary<string, object> parameters)
    {
        var newConnectionString = connectionString + $";Database={databaseName}";

        using var connection = new SqlConnection(newConnectionString);
        connection.Open();

        using var command = new SqlCommand(query, connection);
        foreach (var parameter in parameters) command.Parameters.AddWithValue(parameter.Key, parameter.Value);

        command.ExecuteNonQuery();
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
        var query = selectSql + fromWhereSql + orderBy;

        using var connection = new SqlConnection(newConnectionString);
        connection.Open();

        using var command = new SqlCommand(query, connection);
        foreach (var parameter in parameters) command.Parameters.AddWithValue(parameter.Key, parameter.Value);
        command.CommandText += $" OFFSET {pageNumber * pageSize} ROWS FETCH NEXT {pageSize} ROWS ONLY";

        using var reader = command.ExecuteReader();
        var results = new List<T>();

        while (reader.Read())
        {
            var result = Activator.CreateInstance<T>();
            foreach (var property in typeof(T).GetProperties()) property.SetValue(result, reader[property.Name]);
            results.Add(result);
        }

        return results;
    }

    public IEnumerable<T> ExecuteQuery<T>(
        string databaseName,
        string selectSql,
        string fromWhereSql,
        string orderBy,
        Dictionary<string, object> parameters)
    {
        var newConnectionString = connectionString + $";Database={databaseName}";
        var query = selectSql + fromWhereSql + orderBy;

        using var connection = new SqlConnection(newConnectionString);
        connection.Open();

        using var command = new SqlCommand(query, connection);
        foreach (var parameter in parameters) command.Parameters.AddWithValue(parameter.Key, parameter.Value);

        using var reader = command.ExecuteReader();
        var results = new List<T>();

        while (reader.Read())
        {
            var result = Activator.CreateInstance<T>();
            foreach (var property in typeof(T).GetProperties()) property.SetValue(result, reader[property.Name]);
            results.Add(result);
        }

        return results;
    }
}