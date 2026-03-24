# ClearSpeak v1.2 Prerequisites Implementation Plan

## Summary

This plan defines the prerequisite foundation for ClearSpeak v1.2 before feature work on passage history, compare, and progress proceeds.

The prerequisites are locked to these decisions:

- persistence uses SQLite with EF Core
- user identity uses Google Authentication
- backend APIs use JWT Bearer authentication
- local development runs on HTTPS with a cross-platform self-signed certificate workflow

This plan is decision-complete. An implementer should not need to make additional architecture choices for persistence, identity, or local HTTPS.

## Success Criteria

The prerequisite layer is complete when:

- the backend starts with EF Core + SQLite and applies migrations automatically in development
- a signed-in Google user maps to a stable local ClearSpeak user record
- authenticated API calls use `Authorization: Bearer <token>`
- all v1.2 learner data is persisted under a local user id
- local development works over `https://localhost:7247`
- first-time local setup generates, stores, and trusts a self-signed certificate on macOS, Windows, and Linux without committing secrets or cert files to git

## Locked Decisions

### 1. Persistence

- Use `Microsoft.EntityFrameworkCore.Sqlite` as the primary database provider.
- Use EF Core code-first migrations committed to the repo.
- Use a single application `DbContext` named `ClearSpeakDbContext`.
- Use SQLite for both local development and v1.2 deployment targets.
- Use one relational schema for auth, passages, attempts, and refresh sessions.

### 2. Identity And Auth

- Use Google OAuth 2.0 for user sign-in.
- Use Google Identity Services in the SPA with redirect-based authorization code flow.
- Exchange the Google authorization code on the backend.
- Validate Google identity on the backend, then issue first-party ClearSpeak JWT access tokens.
- Do not use cookie authentication for API authorization.
- Use short-lived JWT access tokens plus persistent refresh tokens.
- Persist refresh tokens server-side as hashes only.
- Require authentication for learner-specific APIs.

### 3. Local HTTPS

- Keep local backend origin on `https://localhost:7247`.
- Generate a self-signed localhost certificate outside the repo.
- Store the certificate under a platform-specific user-local application data directory.
- Automate certificate trust installation for macOS, Windows, and Linux.
- Keep certificate password out of committed config.
- Load the certificate into Kestrel through environment variables.

## Implementation Changes

### Backend Foundation

- Add EF Core packages:
  - `Microsoft.EntityFrameworkCore.Sqlite`
  - `Microsoft.EntityFrameworkCore.Design`
  - `Microsoft.AspNetCore.Authentication.JwtBearer`
- Add a new data area under `src/webapp/Data/`.
- Add `ClearSpeakDbContext`.
- Add EF Core migrations under `src/webapp/Data/Migrations/`.
- Add a startup extension such as `AddClearSpeakData()` and `UseClearSpeakData()` to keep `Program.cs` small.
- Enable automatic migration application in Development only.
- In non-development environments, fail startup if the schema is behind migrations rather than applying them implicitly.

### Runtime Database Configuration

- Add connection string key `ConnectionStrings:ClearSpeak`.
- Development default:
  - `Data Source=App_Data/clearspeak.dev.db`
- Production default:
  - no committed default path
  - require `ConnectionStrings__ClearSpeak` from environment/App Service settings
- Create `src/webapp/App_Data/` at runtime if missing.
- Add `src/webapp/App_Data/` to `.gitignore`.

### Domain Tables

Implement these tables first. They are the minimum schema required for v1.2 persistence and identity.

#### `Users`

- `Id` `TEXT` / GUID primary key
- `AuthProvider` `TEXT` fixed to `Google` in v1.2
- `ProviderSubject` `TEXT` unique, Google `sub`
- `Email` `TEXT`
- `EmailVerified` `INTEGER`
- `DisplayName` `TEXT`
- `GivenName` `TEXT` nullable
- `FamilyName` `TEXT` nullable
- `AvatarUrl` `TEXT` nullable
- `CreatedAtUtc` `TEXT`
- `LastLoginAtUtc` `TEXT`

