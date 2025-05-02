using FFMpegCore;

namespace API.Services;

public interface IFFmpegService : IServiceBase
{
    public Task<String> SaveFileToTemp(IFormFile file, string mongoId);
    public Task<(String, List<String>)> GetVideoAudioAndSnapshots(string videoPath, string outputId, bool deleteInputFile);

}
public class FFmpegService : IFFmpegService
{
    private readonly string _tempFolderPath;
    public FFmpegService()
    {
        _tempFolderPath = Path.Combine(Path.GetTempPath(), "TurboTag");
        Directory.CreateDirectory(_tempFolderPath); // Ensure the folder exists
    }

    public async Task<(String, List<String>)> GetVideoAudioAndSnapshots(string videoPath, string outputId, bool deleteInputFile)
    {
        var outputFolderPath = Path.Combine(_tempFolderPath, outputId);
        Directory.CreateDirectory(outputFolderPath);

        var audioPath = Path.Combine(outputFolderPath, "Audio.mp3");
        var snapshotPaths = new List<string>();

        FFMpeg.ExtractAudio(videoPath, audioPath);

        var mediaInfo = await FFProbe.AnalyseAsync(videoPath);
        int snapshotPeriod = (int)mediaInfo.Duration.TotalSeconds/5;

        for (int i = 1; i <= 5; i++)
        {
            await FFMpeg.SnapshotAsync(videoPath, Path.Combine(outputFolderPath, $"Snapshot-{i}.jpg"), null, TimeSpan.FromSeconds(i*snapshotPeriod));
        }

        if (deleteInputFile)
        {
            File.Delete(videoPath);
        }

        return (audioPath, snapshotPaths);
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
    
}