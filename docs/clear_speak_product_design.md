# ClearSpeak — Simplified Product Design

## 1. Product overview

**ClearSpeak** is a lightweight web app for US English learners who want quick feedback on their pronunciation while reading a known text aloud.

The user can:
1. edit a short piece of text in a textbox
2. record themselves reading it aloud with a microphone
3. send the text and audio to the backend
4. receive pronunciation feedback

This MVP is intentionally minimal. It has no accounts, no history, no library, and no persistence.

---

## 2. Product goal

Help a learner quickly identify likely mispronunciations in US English and understand what to improve next.

---

## 3. Scope

### Included
- editable textbox with default sample text
- microphone recording in the browser
- backend pronunciation evaluation
- feedback view with scores and word-level issues

### Excluded
- no user accounts
- no passage library
- no saved history
- no persistence of recordings or results
- no dashboards or progress tracking
- no lesson plans

---

## 4. Target user

An English learner who wants fast, simple pronunciation feedback for **US English** by reading a known text aloud.

---

## 5. Core user flow

1. User opens the page.
2. User sees an editable textbox pre-filled with sample text.
3. User changes the text if needed.
4. User clicks **Record** and reads the text aloud.
5. User clicks **Stop**.
6. User clicks **Analyze pronunciation**.
7. Frontend sends the text and audio to the backend.
8. Backend evaluates the speech.
9. User sees the feedback.

---

## 6. UX structure

The app is a **single-page web app** with three blocks.

### Block A — Text input
Purpose: define what the user will read.

Elements:
- heading: **Text to read**
- large multiline textbox
- default text already filled in
- helper text: “Edit the text, then read it aloud.”

### Default sample text
> Three friends were walking through the narrow street when they heard a voice from the white house on the hill.  
> Wait for William, said Heather, or we’ll miss the early train.

### Block B — Recording
Purpose: capture the learner’s speech.

Elements:
- **Record** button
- **Stop** button
- **Play recording** button
- recording timer
- microphone permission / error message
- **Analyze pronunciation** button

Behavior:
- Analyze is disabled until audio exists
- user can re-record
- UI stays simple and focused

### Block C — Feedback
Purpose: show the result clearly.

Sections:
1. overall scores
2. short summary
3. highlighted text
4. word detail panel
5. next steps

---

## 7. Feedback design

### A. Overall score card
Show:
- Pronunciation
- Accuracy
- Fluency
- Completeness
- Prosody

Example:
- Pronunciation: 76
- Accuracy: 72
- Fluency: 81
- Completeness: 100
- Prosody: 68

### B. Short summary
A compact learner-friendly summary, for example:

> Your speech was understandable, but a few words need correction. Focus on TH sounds and smoother rhythm.

### C. Highlighted text
Show the original text with word-level color coding:
- green = good
- yellow = needs attention
- red = likely mispronounced
- gray underline / strike = omitted or inserted issue

Words should be clickable.

### D. Word detail panel
When a highlighted word is selected, show:
- word
- score
- issue type
- phoneme details if available
- simple coaching tip

Example:
- **Word:** Three  
- **Score:** 54  
- **Issue:** Mispronunciation  
- **Tip:** The first sound should be /θ/, not /t/. Put your tongue lightly between your teeth and release air.

### E. Next step section
Show only 1–3 concise actions, for example:
- Practice these words again: **three, through, think**
- Repeat the first sentence more smoothly
- Focus on rhythm, not just individual words

---

## 8. Frontend design

### Layout
A simple vertical page:

#### Header
- product name: **ClearSpeak**
- one-line subtitle: *Read aloud. Get clear pronunciation feedback.*

#### Main content
1. text input panel
2. recording panel
3. feedback panel

### UI states
- idle
- recording
- recorded
- analyzing
- results
- error

### Design principles
- one page only
- minimal visual noise
- clear primary actions
- no extra navigation
- no dashboard

---

## 9. Backend design

### Responsibility
The backend should:
1. receive audio and text
2. call Azure Speech pronunciation assessment
3. normalize the result
4. generate learner-friendly feedback
5. return the response

### Important constraint
The backend does **not** store anything.

It does not persist:
- audio
- users
- results
- attempts

It processes the request and returns a result only.

---

## 10. Minimal API design

### Endpoint
`POST /api/pronunciation/analyze`

### Request
`multipart/form-data`

Fields:
- `audio` — recorded audio file
- `text` — reference text from textbox
- `locale` — `en-US`

### Response example
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
      "feedback": "The first sound should be /θ/, not /t/."
    },
    {
      "text": "through",
      "score": 71,
      "errorType": "None",
      "feedback": null
    }
  ],
  "nextSteps": [
    "Practice the word 'three' several times.",
    "Repeat the first sentence more smoothly."
  ]
}
```

---

## 11. Backend pipeline

### Request flow
1. receive audio and text
2. validate request
3. call Azure Speech pronunciation assessment
4. extract overall scores
5. extract word-level results
6. extract phoneme-level results when useful
7. build simplified feedback
8. return JSON response

### Validation rules
- text must not be empty
- audio file must exist
- locale must be `en-US`

### Feedback engine
Keep feedback rule-based.

#### Example rules
- if a word score is below 60 and error type is `Mispronunciation`, mark it as a priority word
- if completeness is low, tell the learner they likely skipped some words
- if fluency is low, suggest smoother phrasing
- if prosody is low, suggest stronger stress and more natural intonation

No LLM is required for this MVP.

---

## 12. Minimal response contract

### Top-level fields
- `scores`
- `summary`
- `words`
- `nextSteps`

### Word fields
- `text`
- `score`
- `errorType`
- `feedback`

Optional later:
- `offset`
- `duration`
- `phonemes`

---

## 13. Example page behavior

### Initial page
- textbox contains default text
- Record button is enabled
- feedback area is empty

### After recording
- Play recording is enabled
- Analyze button is enabled

### During analysis
- show spinner
- show text: **Analyzing pronunciation...**

### After result
- score cards appear
- text is highlighted
- weak words are clickable
- summary and next steps are shown

---

## 14. Simplified component list

### Frontend
- `TextInputPanel`
- `RecorderPanel`
- `AnalyzeButton`
- `ScoreSummary`
- `HighlightedText`
- `WordFeedbackPanel`
- `NextStepsPanel`

### Backend
- `PronunciationController`
- `PronunciationAssessmentService`
- `FeedbackBuilder`

---

## 15. Success criteria

The MVP is successful if a user can:
1. open the page
2. edit the text
3. record speech
4. analyze pronunciation
5. understand which words need work

That is enough for version 1.

---

## 16. Recommended implementation shape

### Frontend
- React, Next.js, or plain HTML/JavaScript
- browser microphone recording via MediaRecorder

### Backend
- ASP.NET Core Web API
- use the Azure Speech pronunciation pipeline already implemented

---

## 17. Product statement

**ClearSpeak** is a single-page pronunciation checker for **US English** reading practice.

The learner can:
- edit a text
- read it aloud
- submit audio to the backend
- receive scores, weak-word highlighting, and short corrective feedback

The MVP is intentionally minimal and has:
- no accounts
- no storage
- no history
- no library

It is optimized for one task: **read aloud and get clear pronunciation feedback immediately**.
