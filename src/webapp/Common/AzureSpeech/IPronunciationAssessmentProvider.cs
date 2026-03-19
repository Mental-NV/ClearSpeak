using ClearSpeak.WebApp.Common.Models;

namespace ClearSpeak.WebApp.Common.AzureSpeech;

public interface IPronunciationAssessmentProvider
{
    Task<AssessmentReport> AssessAsync(PronunciationAssessmentRequest request, CancellationToken cancellationToken = default);
}
