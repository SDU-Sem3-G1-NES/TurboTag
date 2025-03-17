namespace API.Services.DataAccess;

public interface IDbAccess
{
    public string ReturnSqlConnectionString();
    public string ReturnNoSqlConnectionString();
}
public class DbAccess : IDbAccess
{
    private string? sqlUser { get; set; }
    private string? sqlPassword { get; set; }
    private string? sqlDbName { get; set; }
    private string? sqlPort { get; set; }
    private string? sqlHost { get; set; } 
    private string? sqlConnectionString { get; set; }
    
    private string? noSqlUser { get; set; }
    private string? noSqlPassword { get; set; }
    private string? noSqlDbName { get; set; }
    private string? noSqlPort { get; set; }
    private string? noSqlHost { get; set; }
    private string? noSqlConnectionString { get; set; }
    public string ReturnSqlConnectionString()
    {
        sqlUser = "mock";
        sqlPassword = "mock";
        sqlDbName = "mock";
        sqlHost = "localhost";
        sqlPort = "3306";
        sqlConnectionString = sqlUser + "" + sqlPassword + "" + sqlDbName+ "" + sqlHost + " " + sqlPort;
        return sqlConnectionString;
    }
    public string ReturnNoSqlConnectionString()
    {
        noSqlUser = "mock";
        noSqlPassword = "mock";
        noSqlDbName = "mock";
        noSqlHost = "localhost";
        noSqlPort = "27017";
        noSqlConnectionString = "//" + noSqlUser + ":" + noSqlPassword + "@" + noSqlHost + ":" + noSqlPort + "/" +
                                noSqlDbName;
        return noSqlConnectionString;
    }
}