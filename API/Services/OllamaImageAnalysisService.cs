using System.Text.Json;

namespace API.Services;

public interface IOllamaImageAnalysisService
{
    Task<string> AnalyzeImageAsync(string imagePath);
    Task<string> AnalyzeImageWithOptionsAsync(string imagePath, OllamaImageOptions options);
}

public class OllamaImageAnalysisService(string ollamaEndpoint = "http://localhost:11434") : IOllamaImageAnalysisService
{
    private readonly HttpClient _httpClient = new()
    {
        Timeout = TimeSpan.FromMinutes(5)
    };

    public async Task<string> AnalyzeImageAsync(string imagePath)
    {
        var options = new OllamaImageOptions { };
        return await AnalyzeImageWithOptionsAsync(imagePath, options);
    }

    public async Task<string> AnalyzeImageWithOptionsAsync(string imagePath, OllamaImageOptions options)
    {
        if (!File.Exists(imagePath))
            throw new FileNotFoundException("Image file not found", imagePath);

        try
        {
            // Read image as bytes and convert to base64
            var imageBytes = await File.ReadAllBytesAsync(imagePath);
            var imageBase64 = Convert.ToBase64String(imageBytes);
            
            // Prepare request payload
            var requestContent = new
            {
                model = options.Model,
                prompt = options.Prompt,
                images = new[] { imageBase64 },
                stream = false
            };

            // Send request to Ollama API
            var response = await _httpClient.PostAsJsonAsync(
                $"{ollamaEndpoint}/api/generate", 
                requestContent);
            
            response.EnsureSuccessStatusCode();
            
            var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
            return jsonResponse.GetProperty("response").GetString() ?? "No response from model";
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to analyze image with Ollama", ex);
        }
    }
}

public class OllamaImageOptions
{
    public string Model { get; set; } = "llava";
    public string Prompt { get; set; } = "Describe what you see in this image";
    public bool Stream { get; set; } = false;
}