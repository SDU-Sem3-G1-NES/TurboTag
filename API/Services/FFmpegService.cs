using FFMpegCore;

namespace API.Services;

public interface IFFmpegService : IServiceBase
{
    public Task<String> SaveFileToTemp(IFormFile file, string fileId);
    public Task<List<String>> GetVideoAudio(string videoPath, string outputId, bool deleteInputFile);
}
public class FFmpegService : IFFmpegService
{
    private readonly string _tempFolderPath;
    public FFmpegService()
    {
        _tempFolderPath = Path.Combine(Path.GetTempPath(), "TurboTag");
        Directory.CreateDirectory(_tempFolderPath); // Ensure the folder exists
    }

    public async Task<List<String>> GetVideoAudio(string videoPath, string outputId, bool deleteInputFile)
    {
        if (!File.Exists(videoPath))
        {
            return (new List<string>());
        }
        var outputFolderPath = Path.Combine(_tempFolderPath, outputId);
        Directory.CreateDirectory(outputFolderPath);
        var audioPath = Path.Combine(outputFolderPath, "Audio.mp3");
        var snapshotPaths = new List<string>();
        var audioPaths = new List<string>();
        try
        {
            var mediaInfo = await FFProbe.AnalyseAsync(videoPath);

            if (mediaInfo.AudioStreams.Any())
            {
                FFMpeg.ExtractAudio(videoPath, audioPath);
                audioPaths = await SplitAudioTrack(audioPath);
            }
        }
        catch (Exception)
        {
            return new List<string>();
        }
        
        if (deleteInputFile)
        {
            File.Delete(videoPath);
        }
        
        return (audioPaths);
    }
    
    public async Task<String> SaveFileToTemp(IFormFile file, string fileId)
    {
        if (file.Length > 0 && (file.ContentType.StartsWith("video/") || file.ContentType.StartsWith("audio/")))
        {
            var outputFolderPath = Path.Combine(_tempFolderPath, fileId);
            Directory.CreateDirectory(outputFolderPath);

            string tempFilePath = Path.Combine(_tempFolderPath, $"{fileId}.tmp");

            await using var stream = new FileStream(tempFilePath, FileMode.Create);
            await file.CopyToAsync(stream);
            return tempFilePath;
        }
        return String.Empty;
    }
    
    private async Task<List<String>> SplitAudioTrack(string audioPath)
    {
        var outputPaths = new List<string>();
        
        var mediaInfo = await FFProbe.AnalyseAsync(audioPath);
        var bitrate = mediaInfo.AudioStreams.First().BitRate;
        int chunkDuration = 192000000 / (int)bitrate; // Duration in seconds for 24MB
        
        if (mediaInfo.Duration.TotalSeconds > chunkDuration)
        {
            var outputFolderPath = Path.GetDirectoryName(audioPath);
            int chunkIndex = 0;
            TimeSpan startTime = TimeSpan.Zero;
        
            while (startTime < TimeSpan.FromSeconds(mediaInfo.Duration.TotalSeconds))
            {
                var outputChunkPath = Path.Combine(outputFolderPath!, $"Audio-{chunkIndex + 1}.mp3");
                outputPaths.Add(outputChunkPath);
            
                TimeSpan endTime = startTime.Add(TimeSpan.FromSeconds(chunkDuration));
            
                await FFMpeg.SubVideoAsync(audioPath, outputChunkPath, startTime, endTime);
                
                startTime = endTime;
                chunkIndex++;
            }
            
            File.Delete(audioPath);
        }
        
        else
        {
            outputPaths.Add(audioPath);
        }
        
        return outputPaths;
    }
    
}