# ClearSpeak

ClearSpeak is a pronunciation-checking MVP for US English learners. The web app records speech in the browser, uploads audio plus the reference text to an ASP.NET Core backend, normalizes the audio to 16 kHz mono PCM WAV, and returns pronunciation feedback from Azure Speech Services or a deterministic fake provider.

The backend intentionally reuses the original CLI pronunciation engine instead of rewriting it from scratch. The extracted pieces now live under `src/webapp/Common/`:

- `Common/AzureSpeech/AzureSpeechPronunciationAssessmentProvider.cs`
- `Common/Models/AssessmentModels.cs`
- `Common/Feedback/FeedbackEngine.cs`

## Project structure

```text
src/
  clientspa/   React + TypeScript + Vite single-page app
  webapp/      ASP.NET Core Minimal API, static hosting, pronunciation pipeline
tests/
  ClearSpeak.WebApp.Tests/   xUnit backend tests
```

## Prerequisites

- .NET SDK 10
- Node.js 24+ and npm
- `ffmpeg` on `PATH` for WebM/MP4 uploads and non-canonical WAV conversion

## Configuration

The app supports `en-US` only for this MVP.

Azure configuration can come from `appsettings.json` / `appsettings.Development.json` or environment variables:

- `AZURE_SPEECH_ENDPOINT`
- `AZURE_SPEECH_KEY`

Development defaults to the fake provider:

```json
"Pronunciation": {
  "Provider": "Fake",
  "AllowedLocale": "en-US",
  "MaxUploadBytes": 10485760,
  "FfmpegPath": "ffmpeg"
}
```

Set `Pronunciation:Provider` to `Azure` for live Azure Speech analysis.

## Local development

Install frontend dependencies:

```bash
npm --prefix src/clientspa install
```

Run frontend dev server:

```bash
npm --prefix src/clientspa run dev
```

Run backend API:

```bash
dotnet run --project src/webapp/ClearSpeak.WebApp.csproj
```

Build the SPA into `src/clientspa/dist`:

```bash
npm --prefix src/clientspa run build
```

Build the backend and copy built frontend assets into `src/webapp/wwwroot`:

```bash
dotnet build src/webapp/ClearSpeak.WebApp.csproj
```

Run the built app with static frontend hosting:

```bash
dotnet run --project src/webapp/ClearSpeak.WebApp.csproj
```

Repo-level shortcuts:

```bash
npm run build
npm run dev:web
npm run test:web
npm run test:spa
```

## CI/CD

GitHub Actions runs build, test, and publish on:

- pull requests
- all pushes
- manual `workflow_dispatch`

The workflow uploads a deployable artifact named `webapp-publish` on every run. Pushes to `master` also deploy that artifact to Azure App Service.

Required GitHub configuration:

- Repository secret: `AZURE_WEBAPP_PUBLISH_PROFILE`

Non-`master` pushes and pull requests build, test, and publish only. `master` pushes build, test, publish, and deploy to Azure App Service.

## Supported uploads

- `audio/webm;codecs=opus`
- `audio/mp4`
- `audio/wav`

All uploads are normalized to mono 16-bit PCM WAV at 16 kHz before pronunciation assessment.

## Browser support

- Desktop-first
- Chrome / Edge / Firefox are the primary target
- Mobile Safari is best-effort

## Testing

Backend:

```bash
dotnet test tests/ClearSpeak.WebApp.Tests/ClearSpeak.WebApp.Tests.csproj
```

Frontend:

```bash
npm --prefix src/clientspa run test
```

## Troubleshooting

- Microphone permission denied:
  Allow microphone access in the browser, then re-record.
- `ffmpeg` missing:
  Canonical WAV uploads can still work, but WebM/MP4 uploads and non-canonical WAV uploads will return a dependency error until `ffmpeg` is installed or `Pronunciation:FfmpegPath` points to a valid binary.
- Azure provider configuration error:
  Set `AZURE_SPEECH_ENDPOINT` and `AZURE_SPEECH_KEY`, or switch `Pronunciation:Provider` back to `Fake` for local development.
