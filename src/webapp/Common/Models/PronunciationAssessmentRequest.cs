namespace ClearSpeak.WebApp.Common.Models;

public sealed class PronunciationAssessmentRequest
{
    public required string AudioPath { get; init; }
    public required string ReferenceText { get; init; }
    public string Locale { get; init; } = "en-US";
    public bool IncludeRawJson { get; init; }
}
