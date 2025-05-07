using System.Diagnostics;
using System.Text;

namespace API.Services;

public interface IWhisperService
{
    Task<string> TranscribeAsync(string audioFilePath, string? language = null);
    Task<string> TranscribeWithOptionsAsync(string audioFilePath, WhisperOptions options);
}

public class WhisperService(string whisperPath = "whisper") : IWhisperService
{
    public async Task<string> TranscribeAsync(string audioFilePath, string? language = null)
    {
        var options = new WhisperOptions();
        if (!string.IsNullOrEmpty(language))
            options.Language = language;

        return await TranscribeWithOptionsAsync(audioFilePath, options);
    }

    public async Task<string> TranscribeWithOptionsAsync(string audioFilePath, WhisperOptions options)
    {
        if (!File.Exists(audioFilePath))
            throw new FileNotFoundException("Audio file not found", audioFilePath);

        var arguments = BuildCommandArguments(audioFilePath, options);

        var startInfo = new ProcessStartInfo
        {
            FileName = whisperPath,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = startInfo };
        var output = new StringBuilder();
        var error = new StringBuilder();

        process.OutputDataReceived += (_, args) => 
        {
            if (!string.IsNullOrEmpty(args.Data))
                output.AppendLine(args.Data);
        };

        process.ErrorDataReceived += (_, args) => 
        {
            if (!string.IsNullOrEmpty(args.Data))
                error.AppendLine(args.Data);
        };

        try
        {
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
                throw new Exception($"Whisper transcription failed: {error}");
            
            var outputFilePath = Path.ChangeExtension(audioFilePath, options.OutputFormat);
            if (File.Exists(outputFilePath))
                return await File.ReadAllTextAsync(outputFilePath);
            
            return output.ToString();
        }
        catch (Exception ex) when (ex is not FileNotFoundException)
        {
            throw new Exception("Failed to execute Whisper transcription", ex);
        }
    }

    private string BuildCommandArguments(string audioFilePath, WhisperOptions options)
    {
        var args = new StringBuilder($"\"{audioFilePath}\" --model {options.Model}");
        
        if (!string.IsNullOrEmpty(options.Language))
            args.Append($" --language {options.Language}");
            
        if (options.Translate)
            args.Append(" --task translate");
            
        if (!string.IsNullOrEmpty(options.OutputFormat))
            args.Append($" --output_format {options.OutputFormat}");
            
        args.Append($" --beam_size {options.BeamSize}");
        
        return args.ToString();
    }
}

public class WhisperOptions
{
    public string Model { get; set; } = "base";
    public string Language { get; set; }
    public string OutputFormat { get; set; } = "json";
    public bool Translate { get; set; } = false;
    public int BeamSize { get; set; } = 5;
}