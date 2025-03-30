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

    public IEnumerable<T> GetPagedResult<T>(
        string databaseName,
        string selectSql,
        string fromWhereSql,
        string orderBy,
        Dictionary<string, object> parameters,
        int pageNumber,
        int pageSize);
}