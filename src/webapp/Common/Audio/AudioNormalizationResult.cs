namespace ClearSpeak.WebApp.Common.Audio;

public sealed class AudioNormalizationResult
{
    public required string NormalizedFilePath { get; init; }
    public required DetectedAudioFormat OriginalFormat { get; init; }
    public required bool WasConverted { get; init; }
    public required IReadOnlyList<string> TempFiles { get; init; }
}
