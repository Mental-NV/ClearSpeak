using System.Text.Json;
using System.Text.Json.Serialization;

namespace ClearSpeak.WebApp.Common.Models;

public sealed class AssessmentReport
{
    public string Locale { get; init; } = "en-US";
    public string AudioPath { get; init; } = string.Empty;
    public string ReferenceText { get; init; } = string.Empty;
    public string RecognizedText { get; init; } = string.Empty;
    public string? SessionId { get; init; }
    public string ResultReason { get; init; } = string.Empty;
    public ScoreSummary Scores { get; init; } = new();
    public List<string> OverallFeedback { get; init; } = [];
    public List<WordAssessment> FocusWords { get; init; } = [];
    public List<WordAssessment> StrongWords { get; init; } = [];
    public List<WordAssessment> AllWords { get; init; } = [];
    public string? RawJson { get; init; }
}

public sealed class ScoreSummary
{
    public double AccuracyScore { get; init; }
    public double FluencyScore { get; init; }
    public double CompletenessScore { get; init; }
    public double ProsodyScore { get; init; }
    public double PronunciationScore { get; init; }
}

public sealed class WordAssessment
{
    public string Word { get; init; } = string.Empty;
    public double AccuracyScore { get; init; }
    public string ErrorType { get; init; } = "None";
    public string Feedback { get; init; } = string.Empty;
    public long OffsetTicks { get; init; }
    public long DurationTicks { get; init; }
    public List<PhonemeAssessment> Phonemes { get; init; } = [];
}

public sealed class PhonemeAssessment
{
    public string Symbol { get; init; } = string.Empty;
    public double AccuracyScore { get; init; }
    public long OffsetTicks { get; init; }
    public long DurationTicks { get; init; }
    public List<NBestPhonemeAssessment> Alternatives { get; init; } = [];
}

public sealed class NBestPhonemeAssessment
{
    public string Symbol { get; init; } = string.Empty;
    public double Score { get; init; }
}

public static class JsonDefaults
{
    public static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
}
