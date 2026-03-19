namespace ClearSpeak.WebApp.Common.AzureSpeech;

public sealed class AzureSpeechOptions
{
    public const string SectionName = "AzureSpeech";

    public string Endpoint { get; init; } = string.Empty;
    public string Key { get; init; } = string.Empty;
}
