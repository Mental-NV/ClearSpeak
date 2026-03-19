using ClearSpeak.WebApp.Common.Models;

namespace ClearSpeak.WebApp.Features.Pronunciation;

public sealed class AnalyzePronunciationResponse
{
    public required ScoreCardResponse Scores { get; init; }
    public required string Summary { get; init; }
    public required IReadOnlyList<string> NextSteps { get; init; }
    public required IReadOnlyList<AnalyzedWordResponse> Words { get; init; }
    public string RecognizedText { get; init; } = string.Empty;
    public string? SessionId { get; init; }

    public static AnalyzePronunciationResponse FromReport(AssessmentReport report)
    {
        var nextSteps = report.OverallFeedback
            .Skip(1)
            .Where(static line => !line.StartsWith("Overall pronunciation", StringComparison.OrdinalIgnoreCase))
            .Take(2)
            .ToList();

        if (report.FocusWords.Count > 0 && nextSteps.Count < 3)
        {
            var focusWord = report.FocusWords[0];
            var fallbackStep = $"Practice '{focusWord.Word}' slowly, then reread the sentence.";
            if (!nextSteps.Contains(fallbackStep, StringComparer.OrdinalIgnoreCase))
            {
                nextSteps.Add(fallbackStep);
            }
        }

        return new AnalyzePronunciationResponse
        {
            Scores = new ScoreCardResponse
            {
                Pronunciation = report.Scores.PronunciationScore,
                Accuracy = report.Scores.AccuracyScore,
                Fluency = report.Scores.FluencyScore,
                Completeness = report.Scores.CompletenessScore,
                Prosody = report.Scores.ProsodyScore
            },
            Summary = report.OverallFeedback.FirstOrDefault() ?? "Your speech was analyzed.",
            NextSteps = nextSteps.Take(3).ToList(),
            RecognizedText = report.RecognizedText,
            SessionId = report.SessionId,
            Words = report.AllWords.Select(word => new AnalyzedWordResponse
            {
                Text = word.Word,
                Score = word.AccuracyScore,
                ErrorType = word.ErrorType,
                Feedback = word.Feedback,
                IsFocusWord = report.FocusWords.Any(focusWord => string.Equals(focusWord.Word, word.Word, StringComparison.OrdinalIgnoreCase) &&
                    focusWord.OffsetTicks == word.OffsetTicks),
                OffsetTicks = word.OffsetTicks == 0 ? null : word.OffsetTicks,
                DurationTicks = word.DurationTicks == 0 ? null : word.DurationTicks,
                Phonemes = word.Phonemes.Select(static phoneme => new AnalyzedPhonemeResponse
                {
                    Phoneme = phoneme.Symbol,
                    Score = phoneme.AccuracyScore,
                    Alternatives = phoneme.Alternatives.Select(static alternative => new PhonemeAlternativeResponse
                    {
                        Phoneme = alternative.Symbol,
                        Score = alternative.Score
                    }).ToList()
                }).ToList()
            }).ToList()
        };
    }
}

public sealed class ScoreCardResponse
{
    public double Pronunciation { get; init; }
    public double Accuracy { get; init; }
    public double Fluency { get; init; }
    public double Completeness { get; init; }
    public double Prosody { get; init; }
}

public sealed class AnalyzedWordResponse
{
    public required string Text { get; init; }
    public double Score { get; init; }
    public required string ErrorType { get; init; }
    public required string Feedback { get; init; }
    public required bool IsFocusWord { get; init; }
    public long? OffsetTicks { get; init; }
    public long? DurationTicks { get; init; }
    public required IReadOnlyList<AnalyzedPhonemeResponse> Phonemes { get; init; }
}

public sealed class AnalyzedPhonemeResponse
{
    public required string Phoneme { get; init; }
    public double Score { get; init; }
    public required IReadOnlyList<PhonemeAlternativeResponse> Alternatives { get; init; }
}

public sealed class PhonemeAlternativeResponse
{
    public required string Phoneme { get; init; }
    public double Score { get; init; }
}
