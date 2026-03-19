namespace ClearSpeak.WebApp.Common.Audio;

public sealed class ProcessResult
{
    public required int ExitCode { get; init; }
    public required string StandardOutput { get; init; }
    public required string StandardError { get; init; }
}
