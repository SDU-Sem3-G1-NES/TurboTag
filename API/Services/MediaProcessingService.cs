using System.Text;

namespace API.Services;

public class MediaProcessingService(IWhisperService whisperService, IOllamaImageAnalysisService ollamaImageAnalysisService, IFileService fileService) : IServiceBase
{
    public async Task<string> ProcessMediaAsync(List<string> audioFilePath, List<string> imageFilePath)
    {
        var finalDescription = new StringBuilder();
        try
        {
            foreach (var audioPath in audioFilePath)
            {
                var transcription = await whisperService.TranscribeAsync(audioPath);
                var fileDirectory = Path.GetDirectoryName(audioPath);
                var objectId = Path.GetFileName(fileDirectory);
                var fileInfo = fileService.GetFileInfoByObjectId(objectId);
                if (fileInfo != null)
                {
                    fileInfo.Description += transcription;
                    fileService.UpdateFileInfo(fileInfo);
                    finalDescription.Append(fileInfo.Description);
                }
            }
            foreach (var imagePath in imageFilePath)
            {
                var imageAnalysis = await ollamaImageAnalysisService.AnalyzeImageAsync(imagePath);
                var fileDirectory = Path.GetDirectoryName(imagePath);
                var objectId = Path.GetFileName(fileDirectory);
                var fileInfo = fileService.GetFileInfoByObjectId(objectId);
                if (fileInfo != null)
                {
                    fileInfo.Description += imageAnalysis;
                    fileService.UpdateFileInfo(fileInfo);
                    finalDescription.Append(fileInfo.Description);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing media: {ex.Message}");
            throw;
        }
        return finalDescription.ToString();
    }
}