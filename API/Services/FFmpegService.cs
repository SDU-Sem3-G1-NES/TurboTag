using System.Drawing;
using FFMpegCore;

namespace API.Services;

public interface IFFmpegService : IServiceBase
{
    public Task<String> SaveFileToTemp(IFormFile file, string fileId);
    public Task<List<String>> GetVideoAudio(string videoPath, string outputId, bool deleteInputFile);
    public Task<String> MakeVideoThumbnail(string videoPath);
}
public class FFmpegService : IFFmpegService
{
    private readonly string _tempFolderPath;
    private readonly IFileService _fileService;
    public FFmpegService(IFileService fileService)
    {
        _fileService = fileService;
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
    
    public async Task<String> MakeVideoThumbnail(string videoPath)
    {
        if (!File.Exists(videoPath))
        {
            return String.Empty;
        }
        try
        {
            var thumbnailPath = await GetVideoSnapshot(videoPath, Guid.NewGuid().ToString());
            await using var thumbnailStream = System.IO.File.OpenRead(thumbnailPath);
            var mongoId = await _fileService.UploadChunkedFile(thumbnailStream, $"thumbnail-{Guid.NewGuid().ToString()}.png") ?? "";
            File.Delete(thumbnailPath);
            return mongoId;
        }
        catch (Exception)
        {
            return String.Empty;
        }
    }
    
    private async Task<String> GetVideoSnapshot(string videoPath, string outputId)
    {
        var outputFolderPath = Path.Combine(_tempFolderPath, outputId);
        Directory.CreateDirectory(outputFolderPath);
        var thumbnailPath = Path.Combine(outputFolderPath, $"snapshot-{outputId}.png");
        try
        {
            var mediaInfo = await FFProbe.AnalyseAsync(videoPath);
            if (mediaInfo.VideoStreams.Any())
            {
                var videoStream = mediaInfo.VideoStreams.First();
                var originalWidth = videoStream.Width;
                var originalHeight = videoStream.Height;
                
                int targetHeight = 160;
                int targetWidth = (int)((double)originalWidth / originalHeight * targetHeight);

                var thumbnailSize = new Size(targetWidth, targetHeight);
                await FFMpeg.SnapshotAsync(videoPath, thumbnailPath, thumbnailSize,
                    TimeSpan.FromSeconds(5));
            }
        }
        catch (Exception)
        {
            return String.Empty;
        }
        return thumbnailPath;
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