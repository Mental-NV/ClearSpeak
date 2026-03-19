using ClearSpeak.WebApp.Common.Models;

namespace ClearSpeak.WebApp.Common.Feedback;

public static class FeedbackEngine
{
    public static List<string> BuildOverallFeedback(
        double accuracy,
        double fluency,
        double completeness,
        double prosody,
        double pronunciation,
        IReadOnlyList<WordAssessment> focusWords)
    {
        var feedback = new List<string>();

        if (pronunciation >= 90)
        {
            feedback.Add("Overall pronunciation is strong. Most words are close to the target model.");
        }
        else if (pronunciation >= 75)
        {
            feedback.Add("Overall pronunciation is understandable, but several words still need correction.");
        }
        else
        {
            feedback.Add("Overall pronunciation needs more work. Focus on a small set of repeated word and phoneme issues first.");
        }

        if (accuracy < 75)
        {
            feedback.Add("Segment-level accuracy is the main issue. Slow down and exaggerate difficult consonants and vowel contrasts.");
        }

        if (fluency < 75)
        {
            feedback.Add("Fluency is below target. Read in larger thought-groups instead of word by word.");
        }

        if (completeness < 95)
        {
            feedback.Add("Completeness is below target. Some words were skipped or not recognized clearly enough.");
        }

        if (prosody > 0 && prosody < 75)
        {
            feedback.Add("Prosody needs work. Pay attention to sentence stress, rhythm, and pauses.");
        }

        var topWord = focusWords.FirstOrDefault();
        if (topWord is not null)
        {
            feedback.Add($"First repair target: '{topWord.Word}'. {topWord.Feedback}");
        }

        return feedback;
    }

    public static string BuildWordFeedback(
        string word,
        double accuracy,
        string? errorType,
        IReadOnlyList<PhonemeAssessment> phonemes)
    {
        var normalizedErrorType = string.IsNullOrWhiteSpace(errorType) ? "None" : errorType;
        var weakestPhoneme = phonemes
            .OrderBy(static phoneme => phoneme.AccuracyScore)
            .FirstOrDefault();

        string message;

        switch (normalizedErrorType)
        {
            case "Omission":
                message = $"You likely skipped '{word}' or made it too weak to detect. Make the whole word audible.";
                break;
            case "Insertion":
                message = $"An extra word or sound was detected near '{word}'. Keep the reading closer to the reference text.";
                break;
            case "Mispronunciation":
                message = $"'{word}' was marked as mispronounced. Repeat it slowly, then in the full sentence.";
                break;
            case "UnexpectedBreak":
                message = $"There seems to be an unnatural pause around '{word}'. Keep the phrase more connected.";
                break;
            case "MissingBreak":
                message = $"A pause or boundary seems to be missing near '{word}'. Separate this phrase more clearly.";
                break;
            case "Monotone":
                message = $"'{word}' is part of a flat prosody pattern. Add clearer stress and pitch movement in the sentence.";
                break;
            default:
                if (accuracy >= 90)
                {
                    message = $"'{word}' is strong.";
                }
                else if (accuracy >= 75)
                {
                    message = $"'{word}' is acceptable but can still be cleaner.";
                }
                else
                {
                    message = $"'{word}' needs clearer articulation.";
                }

                break;
        }

        if (weakestPhoneme is not null && weakestPhoneme.AccuracyScore < 75)
        {
            var alternativeHint = weakestPhoneme.Alternatives.Count > 0
                ? $" Closest alternatives: {string.Join(", ", weakestPhoneme.Alternatives.Take(3).Select(static alternative => $"/{alternative.Symbol}/ ({alternative.Score:0.#})"))}."
                : string.Empty;

            message += $" Focus on phoneme /{weakestPhoneme.Symbol}/.{alternativeHint}";
        }

        return message;
    }
}
