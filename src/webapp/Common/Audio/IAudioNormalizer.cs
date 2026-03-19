using Microsoft.AspNetCore.Http;

namespace ClearSpeak.WebApp.Common.Audio;

public interface IAudioNormalizer
{
    Task<AudioNormalizationResult> NormalizeAsync(IFormFile file, CancellationToken cancellationToken = default);
}
