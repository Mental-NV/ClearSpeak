using System.Text.RegularExpressions;
using ClearSpeak.WebApp.Common.Feedback;
using ClearSpeak.WebApp.Common.Models;
using Microsoft.Extensions.Logging;

namespace ClearSpeak.WebApp.Common.AzureSpeech;

public sealed partial class FakePronunciationAssessmentProvider : IPronunciationAssessmentProvider
{
    private readonly ILogger<FakePronunciationAssessmentProvider> _logger;

    public FakePronunciationAssessmentProvider(ILogger<FakePronunciationAssessmentProvider> logger)
    {
        _logger = logger;
    }

    public Task<AssessmentReport> AssessAsync(PronunciationAssessmentRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Using fake pronunciation provider for text length {TextLength}.", request.ReferenceText.Length);

        var words = WordRegex().Matches(request.ReferenceText)
            .Select(static match => match.Value)
            .ToList();

        var assessedWords = new List<WordAssessment>(words.Count);
        var offset = 0L;

        for (var index = 0; index < words.Count; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            assessedWords.Add(CreateWord(words[index], index, ref offset));
        }

        var accuracy = assessedWords.Count == 0 ? 0 : AssessmentReportFactory.Round(assessedWords.Average(static word => word.AccuracyScore));
        var pronunciation = accuracy;
        var completeness = assessedWords.Any(static word => string.Equals(word.ErrorType, "Omission", StringComparison.OrdinalIgnoreCase)) ? 92 : 100;
        var fluency = words.Count > 10 ? 82 : 88;
        var prosody = 74;

        var report = AssessmentReportFactory.Create(
            request.Locale,
            request.AudioPath,
            request.ReferenceText,
            string.Join(" ", words),
            sessionId: "fake-session",
            resultReason: "RecognizedSpeech",
            new ScoreSummary
            {
                AccuracyScore = accuracy,
                FluencyScore = fluency,
                CompletenessScore = completeness,
                ProsodyScore = prosody,
                PronunciationScore = pronunciation
            },
            assessedWords,
            rawJson: null);

        return Task.FromResult(report);
    }

    private static WordAssessment CreateWord(string word, int index, ref long offsetTicks)
    {
        var normalized = word.ToLowerInvariant();
        var errorType = "None";
        var score = 92d;
        var phonemes = new List<PhonemeAssessment>();

        if (normalized.Contains("th", StringComparison.Ordinal))
        {
            errorType = "Mispronunciation";
            score = 58;
            phonemes.Add(CreatePhoneme("θ", 43, offsetTicks, 1_800_000, ("t", 61), ("s", 22)));
        }
        else if (normalized.Contains("w", StringComparison.Ordinal))
        {
            errorType = "Mispronunciation";
            score = 69;
            phonemes.Add(CreatePhoneme("w", 60, offsetTicks, 1_800_000, ("v", 48), ("u", 20)));
        }
        else if (normalized.Contains("r", StringComparison.Ordinal) && normalized.Length > 4)
        {
            score = 77;
            phonemes.Add(CreatePhoneme("ɹ", 71, offsetTicks, 1_600_000, ("l", 33)));
        }
        else if (index == 5)
        {
            errorType = "MissingBreak";
            score = 74;
        }
        else if (index == 10)
        {
            errorType = "Omission";
            score = 40;
        }

        if (phonemes.Count == 0 && score < 85)
        {
            phonemes.Add(CreatePhoneme("ə", score, offsetTicks, 1_200_000));
        }

        var duration = 3_500_000L;
        var roundedScore = AssessmentReportFactory.Round(score);

        var assessedWord = new WordAssessment
        {
            Word = word,
            AccuracyScore = roundedScore,
            ErrorType = errorType,
            OffsetTicks = offsetTicks,
            DurationTicks = duration,
            Feedback = FeedbackEngine.BuildWordFeedback(word, roundedScore, errorType, phonemes),
            Phonemes = phonemes
        };

        offsetTicks += duration;
        return assessedWord;
    }

    private static PhonemeAssessment CreatePhoneme(string symbol, double score, long offsetTicks, long durationTicks, params (string Symbol, double Score)[] alternatives)
        => new()
        {
            Symbol = symbol,
            AccuracyScore = score,
            OffsetTicks = offsetTicks,
            DurationTicks = durationTicks,
            Alternatives =
            [
                .. alternatives.Select(static alternative => new NBestPhonemeAssessment
                {
                    Symbol = alternative.Symbol,
                    Score = alternative.Score
                })
            ]
        };

    [GeneratedRegex("[A-Za-z']+")]
    private static partial Regex WordRegex();
}
