using Microsoft.AspNetCore.Mvc;

namespace ClearSpeak.WebApp.Features.Pronunciation;

public sealed class AnalyzePronunciationRequest
{
    [FromForm(Name = "audio")]
    public IFormFile? Audio { get; init; }

    [FromForm(Name = "text")]
    public string? Text { get; init; }

    [FromForm(Name = "locale")]
    public string? Locale { get; init; }
}
