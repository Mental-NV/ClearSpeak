using System.ComponentModel;
using System.Diagnostics;

namespace ClearSpeak.WebApp.Common.Audio;

public sealed class ProcessRunner : IProcessRunner
{
    public async Task<ProcessResult> RunAsync(string fileName, IReadOnlyList<string> arguments, CancellationToken cancellationToken = default)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = fileName,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        foreach (var argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        using var process = new Process { StartInfo = startInfo };

        try
        {
            process.Start();
        }
        catch (Exception ex) when (ex is Win32Exception or InvalidOperationException)
        {
            throw new AudioNormalizationException($"Unable to start '{fileName}'. Install ffmpeg or set Pronunciation:FfmpegPath.", isDependencyFailure: true);
        }

        var standardOutputTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
        var standardErrorTask = process.StandardError.ReadToEndAsync(cancellationToken);

        await process.WaitForExitAsync(cancellationToken);

        return new ProcessResult
        {
            ExitCode = process.ExitCode,
            StandardOutput = await standardOutputTask,
            StandardError = await standardErrorTask
        };
    }
}
