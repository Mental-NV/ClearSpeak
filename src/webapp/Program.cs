using ClearSpeak.WebApp.Common.Audio;
using ClearSpeak.WebApp.Common.AzureSpeech;
using ClearSpeak.WebApp.Common.Validation;
using ClearSpeak.WebApp.Features.Frontend;
using ClearSpeak.WebApp.Features.Health;
using ClearSpeak.WebApp.Features.Pronunciation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.Configure<AzureSpeechOptions>(builder.Configuration.GetSection(AzureSpeechOptions.SectionName));
builder.Services.Configure<PronunciationOptions>(builder.Configuration.GetSection(PronunciationOptions.SectionName));

builder.Services.AddSingleton<PronunciationRequestValidator>();
builder.Services.AddSingleton<IProcessRunner, ProcessRunner>();
builder.Services.AddSingleton<IAudioNormalizer, FfmpegAudioNormalizer>();
builder.Services.AddSingleton<IPronunciationAssessmentProvider>(static serviceProvider =>
{
    var options = serviceProvider.GetRequiredService<IConfiguration>()
        .GetSection(PronunciationOptions.SectionName)
        .Get<PronunciationOptions>() ?? new PronunciationOptions();

    return string.Equals(options.Provider, PronunciationProviderNames.Fake, StringComparison.OrdinalIgnoreCase)
        ? ActivatorUtilities.CreateInstance<FakePronunciationAssessmentProvider>(serviceProvider)
        : ActivatorUtilities.CreateInstance<AzureSpeechPronunciationAssessmentProvider>(serviceProvider);
});

var app = builder.Build();

app.UseExceptionHandler();
app.UseClearSpeakFrontend();

app.MapHealthEndpoints();
app.MapPronunciationEndpoints();
app.MapFrontendFallback();

app.Run();

public partial class Program;
