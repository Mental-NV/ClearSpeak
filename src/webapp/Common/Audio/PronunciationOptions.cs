namespace ClearSpeak.WebApp.Common.Audio;

public sealed class PronunciationOptions
{
    public const string SectionName = "Pronunciation";

    public string Provider { get; init; } = "Azure";
    public string AllowedLocale { get; init; } = "en-US";
    public long MaxUploadBytes { get; init; } = 10 * 1024 * 1024;
    public string FfmpegPath { get; init; } = "ffmpeg";
}
