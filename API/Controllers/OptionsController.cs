using Api.Controllers;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class OptionsController(IOptionsProviderService optionsProviderService) : ControllerBase
{
    [HttpGet("GetTagOptions")]
    public ActionResult<IEnumerable<OptionDto>> GetTagOptions([FromQuery] BaseOptionsFilter filter)
    {
        return Ok(optionsProviderService.GetTagOptions(filter));
    }

    [HttpGet("GetUploaderOptions")]
    public ActionResult<IEnumerable<OptionDto>> GetUploaderOptions([FromQuery] BaseOptionsFilter filter)
    {
        return Ok(optionsProviderService.GetUploaderOptions(filter));
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