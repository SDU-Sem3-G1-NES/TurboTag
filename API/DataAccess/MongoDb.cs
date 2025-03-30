namespace API.DataAccess;

public class MongoDb(string user, string password, string name, string port, string host, string connectionString)
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
        port = "3306";
        connectionString = $"Host={host};Port={port};Database={name};User Id={user};Password={password};";
        return connectionString;
    }
}