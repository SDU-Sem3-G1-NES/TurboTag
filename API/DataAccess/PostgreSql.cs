namespace API.DataAccess;

public class PostgreSql(string user, string password, string name, string port, string host, string connectionString) : IDbAccess
{
    private string user { get; set; } = user;
    private string password { get; set; } = password;
    private string name { get; set; } = name;
    private string port { get; set; } = port;
    private string host { get; set; } = host;
    private string connectionString { get; set; } = connectionString;
    public string ReturnConnectionString()
    {
        user = "mock";
        password = "mock";
        name = "mock";
        host = "localhost";
        port = "27017";
        connectionString = "//" + user + ":" + password + "@" + host + ":" + port + "/" +
                                name;
        return connectionString;
    }
}