Constraints:

- unique index on `ProviderSubject`
- index on `Email`

#### `RefreshSessions`

- `Id` `TEXT` / GUID primary key
- `UserId` FK to `Users`
- `TokenHash` `TEXT`
- `CreatedAtUtc` `TEXT`
- `ExpiresAtUtc` `TEXT`
- `RevokedAtUtc` `TEXT` nullable
- `ReplacedBySessionId` `TEXT` nullable
- `UserAgent` `TEXT` nullable
- `IpAddress` `TEXT` nullable

Constraints:

- unique index on `TokenHash`
- index on `UserId`

#### `Passages`

- `Id` `TEXT` primary key, stable PRD passage id
- `Title` `TEXT`
- `Text` `TEXT`
- `Difficulty` `TEXT`
- `EstimatedDurationSeconds` `INTEGER`
- `FocusTagsJson` `TEXT`
- `TopicTagsJson` `TEXT`
- `IsFeatured` `INTEGER`
- `IsActive` `INTEGER`
- `SortOrder` `INTEGER`
- `Description` `TEXT` nullable
- `RecommendedFor` `TEXT` nullable
- `CreatedAtUtc` `TEXT`
- `UpdatedAtUtc` `TEXT`

Decision:

- Store tags as JSON arrays in SQLite for v1.2.
- Do not normalize tags into join tables yet.
- The current 36-passage seed catalog is small enough that JSON-tag filtering is acceptable for v1.2.

#### `Attempts`

- `Id` `TEXT` / GUID primary key
- `UserId` FK to `Users`
- `PassageId` FK to `Passages`, nullable for custom text
- `AttemptType` `TEXT` enum values `Passage` or `CustomText`
- `PassageTitleSnapshot` `TEXT` nullable
- `ReferenceTextSnapshot` `TEXT`
- `RecognizedTextSnapshot` `TEXT` nullable
- `Locale` `TEXT`
- `PronunciationScore` `REAL`
- `AccuracyScore` `REAL`
- `FluencyScore` `REAL`
- `CompletenessScore` `REAL`
- `ProsodyScore` `REAL`
- `Summary` `TEXT`
- `NextStepsJson` `TEXT`
- `CreatedAtUtc` `TEXT`

Constraints:

- index on `UserId, CreatedAtUtc DESC`
- index on `UserId, PassageId, CreatedAtUtc DESC`
- index on `AttemptType`

#### `AttemptWordResults`

- `Id` `TEXT` / GUID primary key
- `AttemptId` FK to `Attempts`
- `WordText` `TEXT`
- `WordOrder` `INTEGER`
- `Score` `REAL`
- `ErrorType` `TEXT`
- `Feedback` `TEXT`
- `IsFocusWord` `INTEGER`
- `OffsetTicks` `INTEGER` nullable
- `DurationTicks` `INTEGER` nullable
- `PhonemesJson` `TEXT`

Constraints:

- index on `AttemptId, WordOrder`

Decision:

- Store phoneme details as JSON on each word row for v1.2.
- Do not create a separate phoneme table yet.

### Seeding Strategy

- Move the runtime passage seed source out of `docs/`.
- Create `src/webapp/Data/Seed/passages.v1.2.json`.
- Keep `docs/clear_speak_v1_2_passage_seed_catalog_all_levels.json` as the planning/reference artifact.
- Add a `PassageCatalogSeeder` that:
  - inserts missing passages
  - updates non-breaking metadata on existing rows
  - never rewrites attempt snapshots
- Seed passages at startup after migrations.
- Inactive passages remain in the database and are excluded from normal browse queries.

### Auth Architecture

#### Google Sign-In Flow

Use this exact flow:

