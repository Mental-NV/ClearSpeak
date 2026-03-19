using ClearSpeak.WebApp.Common.Audio;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace ClearSpeak.WebApp.Tests.Common;

public sealed class FfmpegAudioNormalizerTests
{
    [Fact]
    public async Task NormalizeAsync_BypassesRunner_ForCanonicalWave()
    {
        var file = TestFormFileFactory.Create(TestAudioFactory.CreateCanonicalWave(), "sample.wav", "audio/wav");
        var runner = new RecordingProcessRunner();
        var normalizer = new FfmpegAudioNormalizer(
            Options.Create(new PronunciationOptions()),
            runner,
            NullLogger<FfmpegAudioNormalizer>.Instance);

        var result = await normalizer.NormalizeAsync(file);

        Assert.False(result.WasConverted);
        Assert.False(runner.WasCalled);

        foreach (var tempFile in result.TempFiles.Where(File.Exists))
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public async Task NormalizeAsync_RejectsUnknownFormat()
    {
        var file = TestFormFileFactory.Create([1, 2, 3], "sample.txt", "text/plain");
        var normalizer = new FfmpegAudioNormalizer(
            Options.Create(new PronunciationOptions()),
            new RecordingProcessRunner(),
            NullLogger<FfmpegAudioNormalizer>.Instance);

        await Assert.ThrowsAsync<AudioNormalizationException>(() => normalizer.NormalizeAsync(file));
    }

    private sealed class RecordingProcessRunner : IProcessRunner
    {
        public bool WasCalled { get; private set; }

        public Task<ProcessResult> RunAsync(string fileName, IReadOnlyList<string> arguments, CancellationToken cancellationToken = default)
        {
            WasCalled = true;
            return Task.FromResult(new ProcessResult
            {
                ExitCode = 0,
                StandardOutput = string.Empty,
                StandardError = string.Empty
            });
        }
    }
}
