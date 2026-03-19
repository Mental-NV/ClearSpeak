using ClearSpeak.WebApp.Common.Feedback;

namespace ClearSpeak.WebApp.Common.Models;

public static class AssessmentReportFactory
{
    public static AssessmentReport Create(
        string locale,
        string audioPath,
        string referenceText,
        string recognizedText,
        string? sessionId,
        string resultReason,
        ScoreSummary scores,
        IReadOnlyList<WordAssessment> allWords,
        string? rawJson)
    {
        var focusWords = allWords
            .Where(IsFocusWord)
            .OrderBy(FocusWordRank)
            .ThenBy(static word => word.AccuracyScore)
            .Take(8)
            .ToList();

        var strongWords = allWords
            .Where(static word => string.Equals(word.ErrorType, "None", StringComparison.OrdinalIgnoreCase) && word.AccuracyScore >= 90)
            .OrderByDescending(static word => word.AccuracyScore)
            .Take(5)
            .ToList();

        return new AssessmentReport
        {
            Locale = locale,
            AudioPath = audioPath,
            ReferenceText = referenceText,
            RecognizedText = recognizedText,
            SessionId = sessionId,
            ResultReason = resultReason,
            Scores = scores,
            OverallFeedback = FeedbackEngine.BuildOverallFeedback(
                scores.AccuracyScore,
                scores.FluencyScore,
                scores.CompletenessScore,
                scores.ProsodyScore,
                scores.PronunciationScore,
                focusWords),
            FocusWords = focusWords,
            StrongWords = strongWords,
            AllWords = [.. allWords],
            RawJson = rawJson
        };
    }

    public static double Round(double value) => Math.Round(value, 1, MidpointRounding.AwayFromZero);

    private static bool IsFocusWord(WordAssessment word)
    {
        if (!string.Equals(word.ErrorType, "None", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (word.AccuracyScore < 80)
        {
            return true;
        }

        return word.Phonemes.Any(static phoneme => phoneme.AccuracyScore < 70);
    }

    private static int FocusWordRank(WordAssessment word)
        => word.ErrorType switch
        {
            "Omission" => 0,
            "Insertion" => 1,
            "Mispronunciation" => 2,
            "UnexpectedBreak" => 3,
            "MissingBreak" => 4,
            "Monotone" => 5,
            _ => 6
        };
}