1. SPA loads Google Identity Services.
2. User clicks `Sign in with Google`.
3. SPA redirects the browser to Google.
4. Google redirects back to `https://localhost:7247/auth/callback` with an authorization code.
5. SPA callback page extracts the code and posts it to `POST /api/auth/google/exchange`.
6. Backend exchanges the code with Google using the app client secret.
7. Backend validates the returned Google identity payload.
8. Backend upserts the local `Users` row using Google `sub`.
9. Backend creates a signed ClearSpeak access token and a refresh session.
10. Backend returns auth payload JSON to the SPA.

#### First-Party Token Model

- Access token:
  - signed by ClearSpeak backend
  - JWT Bearer
  - lifetime: 15 minutes
- Refresh token:
  - opaque random 64-byte token
  - lifetime: 30 days
  - stored hashed in `RefreshSessions`
  - rotated on each refresh

#### Access Token Claims

- `sub`: local `Users.Id`
- `email`
- `name`
- `picture` if available
- `provider`: `google`
- `ver`: `1`

#### Frontend Token Storage

- Store access token in memory first.
- Mirror access token to `sessionStorage` to survive page refresh in the current browser session.
- Store refresh token in `localStorage`.
- Clear both on logout.

Decision note:

- This accepts the standard XSS tradeoff of bearer-token storage because JWT bearer is explicitly preferred over cookies.
- No http-only cookie fallback will be added in v1.2 prerequisites.

### Auth Endpoints

Implement these endpoints under `src/webapp/Features/Auth/`:

- `POST /api/auth/google/exchange`
  - request: `{ code: string, redirectUri: string }`
  - response:
    - `accessToken`
    - `accessTokenExpiresAtUtc`
    - `refreshToken`
    - `refreshTokenExpiresAtUtc`
    - `user`
- `POST /api/auth/refresh`
  - request: `{ refreshToken: string }`
  - response: same shape as exchange
- `POST /api/auth/logout`
  - request: `{ refreshToken: string }`
  - response: `204 No Content`
- `GET /api/me`
  - response:
    - `id`
    - `email`
    - `displayName`
    - `avatarUrl`

### API Authorization Rules

- Anonymous:
  - `GET /api/health`
  - `GET /api/passages`
  - `GET /api/passages/{id}`
  - `POST /api/auth/google/exchange`
  - `POST /api/auth/refresh`
- Authenticated:
  - `POST /api/pronunciation/analyze`
  - `GET /api/attempts`
  - `GET /api/attempts/{id}`
  - `GET /api/attempts/{id}/compare`
  - `GET /api/progress`
  - `GET /api/progress/passages`
  - `GET /api/me`
  - `POST /api/auth/logout`

Decision:

- `POST /api/pronunciation/analyze` becomes authenticated because a successful analysis must create a saved attempt tied to a user.
- Practice browsing remains available before sign-in.

### Pronunciation Pipeline Persistence Hook

- Extend the analyze request contract to accept optional `passageId`.
- On successful pronunciation analysis:
  - map provider output to response DTO as today
  - persist the `Attempts` row
  - persist `AttemptWordResults`
  - return the same analysis payload plus `attemptId`
- On failed analysis:
  - do not write learner-visible attempt rows
- Attempt snapshots are immutable once written.

### Frontend Auth Foundation

- Add a new auth area under `src/clientspa/src/features/auth/`.
- Add an auth provider/context responsible for:
  - bootstrapping the current session from `sessionStorage` and `localStorage`
  - refreshing access tokens
  - exposing `signIn`, `signOut`, `isAuthenticated`, `user`
- Add a dedicated SPA callback route at `/auth/callback` that completes the code exchange.
- Add an API client wrapper that automatically adds bearer tokens.
- Treat `401` as:
  - one refresh attempt
  - if refresh fails, sign out and route to sign-in prompt

### Local HTTPS Plan

#### Certificate Storage

Store development cert files outside the repo in a platform-specific user-local directory:

- macOS:
  - `~/.clearspeak/dev-certs/`
- Linux:
  - `~/.clearspeak/dev-certs/`
