namespace ClearSpeak.WebApp.Common.Audio;

public sealed class AudioNormalizationException : Exception
{
    public AudioNormalizationException(string message, bool isDependencyFailure = false) : base(message)
    {
        IsDependencyFailure = isDependencyFailure;
    }

    public bool IsDependencyFailure { get; }
}
