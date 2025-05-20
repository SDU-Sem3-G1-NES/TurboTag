using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
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
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpPost("generate")]
        public async Task<GenerationResult> Generate([FromBody] string request)
        {
            var pythonResponse = await _httpClient.PostAsJsonAsync("http://localhost:8000/generate-content", new { text = request });

            if (!pythonResponse.IsSuccessStatusCode)
                throw new HttpRequestException($"Failed to generate content. Status code: {(int)pythonResponse.StatusCode}");

            var result = await pythonResponse.Content.ReadFromJsonAsync<GenerationResult>();

            if (result == null)
                throw new HttpRequestException("Invalid response from the server");

            return result;
        }
    }

    public class GenerationResult
    {
        public string? Tags { get; set; }
        public string? Description { get; set; }
    }
}