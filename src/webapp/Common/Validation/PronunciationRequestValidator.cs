using ClearSpeak.WebApp.Common.Audio;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ClearSpeak.WebApp.Common.Validation;

public sealed class PronunciationRequestValidator
{
    private readonly PronunciationOptions _options;

    public PronunciationRequestValidator(IOptions<PronunciationOptions> options)
    {
        _options = options.Value;
    }

    public ValidationResult Validate(IFormFile? audio, string? text, string? locale)
    {
        if (audio is null || audio.Length <= 0)
        {
            return ValidationResult.Failure(StatusCodes.Status400BadRequest, "audio", "Audio file is required.");
        }

        if (audio.Length > _options.MaxUploadBytes)
        {
            return ValidationResult.Failure(StatusCodes.Status413PayloadTooLarge, "audio", $"Audio file must be {(_options.MaxUploadBytes / (1024 * 1024)):0} MB or smaller.");
        }

        var normalizedText = NormalizeWhitespace(text);
        if (string.IsNullOrWhiteSpace(normalizedText))
        {
            return ValidationResult.Failure(StatusCodes.Status400BadRequest, "text", "Reference text is required.");
        }

        var requestedLocale = string.IsNullOrWhiteSpace(locale) ? _options.AllowedLocale : locale.Trim();
        if (!string.Equals(requestedLocale, _options.AllowedLocale, StringComparison.OrdinalIgnoreCase))
        {
            return ValidationResult.Failure(StatusCodes.Status400BadRequest, "locale", $"This MVP only supports {_options.AllowedLocale}.");
        }

        return ValidationResult.Success(normalizedText, _options.AllowedLocale);
    }

    private static string NormalizeWhitespace(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return string.Join(" ", value.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
    }
}