- Windows:
  - `%LOCALAPPDATA%\ClearSpeak\dev-certs\`
- files:
  - `clearspeak-localhost.crt`
  - `clearspeak-localhost.key`
  - `clearspeak-localhost.pfx`
  - `clearspeak-localhost.password`
  - `metadata.json`

Decision:

- The runtime implementation should resolve this directory through a single helper that maps:
  - Windows to `%LOCALAPPDATA%\ClearSpeak\dev-certs\`
  - macOS and Linux to `$HOME/.clearspeak/dev-certs/`

#### Certificate Shape

- Common Name: `localhost`
- SANs:
  - `DNS:localhost`
  - `IP:127.0.0.1`
  - `IP:::1`
- validity: 365 days
- key size: RSA 2048

#### Dev Scripts

Add these cross-platform scripts under `scripts/dev/`:

- `setup-local-https.mjs`
  - create cert directory if missing
  - generate a new self-signed cert if missing or expiring within 30 days
  - export `.crt`, `.key`, and `.pfx`
  - write random PFX password to `clearspeak-localhost.password`
  - install trust into the platform trust store
  - write `metadata.json` with thumbprint and expiry
- `run-web-https.mjs`
  - resolve the platform-specific certificate directory
  - read `clearspeak-localhost.password`
  - set:
    - `ASPNETCORE_Kestrel__Certificates__Default__Path`
    - `ASPNETCORE_Kestrel__Certificates__Default__Password`
  - spawn `dotnet run --project src/webapp/ClearSpeak.WebApp.csproj --launch-profile https`

Add thin convenience wrappers:

- `setup-local-https.sh`
  - invokes `node ./scripts/dev/setup-local-https.mjs`
- `run-web-https.sh`
  - invokes `node ./scripts/dev/run-web-https.mjs`
- `setup-local-https.cmd`
  - invokes `node scripts\\dev\\setup-local-https.mjs`
- `run-web-https.cmd`
  - invokes `node scripts\\dev\\run-web-https.mjs`

Add npm scripts at the repo root:

- `npm run setup:https`
  - runs `node ./scripts/dev/setup-local-https.mjs`
- `npm run dev:https`
  - runs `node ./scripts/dev/run-web-https.mjs`

Decision:

- Node.js is the canonical implementation entrypoint because it is already a required dependency for the frontend and is simpler to standardize across macOS, Windows, and Linux.
- The Node scripts are the single source of truth for HTTPS setup and launch behavior.
- The shell and cmd files are wrappers only and must not duplicate logic.

#### Trust Installation Rules

Implement trust installation with these OS-specific behaviors:

- macOS:
  - import `clearspeak-localhost.crt` into the login keychain
  - mark it trusted for SSL
  - use `security add-trusted-cert`
- Windows:
  - import `clearspeak-localhost.pfx` or `.crt` into `CurrentUser\\Root`
  - use `Import-PfxCertificate` or `Import-Certificate`
- Linux:
  - install to the user-accessible system CA path when possible
  - support Debian/Ubuntu flow with `/usr/local/share/ca-certificates/` plus `update-ca-certificates`
  - support Fedora/RHEL flow with `/etc/pki/ca-trust/source/anchors/` plus `update-ca-trust`
  - if automatic trust cannot run because of distro mismatch or missing elevation, print exact manual commands and still complete certificate generation

Decision:

- Linux trust is best-effort automated with clear manual fallback because certificate trust commands vary by distribution.
- The app must still run with the generated cert even if Linux trust installation is deferred manually.

#### Kestrel Integration

- Keep `https://localhost:7247` in `launchSettings.json`.
- Do not commit certificate path or password into `appsettings.*.json`.
- Read the default Kestrel certificate entirely from environment variables at runtime.

#### Local Auth Registration

Document this exact Google OAuth local setup:

- Google OAuth application type: Web application
- Authorized JavaScript origin:
  - `https://localhost:7247`
- Authorized redirect URI:
  - `https://localhost:7247/auth/callback`

Decision:

