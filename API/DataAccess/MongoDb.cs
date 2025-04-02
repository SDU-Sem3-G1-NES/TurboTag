namespace API.DataAccess;

public class MongoDb(string user, string password, string name, string port, string host, string connectionString)
{
    private string User { get; set; } = user;
    private string Password { get; set; } = password;
    private string Name { get; set; } = name;
    private string Port { get; set; } = port;
    private string Host { get; set; } = host;
    private string ConnectionString { get; set; } = connectionString;

    public string ReturnConnectionString()
    {
        User = "mock";
        Password = "mock";
        Name = "mock";
        Host = "localhost";
        Port = "3306";
        ConnectionString = $"Host={Host};Port={Port};Database={Name};User Id={User};Password={Password};";
        return ConnectionString;
    }
}