using FFMpegCore;

namespace API.Services;

public interface IFFmpegService : IServiceBase
{
    public Task<(String, List<String>)> GetVideoAudioAndSnapshots(IFormFile video, string fileId);

}
public class FFmpegService : IFFmpegService
{
    private readonly string _tempFolderPath;
    public FFmpegService()
    {
        _tempFolderPath = Path.Combine(Path.GetTempPath(), "TurboTag");
        Directory.CreateDirectory(_tempFolderPath); // Ensure the folder exists
    }

    public async Task<(String, List<String>)> GetVideoAudioAndSnapshots(IFormFile video, string mongoId)
    {
        var outputFolderPath = Path.Combine(_tempFolderPath, mongoId);
        Directory.CreateDirectory(outputFolderPath);

        var audioPath = Path.Combine(outputFolderPath, "Audio.mp3");
        var snapshotPaths = new List<string>();

        var tempFilePath = Path.Combine(_tempFolderPath, $"{Guid.NewGuid()}.tmp");
        await using (var stream = new FileStream(tempFilePath, FileMode.Create))
        {
            await video.CopyToAsync(stream);
        }

        FFMpeg.ExtractAudio(tempFilePath, audioPath);

        var mediaInfo = await FFProbe.AnalyseAsync(tempFilePath);
        int snapshotPeriod = (int)mediaInfo.Duration.TotalSeconds/5;

        for (int i = 1; i <= 5; i++)
        {
            await FFMpeg.SnapshotAsync(tempFilePath, Path.Combine(outputFolderPath, $"Snapshot-{i}.jpg"), null, TimeSpan.FromSeconds(i*snapshotPeriod));
        }

        File.Delete(tempFilePath);

        return (audioPath, snapshotPaths);
    }
}