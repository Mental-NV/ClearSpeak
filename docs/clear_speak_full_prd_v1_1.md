# ClearSpeak — Product Requirements Document (PRD)

- **Product name:** ClearSpeak
- **Document type:** Full PRD
- **Version:** 1.1
- **Status:** Draft for implementation
- **Primary platform:** Web application
- **Primary locale:** US English (`en-US`)
- **Core capability:** User reads a known text aloud; system evaluates pronunciation and returns actionable feedback

---

## 1. Executive summary

ClearSpeak is a lightweight web application for English learners who want to improve their **US English pronunciation** through guided reading practice. The user edits or pastes a passage into a text box, records themselves reading it aloud, submits the recording to the backend, and receives immediate feedback on pronunciation quality.

The product is intentionally narrow in scope. It does **not** include user accounts, exercise libraries, history, persistence, progress dashboards, or course structures in the MVP. It is a single-session pronunciation checker optimized for speed, clarity, and low implementation overhead.

The backend is based on the existing Azure Speech pronunciation-assessment pipeline. The frontend’s responsibility is to capture text and audio, submit them to the backend, and render the returned feedback.

---

## 2. Problem statement

English learners often know that their pronunciation needs improvement but do not know:
- which exact words are problematic,
- whether the issue is clarity, rhythm, completeness, or specific sounds,
- what to practice next.

Most tools either provide overly generic scores or require larger ecosystems such as courses, accounts, or lesson libraries. ClearSpeak solves the narrower problem of **quick, precise pronunciation checking for a custom text passage**.

---

## 3. Product vision

Provide the fastest possible path from **text + speech** to **specific pronunciation feedback**.

ClearSpeak should feel like a focused tool rather than a learning platform:
- open the page,
- adjust the text,
- record,
- analyze,
- understand what to fix.

---

## 4. Goals and non-goals

### 4.1 Goals

1. Allow a learner to input or edit reading text directly in the UI.
2. Allow a learner to record speech from the microphone in-browser.
3. Send text and audio to the backend for pronunciation evaluation.
4. Return a response that is understandable to a learner, not raw provider output.
5. Show which words likely need correction and provide brief next-step guidance.
6. Keep the end-to-end flow simple enough for a first implementation.

### 4.2 Non-goals

The MVP explicitly excludes:
- user registration or authentication,
- exercise library or content catalog,
- saved history or progress tracking,
- persistence of recordings or results,
- personalized long-term plans,
- conversation practice,
- grammar or vocabulary correction,
- teacher workflows,
- multi-language UI.

---

## 5. Target audience

### 5.1 Primary user

Adult or teenage learner of English who:
- is able to read English text,
- wants to improve spoken clarity,
- prefers quick self-practice,
- wants feedback in US English.

### 5.2 User characteristics

- English level: beginner to upper-intermediate, with strongest fit at intermediate
- Device: desktop/laptop first; mobile Safari best-effort
- Typical session length: 1–5 minutes
- Main intent: “Check whether I pronounced this clearly.”

### 5.3 Primary jobs to be done

- “Let me paste a sentence or passage and check my pronunciation.”
- “Tell me which words were pronounced poorly.”
- “Tell me what to improve next.”

---

## 6. Product principles

1. **One job, well executed.** The product exists to evaluate a spoken reading of a given text.
2. **Minimal interface.** Avoid navigation complexity and unnecessary product surfaces.
3. **Fast feedback.** The user should get results quickly after recording.
4. **Actionable output.** Feedback must point to specific words or issues.
5. **No interpretation burden.** The learner should not need to parse raw JSON or technical speech metrics.

---

## 7. MVP scope

### 7.1 Included product surfaces

- Single-page web app
- Editable multiline textbox with default sample text
- Browser microphone recording
- Record / stop / replay controls
- Analyze button
- Feedback view
- Error states for microphone, upload, and backend failures

### 7.2 Included backend responsibilities

- Accept audio file, text, and locale
- Validate request
- Call pronunciation-assessment pipeline
- Normalize provider output
- Generate concise learner-facing feedback
- Return response synchronously

### 7.3 Excluded backend responsibilities

- Database writes
- Blob storage
- User profiles
- Long-running background jobs
- Analytics storage
- Retry queues

---

## 8. User stories

### 8.1 Core user stories

