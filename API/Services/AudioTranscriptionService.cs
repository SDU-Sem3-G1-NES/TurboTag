using API.DTOs;

namespace API.Services;

public interface IAudioTranscriptionService : IServiceBase
{
    Task<string> AudioTranscriptionAsync(List<string> audioFilePaths);
}

public class AudioTranscriptionService(IFileService fileService) : IAudioTranscriptionService
{
    public async Task<string> AudioTranscriptionAsync(List<string> audioFilePaths)
    {
        string transcription;
        try
        {
            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromMinutes(10);
            var apiUrl = "http://localhost:8000/transcribe-paths/";
            var response = await httpClient.PostAsJsonAsync(apiUrl, audioFilePaths);
            response.EnsureSuccessStatusCode();
            transcription = await response.Content.ReadAsStringAsync();
            var filePath = audioFilePaths.FirstOrDefault();
            var objectId = filePath != null ? Path.GetFileName(Path.GetDirectoryName(filePath)) : null;
            var fileInfo = fileService.GetFileInfoByObjectId(objectId!);
            if (fileInfo is { MongoId: not null, Filename: not null })
            {
                var updatedFileInfo = new FileInfoDto(
                    fileInfo.MongoId,
                    fileInfo.Length,
                    fileInfo.ChunkSize,
                    fileInfo.UploadDate,
                    fileInfo.Filename,
                    transcription
                );
                fileService.UpdateFileInfo(updatedFileInfo);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing media: {ex.Message}");
            throw;
        }
        return transcription;
    }
}