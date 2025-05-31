using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenerationController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public GenerationController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("OllamaClient");
        }

        [HttpPost("StartGenerationJob")]
        public async Task<IActionResult> StartGenerationJob([FromBody] GenerationJobRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("http://localhost:8001/generate-async", request);
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "Failed to start generation job");

            return Ok();
        }
    }

    public class GenerationJobRequest
    {
        public int UploadId { get; set; }
        public string Text { get; set; } = "";
    }
}