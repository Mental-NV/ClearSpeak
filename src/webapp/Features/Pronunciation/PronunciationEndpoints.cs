using ClearSpeak.WebApp.Common.Audio;
using ClearSpeak.WebApp.Common.AzureSpeech;
using ClearSpeak.WebApp.Common.Models;
using ClearSpeak.WebApp.Common.Validation;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ClearSpeak.WebApp.Features.Pronunciation;

public static class PronunciationEndpoints
{
    public static IEndpointRouteBuilder MapPronunciationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/pronunciation/analyze", HandleAnalyzeAsync)
            .DisableAntiforgery()
            .Accepts<AnalyzePronunciationRequest>("multipart/form-data")
            .Produces<AnalyzePronunciationResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status413PayloadTooLarge)
            .ProducesProblem(StatusCodes.Status415UnsupportedMediaType)
            .ProducesProblem(StatusCodes.Status503ServiceUnavailable);

        return endpoints;
    }

    private static async Task<Results<Ok<AnalyzePronunciationResponse>, ProblemHttpResult>> HandleAnalyzeAsync(
        [AsParameters] AnalyzePronunciationRequest request,
        PronunciationRequestValidator validator,
        IAudioNormalizer audioNormalizer,
        IPronunciationAssessmentProvider provider,
        ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        var logger = loggerFactory.CreateLogger("PronunciationAnalyze");
        var validation = validator.Validate(request.Audio, request.Text, request.Locale);
        if (!validation.IsValid)
        {
            return TypedResults.Problem(
                statusCode: validation.StatusCode,
                title: "Invalid pronunciation request",
                detail: validation.Error,
                extensions: new Dictionary<string, object?> { ["field"] = validation.Field });
        }

        AudioNormalizationResult? normalizationResult = null;
        try
        {
            normalizationResult = await audioNormalizer.NormalizeAsync(request.Audio!, cancellationToken);

            var report = await provider.AssessAsync(
                new PronunciationAssessmentRequest
                {
                    AudioPath = normalizationResult.NormalizedFilePath,
                    ReferenceText = validation.NormalizedText,
                    Locale = validation.Locale,
                    IncludeRawJson = false
                },
                cancellationToken);

            logger.LogInformation(
                "Pronunciation analysis completed with provider {Provider} and format {Format}.",
                provider.GetType().Name,
                normalizationResult.OriginalFormat);

            return TypedResults.Ok(AnalyzePronunciationResponse.FromReport(report));
        }
        catch (AudioNormalizationException ex)
        {
            logger.LogWarning(ex, "Audio normalization failed.");
            return TypedResults.Problem(
                statusCode: ex.IsDependencyFailure ? StatusCodes.Status503ServiceUnavailable : StatusCodes.Status415UnsupportedMediaType,
                title: ex.IsDependencyFailure ? "Audio normalization dependency unavailable" : "Unsupported audio upload",
                detail: ex.Message);
        }
        catch (PronunciationConfigurationException ex)
        {
            logger.LogWarning(ex, "Pronunciation provider configuration is invalid.");
            return TypedResults.Problem(
                statusCode: StatusCodes.Status503ServiceUnavailable,
                title: "Pronunciation provider is not configured",
                detail: ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Pronunciation analysis failed.");
            return TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Pronunciation analysis failed",
                detail: ex.Message);
        }
        finally
        {
            if (normalizationResult is not null)
            {
                foreach (var tempFile in normalizationResult.TempFiles)
                {
                    try
                    {
                        if (File.Exists(tempFile))
                        {
                            File.Delete(tempFile);
                        }
                    }
                    catch (IOException)
                    {
                    }
                    catch (UnauthorizedAccessException)
                    {
                    }
                }
            }
        }
    }
}
