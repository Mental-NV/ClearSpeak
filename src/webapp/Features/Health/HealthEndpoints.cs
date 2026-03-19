using ClearSpeak.WebApp.Common.Audio;
using Microsoft.Extensions.Options;

namespace ClearSpeak.WebApp.Features.Health;

public static class HealthEndpoints
{
    public static IEndpointRouteBuilder MapHealthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/health", (IOptions<PronunciationOptions> options) => Results.Ok(new
        {
            status = "ok",
            provider = options.Value.Provider,
            locale = options.Value.AllowedLocale
        }));

        return endpoints;
    }
}