1. **As a learner**, I want to edit the text before recording, so I can practice my own material.
2. **As a learner**, I want to record my voice through the microphone, so I can submit my speech without uploading files manually.
3. **As a learner**, I want to analyze my recording, so I can get feedback on pronunciation.
4. **As a learner**, I want to see an overall score summary, so I can quickly understand how well I performed.
5. **As a learner**, I want problematic words to be highlighted, so I can identify where the issues occurred.
6. **As a learner**, I want a short explanation of what to improve next, so I know how to practice.

### 8.2 Supporting user stories

7. **As a learner**, I want to replay my recording before submitting, so I can decide whether to re-record.
8. **As a learner**, I want to re-record easily, so I can retry without refreshing the page.
9. **As a learner**, I want clear error messages if the microphone is unavailable or analysis fails, so I know what to do next.

---

## 9. User flow

### 9.1 Happy path

1. User opens ClearSpeak.
2. User sees a text box populated with default text.
3. User edits the text or leaves it unchanged.
4. User clicks **Record**.
5. Browser requests microphone permission if needed.
6. User reads the text aloud.
7. User clicks **Stop**.
8. User optionally replays the recording.
9. User clicks **Analyze pronunciation**.
10. Frontend sends text + audio + locale to backend.
11. Backend evaluates pronunciation.
12. Frontend shows feedback.

### 9.2 Alternate flows

- User denies microphone permission → show recovery instructions.
- User records but wants a second attempt → discard and re-record.
- User submits empty text → block submission and show validation error.
- Backend fails → show retryable analysis error.

---

## 10. Functional requirements

### 10.1 Frontend requirements

#### FR-1 Editable text input
- The page must display a large multiline textbox.
- The textbox must be pre-populated with default sample text.
- The user must be able to fully edit, replace, or clear the text.
- The Analyze action must be disabled if the text is empty or whitespace only.

#### FR-2 Microphone recording
- The user must be able to start recording from the browser.
- The UI must indicate active recording state.
- The user must be able to stop recording.
- The UI must indicate that a valid recording exists after stopping.
- The user must be able to play back the last recorded audio locally.
- The user must be able to discard and re-record.

#### FR-3 Submission
- The user must be able to submit the current text and latest recording for analysis.
- The Analyze button must be disabled during recording.
- The Analyze button must be disabled while a previous analysis request is in progress.
- The frontend must send `locale = en-US` unless explicitly changed later in a future version.

#### FR-4 Feedback rendering
- The app must show overall pronunciation-related scores.
- The app must show a short summary sentence or paragraph.
- The app must show the original text with word-level highlighting.
- The app must allow a user to inspect details for weak words.
- The app must show 1–3 next-step suggestions.

### 10.2 Backend requirements

#### FR-5 Request handling
- The backend must accept multipart form data with audio and text.
- The backend must validate presence of audio and text.
- The backend must reject unsupported locale values in MVP.

#### FR-6 Evaluation
- The backend must call the Azure Speech pronunciation-evaluation pipeline in scripted mode using the submitted text as reference.
- The backend must evaluate against `en-US`.
- The backend must transform provider output into a simplified response schema.

#### FR-7 Feedback generation
- The backend must produce:
  - overall score block,
  - plain-language summary,
  - word-level issue list,
  - next-step suggestions.
- The backend must avoid returning only raw provider data.

#### FR-8 Statelessness
- The backend must not persist input audio, results, or user data in MVP.
- The backend must process the request and return the result within the request-response cycle.

---

## 11. Non-functional requirements

### 11.1 Performance
- UI should remain responsive throughout recording and analysis.
- After submission, feedback should return fast enough to support a short practice loop.
- Target initial median analysis time: under 10 seconds for short recordings.

### 11.2 Reliability
- The frontend must tolerate transient failure and allow manual retry.
- Backend errors must return machine-readable failure codes and user-safe messages.

### 11.3 Security
- Azure credentials must never be exposed to the client.
- Client must communicate only with the app backend.
- Audio must be transmitted over HTTPS.

### 11.4 Privacy
- The MVP must not intentionally store user recordings or results.
- Any temporary in-memory processing must be scoped to a single request.
- The UI should disclose that the recording is sent for cloud-based speech evaluation.

### 11.5 Accessibility
- Recording and Analyze buttons must be keyboard accessible.
- The page must support screen-reader-friendly labels for controls.
- Color highlighting should be paired with labels or icons for users who cannot distinguish colors reliably.

