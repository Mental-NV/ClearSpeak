using Microsoft.AspNetCore.Http;

namespace ClearSpeak.WebApp.Tests.Common;

internal static class TestFormFileFactory
{
    public static IFormFile Create(byte[] bytes, string fileName, string contentType)
    {
        var stream = new MemoryStream(bytes);
        return new FormFile(stream, 0, bytes.Length, "audio", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }
}
