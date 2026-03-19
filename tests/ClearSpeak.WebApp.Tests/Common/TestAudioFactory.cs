using System.Text;

namespace ClearSpeak.WebApp.Tests.Common;

internal static class TestAudioFactory
{
    public static byte[] CreateCanonicalWave()
    {
        const short channels = 1;
        const int sampleRate = 16_000;
        const short bitsPerSample = 16;
        const short samplesPerTone = 1_600;
        var bytesPerSample = bitsPerSample / 8;
        var blockAlign = channels * bytesPerSample;
        var byteRate = sampleRate * blockAlign;
        var dataSize = samplesPerTone * blockAlign;

        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write(Encoding.ASCII.GetBytes("RIFF"));
        writer.Write(36 + dataSize);
        writer.Write(Encoding.ASCII.GetBytes("WAVE"));
        writer.Write(Encoding.ASCII.GetBytes("fmt "));
        writer.Write(16);
        writer.Write((short)1);
        writer.Write(channels);
        writer.Write(sampleRate);
        writer.Write(byteRate);
        writer.Write((short)blockAlign);
        writer.Write(bitsPerSample);
        writer.Write(Encoding.ASCII.GetBytes("data"));
        writer.Write(dataSize);

        for (var i = 0; i < samplesPerTone; i++)
        {
            writer.Write((short)0);
        }

        writer.Flush();
        return stream.ToArray();
    }
}
