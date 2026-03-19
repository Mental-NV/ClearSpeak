namespace ClearSpeak.WebApp.Common.Audio;

public static class WaveFileInspector
{
    public static bool IsCanonical16KhzMonoPcm(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        if (!stream.CanSeek)
        {
            return false;
        }

        var originalPosition = stream.Position;

        try
        {
            using var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true);
            if (reader.ReadUInt32() != 0x46464952)
            {
                return false;
            }

            _ = reader.ReadUInt32();
            if (reader.ReadUInt32() != 0x45564157)
            {
                return false;
            }

            while (stream.Position + 8 <= stream.Length)
            {
                var chunkId = reader.ReadUInt32();
                var chunkSize = reader.ReadUInt32();

                if (chunkId == 0x20746D66)
                {
                    var audioFormat = reader.ReadUInt16();
                    var channels = reader.ReadUInt16();
                    var sampleRate = reader.ReadUInt32();
                    _ = reader.ReadUInt32();
                    _ = reader.ReadUInt16();
                    var bitsPerSample = reader.ReadUInt16();

                    return audioFormat == 1 &&
                           channels == 1 &&
                           sampleRate == 16_000 &&
                           bitsPerSample == 16;
                }

                stream.Seek(chunkSize + (chunkSize % 2), SeekOrigin.Current);
            }

            return false;
        }
        finally
        {
            stream.Position = originalPosition;
        }
    }
}
