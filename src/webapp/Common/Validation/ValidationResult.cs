namespace ClearSpeak.WebApp.Common.Validation;

public sealed class ValidationResult
{
    private ValidationResult(bool isValid, int statusCode, string? field, string? error, string normalizedText, string locale)
    {
        IsValid = isValid;
        StatusCode = statusCode;
        Field = field;
        Error = error;
        NormalizedText = normalizedText;
        Locale = locale;
    }

    public bool IsValid { get; }
    public int StatusCode { get; }
    public string? Field { get; }
    public string? Error { get; }
    public string NormalizedText { get; }
    public string Locale { get; }

    public static ValidationResult Success(string normalizedText, string locale) => new(true, StatusCodes.Status200OK, null, null, normalizedText, locale);

    public static ValidationResult Failure(int statusCode, string field, string error) => new(false, statusCode, field, error, string.Empty, string.Empty);
}
