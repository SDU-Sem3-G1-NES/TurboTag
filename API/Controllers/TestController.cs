using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public interface ITestBase;

public interface IStringTest : ITestBase
{
    string GetString();
}

public interface IIntTest : ITestBase
{
    int GetInt();
}

public class StringTestImplementation : IStringTest
{
    public string GetString()
    {
        return "Hello, World!";
    }
}

public class IntTestImplementation : IIntTest
{
    public int GetInt()
    {
        return 42;
    }
}

[ApiController]
[Route("[controller]")]
public class TestController(IStringTest _stringTest, IIntTest _intTest) : ControllerBase
{
    [HttpGet]
    public string Get()
    {
        return $"{_stringTest.GetString()} - {_intTest.GetInt()}";
        ;
    }
}