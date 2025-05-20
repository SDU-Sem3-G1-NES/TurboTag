using Api.Controllers;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class OptionsController(IOptionsProviderService optionsProviderService) : ControllerBase
{
    [HttpGet("GetTagOptions")]
    public ActionResult<IEnumerable<OptionDto>> GetTagOptions([FromQuery] TagOptionsFilter filter)
    {
        return Ok(optionsProviderService.GetTagOptions(filter));
    }
}

public class OptionDto
{
    public OptionDto()
    {
    }

    public OptionDto(string displayText, string value)
    {
        DisplayText = displayText;
        Value = value;
    }

    public string? DisplayText { get; set; }
    public string? Value { get; set; }
}