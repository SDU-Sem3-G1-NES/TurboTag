using FFMpegCore;

namespace API.Services;

public interface IFFmpegService : IServiceBase
{
    public Task<String> SaveFileToTemp(IFormFile file, string mongoId);
    public Task<(List<String>, List<String>)> GetVideoAudioAndSnapshots(string videoPath, string outputId, bool deleteInputFile);
}
public class FFmpegService : IFFmpegService
{
    private readonly string _tempFolderPath;
    public FFmpegService()
    {
        _tempFolderPath = Path.Combine(Path.GetTempPath(), "TurboTag");
        Directory.CreateDirectory(_tempFolderPath); // Ensure the folder exists
    }

    public async Task<(List<String>, List<String>)> GetVideoAudioAndSnapshots(string videoPath, string outputId, bool deleteInputFile)
    {
        var outputFolderPath = Path.Combine(_tempFolderPath, outputId);
        Directory.CreateDirectory(outputFolderPath);

        var audioPath = Path.Combine(outputFolderPath, "Audio.mp3");
        var snapshotPaths = new List<string>();

        FFMpeg.ExtractAudio(videoPath, audioPath);

        var audioPaths = await SplitAudioTrack(audioPath);
        
        var mediaInfo = await FFProbe.AnalyseAsync(videoPath);
        int snapshotPeriod = (int)mediaInfo.Duration.TotalSeconds/5;

        for (int i = 0; i < 5; i++)
        {
            var snapshotPath = Path.Combine(outputFolderPath, $"Snapshot-{i+1}.jpg");
            await FFMpeg.SnapshotAsync(videoPath, snapshotPath, null, TimeSpan.FromSeconds(i*snapshotPeriod));
            snapshotPaths.Add(snapshotPath);
        }
        
        if (deleteInputFile)
        {
            File.Delete(videoPath);
        }
        
        return (audioPaths, snapshotPaths);
    }
    
    public async Task<String> SaveFileToTemp(IFormFile file, string mongoId)
    {
        var outputFolderPath = Path.Combine(_tempFolderPath, mongoId);
        Directory.CreateDirectory(outputFolderPath);

        var tempFilePath = Path.Combine(_tempFolderPath, $"{mongoId}.tmp");
        
        await using (var stream = new FileStream(tempFilePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }
        
        return tempFilePath;
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