### 11.6 Browser support
- Primary support: recent desktop Chrome, Edge, Safari, and Firefox where `MediaRecorder` and microphone APIs are available.
- Mobile support in MVP: mobile Safari is best-effort, not a hard compatibility commitment.
- Embedded webviews and in-app browsers are not guaranteed in MVP.
- If recording is unsupported, the UI must clearly communicate that limitation.

---

## 12. UX design

## 12.1 Page structure

Single page with three vertical sections:

1. **Text input section**
2. **Recording section**
3. **Feedback section**

No side navigation, no tabs, no dashboard.

### 12.2 Text input section

Elements:
- page title
- short subtitle
- multiline textbox
- helper text

#### Default text
Example default text:

> Three friends were walking through the narrow street when they heard a voice from the white house on the hill.  
> Wait for William, said Heather, or we’ll miss the early train.

### 12.3 Recording section

Elements:
- Record button
- Stop button
- Play recording button
- Re-record button
- recording timer
- microphone permission / error hint
- Analyze pronunciation button

### 12.4 Feedback section

Order:
1. overall scores
2. summary message
3. highlighted text
4. selected-word detail
5. next steps

### 12.5 Interaction states

Required states:
- idle
- requesting microphone permission
- recording
- recorded
- analyzing
- analysis success
- analysis error

---

## 13. Feedback design

### 13.1 Overall score block

Display these score categories:
- Pronunciation
- Accuracy
- Fluency
- Completeness
- Prosody

Use numeric values plus a simple interpretation label if desired, for example:
- Strong
- Fair
- Needs work

### 13.2 Summary block

One concise summary, such as:

> Your speech was understandable, but a few words need correction. Focus on TH sounds and smoother rhythm.

### 13.3 Highlighted text block

Render submitted text word by word with status styling:
- green = good / no issue
- yellow = weak / borderline
- red = likely mispronounced
- muted special marker = omission/insertion issue if represented

### 13.4 Word detail panel

When a word is clicked, show:
- word text
- score
- issue type
- optional phoneme breakdown
- concise coaching note

Example:
- **Word:** Three
- **Score:** 54
- **Issue:** Mispronunciation
- **Feedback:** The first sound should be /θ/, not /t/. Put your tongue lightly between your teeth and let air pass before the `r`.

### 13.5 Next-step suggestions

Show only 1–3 short items.

Examples:
- Practice these words again: **three, through, think**
- Repeat the first sentence more smoothly
- Focus on rhythm, not only individual sounds

---

## 14. Content design

### 14.1 Tone of voice

Feedback tone must be:
- direct,
- supportive,
- non-judgmental,
- concise.

Avoid:
- excessive technical phonetics by default,
- vague praise with no correction,
- overly negative wording.

### 14.2 Copy examples

#### Empty text validation
- “Enter some text to read before analyzing.”

#### No recording validation
- “Record your voice before analyzing.”

#### Microphone denied
- “Microphone access is blocked. Allow microphone access in your browser settings and try again.”

#### Analysis failure
- “Pronunciation analysis failed. Please try again.”

---

## 15. API design

## 15.1 Endpoint

### `POST /api/pronunciation/analyze`

### Request format
`multipart/form-data`

Fields:
- `audio` — required
- `text` — required
- `locale` — required in API contract, constrained to `en-US` in MVP

### Request example

- `audio`: recorded WebM/Opus, MP4/AAC, or WAV
- `text`: `Three friends were walking through the narrow street...`
- `locale`: `en-US`

### 15.1.1 Audio format contract

- The frontend may upload `audio/webm` (Opus), `audio/mp4` / AAC, or WAV.
- The backend must accept those formats in MVP.
- The backend must convert every accepted upload into **mono 16-bit PCM WAV at 16 kHz** before sending audio to the pronunciation-assessment pipeline.
- The frontend should prefer browser-native recording output via feature detection rather than forcing a single MIME type across all browsers.

## 15.2 Success response

