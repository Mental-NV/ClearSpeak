using ClearSpeak.WebApp.Common.Feedback;
using ClearSpeak.WebApp.Common.Models;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.PronunciationAssessment;
using Microsoft.Extensions.Options;

namespace ClearSpeak.WebApp.Common.AzureSpeech;

public sealed class AzureSpeechPronunciationAssessmentProvider : IPronunciationAssessmentProvider
{
    private readonly AzureSpeechOptions _options;

    public AzureSpeechPronunciationAssessmentProvider(IOptions<AzureSpeechOptions> options)
    {
        _options = options.Value;
    }

    public async Task<AssessmentReport> AssessAsync(PronunciationAssessmentRequest request, CancellationToken cancellationToken = default)
    {
        var endpoint = ResolveRequiredValue(
            _options.Endpoint,
            Environment.GetEnvironmentVariable("AZURE_SPEECH_ENDPOINT"),
            "Azure endpoint is required. Set AzureSpeech:Endpoint or AZURE_SPEECH_ENDPOINT.");
        var key = ResolveRequiredValue(
            _options.Key,
            Environment.GetEnvironmentVariable("AZURE_SPEECH_KEY"),
            "Azure key is required. Set AzureSpeech:Key or AZURE_SPEECH_KEY.");

        if (!Uri.TryCreate(endpoint, UriKind.Absolute, out var endpointUri) ||
            (endpointUri.Scheme != Uri.UriSchemeHttps && endpointUri.Scheme != Uri.UriSchemeHttp))
        {
            throw new PronunciationConfigurationException("AzureSpeech:Endpoint must be a valid absolute URI.");
        }

        var speechConfig = SpeechConfig.FromEndpoint(endpointUri, key);
        speechConfig.SpeechRecognitionLanguage = request.Locale;

        using var audioConfig = AudioConfig.FromWavFileInput(request.AudioPath);
        using var recognizer = new SpeechRecognizer(speechConfig, audioConfig);

        string? sessionId = null;
        recognizer.SessionStarted += (_, eventArgs) => sessionId = eventArgs.SessionId;

        var pronunciationConfig = new PronunciationAssessmentConfig(
            request.ReferenceText,
            GradingSystem.HundredMark,
            Granularity.Phoneme,
            enableMiscue: true);

        pronunciationConfig.PhonemeAlphabet = "IPA";
        pronunciationConfig.NBestPhonemeCount = 5;
        pronunciationConfig.EnableProsodyAssessment();
        pronunciationConfig.ApplyTo(recognizer);

        cancellationToken.ThrowIfCancellationRequested();
        var result = await recognizer.RecognizeOnceAsync().ConfigureAwait(false);

        if (result.Reason == ResultReason.Canceled)
        {
            var cancellation = CancellationDetails.FromResult(result);
            throw new InvalidOperationException(
                $"Azure Speech request was canceled. Reason={cancellation.Reason}; ErrorCode={cancellation.ErrorCode}; ErrorDetails={cancellation.ErrorDetails}");
        }

        if (result.Reason == ResultReason.NoMatch)
        {
            var noMatch = NoMatchDetails.FromResult(result);
            throw new InvalidOperationException($"Azure Speech could not recognize the audio. NoMatchReason={noMatch.Reason}");
        }

        if (result.Reason != ResultReason.RecognizedSpeech)
        {
            throw new InvalidOperationException($"Unexpected recognition result reason: {result.Reason}");
        }

        var assessmentResult = PronunciationAssessmentResult.FromResult(result);
        var rawJson = result.Properties.GetProperty(PropertyId.SpeechServiceResponse_JsonResult);

        var allWords = assessmentResult.Words
            .Select(MapWord)
            .ToList();

        return AssessmentReportFactory.Create(
            request.Locale,
            request.AudioPath,
            request.ReferenceText,
            result.Text,
            sessionId,
            result.Reason.ToString(),
            new ScoreSummary
            {
                AccuracyScore = AssessmentReportFactory.Round(assessmentResult.AccuracyScore),
                FluencyScore = AssessmentReportFactory.Round(assessmentResult.FluencyScore),
                CompletenessScore = AssessmentReportFactory.Round(assessmentResult.CompletenessScore),
                ProsodyScore = AssessmentReportFactory.Round(assessmentResult.ProsodyScore),
                PronunciationScore = AssessmentReportFactory.Round(assessmentResult.PronunciationScore)
            },
            allWords,
            request.IncludeRawJson ? rawJson : null);
    }

    private static WordAssessment MapWord(PronunciationAssessmentWordResult word)
    {
        var phonemes = word.Phonemes?
            .Select(static phoneme => new PhonemeAssessment
            {
                Symbol = phoneme.Phoneme,
                AccuracyScore = AssessmentReportFactory.Round(phoneme.AccuracyScore),
                OffsetTicks = phoneme.Offset,
                DurationTicks = phoneme.Duration,
                Alternatives = phoneme.NBestPhonemes?
                    .Select(static candidate => new NBestPhonemeAssessment
                    {
                        Symbol = candidate.Phoneme,
                        Score = AssessmentReportFactory.Round(candidate.Score)
                    })
                    .ToList() ?? []
            })
            .ToList() ?? [];

        var accuracyScore = AssessmentReportFactory.Round(word.AccuracyScore);
        var errorType = string.IsNullOrWhiteSpace(word.ErrorType) ? "None" : word.ErrorType;

        return new WordAssessment
        {
            Word = word.Word,
            AccuracyScore = accuracyScore,
            ErrorType = errorType,
            Feedback = FeedbackEngine.BuildWordFeedback(word.Word, accuracyScore, errorType, phonemes),
            Phonemes = phonemes
        };
    }

    private static string ResolveRequiredValue(string configuredValue, string? environmentValue, string errorMessage)
    {
        var value = !string.IsNullOrWhiteSpace(configuredValue) ? configuredValue : environmentValue;
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new PronunciationConfigurationException(errorMessage);
        }

        return value.Trim();
    }
}
