namespace ClearSpeak.WebApp.Common.Audio;

public static class AudioFileFormatDetector
{
    public static DetectedAudioFormat Detect(Stream stream, string? fileName, string? contentType)
    {
        ArgumentNullException.ThrowIfNull(stream);

        var header = new byte[16];
        var bytesRead = stream.Read(header, 0, header.Length);
        stream.Position = 0;

        if (bytesRead >= 12 &&
            header[0] == 'R' &&
            header[1] == 'I' &&
            header[2] == 'F' &&
            header[3] == 'F' &&
            header[8] == 'W' &&
            header[9] == 'A' &&
            header[10] == 'V' &&
            header[11] == 'E')
        {
            return DetectedAudioFormat.Wav;
        }

        if (bytesRead >= 4 &&
            header[0] == 0x1A &&
            header[1] == 0x45 &&
            header[2] == 0xDF &&
            header[3] == 0xA3)
        {
            return DetectedAudioFormat.WebM;
        }

        if (bytesRead >= 8 &&
            header[4] == 'f' &&
            header[5] == 't' &&
            header[6] == 'y' &&
            header[7] == 'p')
        {
            return DetectedAudioFormat.Mp4;
        }

        var extension = Path.GetExtension(fileName ?? string.Empty);
        if (string.Equals(extension, ".wav", StringComparison.OrdinalIgnoreCase))
        {
            return DetectedAudioFormat.Wav;
        }

        if (string.Equals(extension, ".webm", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(contentType, "audio/webm", StringComparison.OrdinalIgnoreCase))
        {
            return DetectedAudioFormat.WebM;
        }

        if (string.Equals(extension, ".mp4", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(extension, ".m4a", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(contentType, "audio/mp4", StringComparison.OrdinalIgnoreCase))
        {
            return DetectedAudioFormat.Mp4;
        }

        return DetectedAudioFormat.Unknown;
    }
}