- Use redirect-based authorization code flow with a dedicated SPA callback route.
- The callback route is part of the application shell and must exist in both local and deployed environments.

### Secrets And Config

Add these config keys:

- `ConnectionStrings:ClearSpeak`
- `Authentication:Google:ClientId`
- `Authentication:Google:ClientSecret`
- `Authentication:Jwt:Issuer`
- `Authentication:Jwt:Audience`
- `Authentication:Jwt:SigningKey`
- `Authentication:Jwt:AccessTokenMinutes`
- `Authentication:Jwt:RefreshTokenDays`

Development secret storage:

- use ASP.NET user secrets for backend secrets
- do not commit Google client secrets
- do not commit JWT signing key
- do not commit local certificate password
- do not commit generated local certificate files

SPA runtime config:

- expose only non-secret values to the frontend
- frontend-visible values:
  - Google client id
  - API base URL if needed

### Expected File And Module Additions

The implementation should add or modify these areas:

- `src/webapp/Program.cs`
- `src/webapp/ClearSpeak.WebApp.csproj`
- `src/webapp/Data/`
- `src/webapp/Features/Auth/`
- `src/webapp/Features/Passages/`
- `src/webapp/Common/Security/`
- `src/clientspa/src/features/auth/`
- `src/clientspa/src/lib/api.ts`
- `src/webapp/Properties/launchSettings.json`
- `README.md`
- `.gitignore`
- `scripts/dev/`
- `package.json`

## Delivery Order

Implement in this exact order:

1. Add EF Core, SQLite packages, `DbContext`, base entities, and initial migration.
2. Add passage seed import from `src/webapp/Data/Seed/passages.v1.2.json`.
3. Add Google auth backend exchange flow and first-party JWT issuance.
4. Add refresh-token persistence and refresh/logout endpoints.
5. Add frontend auth provider and bearer-aware API client.
6. Convert `POST /api/pronunciation/analyze` to authenticated, save-on-success behavior.
7. Add local HTTPS scripts, Kestrel environment binding, and README setup.
8. Add integration tests for auth, persistence, and HTTPS bootstrap expectations.

## Test Plan

### Backend Tests

- migration boots a fresh SQLite database successfully
- passage seeder inserts the 36 initial passages exactly once
- passage seeder is idempotent
- Google code exchange rejects invalid code
- Google identity upserts a new local user
- repeat login reuses the same user via `ProviderSubject`
- access tokens validate through JWT bearer middleware
- refresh rotates the refresh token and revokes the previous session
- logout revokes the refresh session
- authenticated analyze writes one `Attempts` row and ordered `AttemptWordResults`
- failed analyze writes no attempt row

### Frontend Tests

- auth provider restores session from storage
- API client sends bearer token when present
- API client refreshes once on `401`
- logout clears in-memory and browser-stored auth state
- analyze request includes `passageId` when practicing from library

### Manual Verification

- macOS: first-time `./scripts/dev/setup-local-https.sh` creates and trusts a cert
- Linux: first-time `./scripts/dev/setup-local-https.sh` creates a cert and either trusts it automatically or prints exact manual trust commands
- Windows: first-time `scripts\\dev\\setup-local-https.cmd` creates and trusts a cert
- `npm run dev:https`, `./scripts/dev/run-web-https.sh`, and `scripts\\dev\\run-web-https.cmd` serve the app on `https://localhost:7247`
- browser shows a trusted local HTTPS connection
- Google sign-in succeeds locally against the HTTPS origin
- signed-in user can analyze audio and see saved data tied to their account

## Assumptions And Defaults

- v1.2 targets one primary deployment environment and a single SQLite database file is acceptable for that scope.
- Node.js is available for local development on macOS, Windows, and Linux.
- Cookie auth is intentionally out of scope for this prerequisite layer.
- SQLite JSON text storage is sufficient for v1.2 filtering and aggregation scale.
- Role-based authorization is out of scope for v1.2.
- Audio files themselves are not retained as part of this prerequisite plan.
