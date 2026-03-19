using ClearSpeak.WebApp.Common.Audio;
using ClearSpeak.WebApp.Common.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ClearSpeak.WebApp.Tests.Common;

public sealed class PronunciationRequestValidatorTests
{
    private readonly PronunciationRequestValidator _validator = new(Options.Create(new PronunciationOptions()));

    [Fact]
    public void Validate_ReturnsError_WhenTextIsMissing()
    {
        var file = TestFormFileFactory.Create(TestAudioFactory.CreateCanonicalWave(), "sample.wav", "audio/wav");

        var result = _validator.Validate(file, "   ", "en-US");

        Assert.False(result.IsValid);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        Assert.Equal("text", result.Field);
    }

    [Fact]
    public void Validate_ReturnsError_WhenLocaleIsNotSupported()
    {
        var file = TestFormFileFactory.Create(TestAudioFactory.CreateCanonicalWave(), "sample.wav", "audio/wav");

        var result = _validator.Validate(file, "hello world", "en-GB");

        Assert.False(result.IsValid);
        Assert.Equal("locale", result.Field);
    }

    [Fact]
    public void Validate_NormalizesWhitespace_WhenRequestIsValid()
    {
        var file = TestFormFileFactory.Create(TestAudioFactory.CreateCanonicalWave(), "sample.wav", "audio/wav");

        var result = _validator.Validate(file, "hello   there \n world", "en-US");

        Assert.True(result.IsValid);
        Assert.Equal("hello there world", result.NormalizedText);
        Assert.Equal("en-US", result.Locale);
    }
}
