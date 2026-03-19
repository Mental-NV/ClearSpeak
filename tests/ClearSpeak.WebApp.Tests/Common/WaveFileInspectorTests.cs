using ClearSpeak.WebApp.Common.Audio;

namespace ClearSpeak.WebApp.Tests.Common;

public sealed class WaveFileInspectorTests
{
    [Fact]
    public void IsCanonical16KhzMonoPcm_ReturnsTrue_ForCanonicalWave()
    {
        using var stream = new MemoryStream(TestAudioFactory.CreateCanonicalWave());

        var result = WaveFileInspector.IsCanonical16KhzMonoPcm(stream);

        Assert.True(result);
    }

    [Fact]
    public void Detect_ReturnsUnknown_ForNonAudioBytes()
    {
        using var stream = new MemoryStream([1, 2, 3, 4, 5]);

        var result = AudioFileFormatDetector.Detect(stream, "sample.bin", "application/octet-stream");

        Assert.Equal(DetectedAudioFormat.Unknown, result);
    }
}
