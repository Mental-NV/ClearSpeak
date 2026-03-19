using ClearSpeak.WebApp.Common.Models;
using ClearSpeak.WebApp.Features.Pronunciation;

namespace ClearSpeak.WebApp.Tests.Features;

public sealed class AnalyzePronunciationResponseTests
{
    [Fact]
    public void FromReport_UsesOverallFeedbackForSummaryAndNextSteps()
    {
        var report = new AssessmentReport
        {
            Scores = new ScoreSummary
            {
                PronunciationScore = 72,
                AccuracyScore = 70,
                FluencyScore = 81,
                CompletenessScore = 100,
                ProsodyScore = 68
            },
            OverallFeedback =
            [
                "Your speech was understandable, but a few words need correction.",
                "Practice the word 'three' several times.",
                "Repeat the first sentence more smoothly."
            ],
            FocusWords =
            [
                new WordAssessment
                {
                    Word = "Three",
                    AccuracyScore = 54,
                    ErrorType = "Mispronunciation",
                    Feedback = "Focus on /theta/."
                }
            ],
            AllWords =
            [
                new WordAssessment
                {
                    Word = "Three",
                    AccuracyScore = 54,
                    ErrorType = "Mispronunciation",
                    Feedback = "Focus on /theta/."
                }
            ]
        };

        var response = AnalyzePronunciationResponse.FromReport(report);

        Assert.Equal(report.OverallFeedback[0], response.Summary);
        Assert.Equal(3, response.NextSteps.Count);
        Assert.Equal("Three", response.Words[0].Text);
    }
}
