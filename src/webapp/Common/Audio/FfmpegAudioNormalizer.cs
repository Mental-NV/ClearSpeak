using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ClearSpeak.WebApp.Common.Audio;

public sealed class FfmpegAudioNormalizer : IAudioNormalizer
{
    private readonly PronunciationOptions _options;
    private readonly IProcessRunner _processRunner;
    private readonly ILogger<FfmpegAudioNormalizer> _logger;

    public FfmpegAudioNormalizer(
        IOptions<PronunciationOptions> options,
        IProcessRunner processRunner,
        ILogger<FfmpegAudioNormalizer> logger)
    {
        _options = options.Value;
        _processRunner = processRunner;
        _logger = logger;
    }

    public async Task<AudioNormalizationResult> NormalizeAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);

        var tempFiles = new List<string>();
        var inputExtension = Path.GetExtension(file.FileName);
        var tempInputPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}{inputExtension}");
        tempFiles.Add(tempInputPath);

        await using (var output = File.Create(tempInputPath))
        {
            await file.CopyToAsync(output, cancellationToken);
        }

        await using var inputStream = File.OpenRead(tempInputPath);
        var detectedFormat = AudioFileFormatDetector.Detect(inputStream, file.FileName, file.ContentType);
        if (detectedFormat == DetectedAudioFormat.Unknown)
        {
            throw new AudioNormalizationException("Unsupported audio format. Upload WebM/Opus, MP4/AAC, or WAV.");
        }

        if (detectedFormat == DetectedAudioFormat.Wav && WaveFileInspector.IsCanonical16KhzMonoPcm(inputStream))
        {
            _logger.LogInformation("Skipping transcoding for canonical WAV upload {FileName}.", file.FileName);
            return new AudioNormalizationResult
            {
                NormalizedFilePath = tempInputPath,
                OriginalFormat = detectedFormat,
                WasConverted = false,
                TempFiles = tempFiles
            };
        }

        var normalizedOutputPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.wav");
        tempFiles.Add(normalizedOutputPath);

        var result = await _processRunner.RunAsync(
            _options.FfmpegPath,
            ["-y", "-i", tempInputPath, "-ac", "1", "-ar", "16000", "-c:a", "pcm_s16le", normalizedOutputPath],
            cancellationToken);

        if (result.ExitCode != 0 || !File.Exists(normalizedOutputPath))
        {
            throw new AudioNormalizationException($"Audio normalization failed. {result.StandardError}".Trim());
        }

        await using var normalizedStream = File.OpenRead(normalizedOutputPath);
        if (!WaveFileInspector.IsCanonical16KhzMonoPcm(normalizedStream))
        {
            throw new AudioNormalizationException("Audio normalization did not produce a mono 16-bit PCM 16 kHz WAV file.");
        }

        return new AudioNormalizationResult
        {
            NormalizedFilePath = normalizedOutputPath,
            OriginalFormat = detectedFormat,
            WasConverted = true,
            TempFiles = tempFiles
        };
    }
}
