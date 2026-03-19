using System.Net;
using System.Net.Http.Headers;
using ClearSpeak.WebApp.Features.Pronunciation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace ClearSpeak.WebApp.Tests.Features;

public sealed class PronunciationEndpointTests
{
    [Fact]
    public async Task Analyze_ReturnsFeedback_WhenFakeProviderIsConfigured()
    {
        await using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder.UseEnvironment("Development"));

        using var client = factory.CreateClient();
        using var content = BuildMultipartContent("Three friends were walking through the narrow street.");

        var response = await client.PostAsync("/api/pronunciation/analyze", content);

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<AnalyzePronunciationResponse>();

        Assert.NotNull(payload);
        Assert.NotEmpty(payload.Words);
        Assert.NotEmpty(payload.NextSteps);
    }

    [Fact]
    public async Task Analyze_ReturnsBadRequest_WhenAudioIsMissing()
    {
        await using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder.UseEnvironment("Development"));

        using var client = factory.CreateClient();
        using var content = new MultipartFormDataContent
        {
            { new StringContent("hello world"), "text" },
            { new StringContent("en-US"), "locale" }
        };

        var response = await client.PostAsync("/api/pronunciation/analyze", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Analyze_ReturnsConfigurationError_WhenAzureModeHasNoCredentials()
    {
        await using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Production");
                builder.ConfigureAppConfiguration((_, configBuilder) =>
                {
                    configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["Pronunciation:Provider"] = "Azure",
                        ["AzureSpeech:Endpoint"] = "not-a-uri",
                        ["AzureSpeech:Key"] = "fake-key"
                    });
                });
            });

        using var client = factory.CreateClient();
        using var content = BuildMultipartContent("Three friends were walking through the narrow street.");

        var response = await client.PostAsync("/api/pronunciation/analyze", content);
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.Contains("valid absolute URI", body);
    }

    private static MultipartFormDataContent BuildMultipartContent(string text)
    {
        var content = new MultipartFormDataContent
        {
            { new StringContent(text), "text" },
            { new StringContent("en-US"), "locale" }
        };

        var audioContent = new ByteArrayContent(Common.TestAudioFactory.CreateCanonicalWave());
        audioContent.Headers.ContentType = new MediaTypeHeaderValue("audio/wav");
        content.Add(audioContent, "audio", "sample.wav");
        return content;
    }
}
