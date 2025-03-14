namespace API.Services.DataAccess;

public class DbAccess
{
    private string? user { get; set; }
    private string? password { get; set; }
    private string? dbName { get; set; }
    private string? port { get; set; }
    private string? host { get; set; } 
    private string? connectionString { get; set; }

    string ReturnConnectionString()
    {
        user = "mock";
        password = "mock";
        dbName = "mock";
        host = "localhost";
        port = "3306";
        connectionString = user + "" + password + "" + dbName+ "" + port;
        return connectionString;
        
    }
}