```json
{
  "scores": {
    "pronunciation": 76,
    "accuracy": 72,
    "fluency": 81,
    "completeness": 100,
    "prosody": 68
  },
  "summary": "Your speech was understandable, but a few words need correction.",
  "words": [
    {
      "text": "Three",
      "score": 54,
      "errorType": "Mispronunciation",
      "severity": "high",
      "feedback": "The first sound should be /θ/, not /t/."
    },
    {
      "text": "through",
      "score": 71,
      "errorType": "None",
      "severity": "none",
      "feedback": null
    }
  ],
  "nextSteps": [
    "Practice the word 'three' several times.",
    "Repeat the first sentence more smoothly."
  ]
}
```

## 15.3 Error responses

### Validation error
```json
{
  "code": "validation_error",
  "message": "Text is required."
}
```

### Unsupported locale
```json
{
  "code": "unsupported_locale",
  "message": "Only en-US is supported in this version."
}
```

### Evaluation failure
```json
{
  "code": "analysis_failed",
  "message": "Pronunciation analysis failed. Please try again."
}
```

---

## 16. Frontend component model

Suggested components:
- `AppPage`
- `TextInputPanel`
- `RecorderPanel`
- `RecordingTimer`
- `AudioPlaybackControl`
- `AnalyzeButton`
- `ScoreSummary`
- `HighlightedText`
- `WordDetailsPanel`
- `NextStepsPanel`
- `ErrorBanner`

---

## 17. Backend component model

Suggested services:
- `PronunciationController` or minimal API endpoint
- `RequestValidator`
- `AudioFormatHandler`
- `PronunciationAssessmentService`
- `FeedbackBuilder`
- `ResponseMapper`

### 17.1 Service responsibilities

#### `AudioFormatHandler`
- accept uploaded WebM/Opus, MP4/AAC, or WAV
- convert every accepted upload into mono 16-bit PCM WAV at 16 kHz
- return canonical WAV to the pronunciation-assessment service



#### `PronunciationAssessmentService`
- accept normalized input
- call Azure Speech SDK / pipeline
- return raw assessment result

#### `FeedbackBuilder`
- derive summary text
- classify weak words
- generate next steps

#### `ResponseMapper`
- convert internal result to public API schema

---

## 18. Feedback logic rules

The initial version should be deterministic and rule-based.

### 18.1 Example rules

- If a word score is below 60 and error type is `Mispronunciation`, mark it as **high severity**.
- If a word score is between 60 and 75, mark it as **medium severity**.
- If completeness is low, include a suggestion to read every word more carefully.
- If fluency is low, include a suggestion to read in smoother phrases.
- If prosody is low, include a suggestion to vary stress and intonation.
- If 1–3 specific words are much weaker than the rest, include them in next steps.

### 18.2 Output constraints

- Summary: maximum 2 sentences
- Next steps: maximum 3 items
- Word feedback: maximum 1–2 sentences per selected word

---

## 19. Default sample text requirements

The default text should:
- be short enough for quick practice,
- contain common English pronunciation challenges,
- be natural to read aloud.

Recommended starting text:

> Three friends were walking through the narrow street when they heard a voice from the white house on the hill.  
> Wait for William, said Heather, or we’ll miss the early train.

This text is useful because it includes several common pronunciation stress points for learners:
- `th`
- `w`
- `h`
- consonant clusters
- connected speech across short function words

---

## 20. Error handling requirements

### 20.1 Frontend error cases
- microphone permission denied
- unsupported browser recording API
- empty text
- missing recording
- network failure
- backend failure

### 20.2 Backend error cases
- missing file
- empty file
- unsupported audio format
- empty text
- locale not supported
- Azure API failure
- timeout or transient upstream failure

### 20.3 UX requirement
All user-facing errors must be:
- concise,
- recoverable where possible,
- free of internal exception details.

---

## 21. Telemetry and observability

Even without persistence, the implementation should emit operational telemetry if available in hosting infrastructure.

Recommended telemetry events:
- page load
- microphone permission requested
- recording started
- recording stopped
- analysis requested
- analysis succeeded
- analysis failed

Recommended metrics:
- analysis latency
- analysis success rate
- median recording duration
- validation failure rate

This telemetry is for operations and product tuning, not learner history.

---

## 22. Success criteria

The MVP is successful if a user can, in one browser session:

1. edit text,
2. record speech,
3. submit for analysis,
4. receive clear feedback,
5. identify which words need work.

### 22.1 Acceptance criteria

- User can complete the full flow without leaving a single page.
- Analyze is impossible without both text and audio.
- Feedback includes overall scores and word-level issues.
- Backend returns no persistent identifier requirement.
- No user account is needed.

---

## 23. Release scope

## 23.1 MVP release checklist

- [ ] Single-page frontend implemented
- [ ] Microphone capture works in target browsers
- [ ] Backend endpoint implemented
- [ ] Azure Speech integration works for `en-US`
- [ ] Feedback response schema stabilized
- [ ] Highlighted text rendering implemented
- [ ] Re-record flow implemented
- [ ] Error states implemented

## 23.2 Deferred features

- multiple locales
- audio upload from disk
- listen-to-model-pronunciation feature
- phoneme drill generator
- sentence-by-sentence mode
- batch analysis
- save/share result

---

## 24. Risks and mitigations

### Risk 1: Browser microphone issues
**Impact:** user cannot record  
**Mitigation:** clear permission messaging and supported-browser note

### Risk 2: Audio format incompatibility
**Impact:** backend cannot analyze some recordings  
**Mitigation:** accept WebM/Opus, MP4/AAC, and WAV at the API boundary, then convert every upload server-side into mono 16-bit PCM WAV at 16 kHz before pronunciation assessment

### Risk 3: Feedback feels too technical
**Impact:** learner confusion  
**Mitigation:** keep default view simple; reveal phoneme detail only on demand

### Risk 4: Analysis latency feels slow
**Impact:** weak user experience  
**Mitigation:** short default text, progress indicator, optimize request path

### Risk 5: Provider output ambiguity
**Impact:** some learner feedback may feel inconsistent  
**Mitigation:** apply conservative rules and avoid overclaiming exact phonetic diagnosis when confidence is weak

---

## 25. Implementation recommendation

### Frontend
- React + TypeScript, or equivalent lightweight SPA stack
- Use browser `MediaRecorder` for capture
- Desktop-first browser support
- Mobile Safari supported on a best-effort basis in MVP
- Use standard fetch/XHR multipart upload

### Backend
- ASP.NET Core Web API or Minimal API
- Reuse existing C# pronunciation CLI logic as service code
- Accept WebM/Opus, MP4/AAC, or WAV uploads
- Convert every accepted upload into mono 16-bit PCM WAV at 16 kHz before pronunciation assessment
- Keep the backend stateless

### Hosting
- Any environment capable of HTTPS and environment-variable configuration for Azure credentials

---

## 26. Resolved decisions and remaining open questions

### Resolved decisions

1. The browser may upload WebM/Opus, MP4/AAC, or WAV. The backend converts every upload to mono 16-bit PCM WAV at 16 kHz before pronunciation assessment.
2. MVP browser strategy is desktop-first, with mobile Safari supported on a best-effort basis.

### Remaining open questions

1. Should phoneme details be shown by default or only after word click?
2. What exact thresholds define green/yellow/red for word highlighting?
3. Do we want replay of user audio only, or also model audio in later phases?

---

## 27. Appendix A — Example single-page layout

### Header
- Product name: ClearSpeak
- Subtitle: “Read text aloud and get instant pronunciation feedback.”

### Section 1: Text to read
- Multiline textbox
- Helper text

### Section 2: Recording
- Record
- Stop
- Play
- Re-record
- Analyze

### Section 3: Feedback
- Score summary
- Summary paragraph
- Highlighted text
- Word details
- Next steps

---

## 28. Appendix B — Example default feedback

### Example good result
- Summary: “Your pronunciation was mostly clear. Focus on smoother rhythm in longer phrases.”
- Next steps:
  - “Repeat the full sentence more smoothly.”
  - “Try speaking slightly slower at the start.”

### Example mixed result
- Summary: “Your speech was understandable, but several words need correction.”
- Next steps:
  - “Practice these words again: three, white, weather.”
  - “Focus on TH sounds.”
  - “Repeat the first sentence in one smooth phrase.”

---

## 29. Final product definition

ClearSpeak MVP is a **single-page pronunciation checker** for US English reading practice.

The learner can:
- enter or edit text,
- record themselves,
- submit the recording,
- receive immediate pronunciation feedback.

The system is intentionally minimal:
- no accounts,
- no storage,
- no history,
- no exercise library,
- no persistence.

Its purpose is to provide a focused and implementable pronunciation-feedback experience on top of the existing Azure Speech Service pipeline.
