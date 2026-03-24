# ClearSpeak v1.2 — Screen-by-Screen Wireframe Spec

## 1. Purpose

This document defines the wireframe-level UX specification for **ClearSpeak v1.2**.

Assumptions:
- **Phase 1 / v1.1** is complete.
- v1.1 already supports the core loop: enter or choose text, record speech, send audio to backend, receive pronunciation feedback.
- **v1.2** adds the highest-ROI continuity features:
  - passage library
  - saved attempts
  - history
  - basic progress tracking
  - retry / compare workflow

This spec is intentionally focused on **wireframe behavior, layout, states, and interactions**, not visual styling.

---

## 2. Product goals for v1.2

The user should be able to:
1. choose a practice passage from a library
2. record and analyze pronunciation
3. save the attempt automatically
4. review prior attempts for the same passage
5. compare the latest attempt against the previous one
6. see simple progress trends without needing a complex dashboard

---

## 3. Information architecture

### Primary navigation

v1.2 should move from a single-page tool toward a small multi-screen app.

Top-level navigation:
- **Practice**
- **History**
- **Progress**
- **Settings**

### Core route map

- `/` → redirect to `/practice`
- `/practice` → practice home / passage library
- `/practice/:passageId` → reading practice workspace
- `/attempts` → attempt history list
- `/attempts/:attemptId` → attempt detail
- `/compare/:attemptId?baseline=:baselineId` → compare two attempts
- `/progress` → basic progress tracking
- `/settings` → language, mic, privacy, account state

---

## 4. Global layout rules

### Desktop-first layout

All v1.2 screens should be optimized for laptop/desktop first.

Global shell:
- top app bar
- optional left nav rail on wider screens
- content area centered with comfortable reading width

### App shell regions

#### A. Top bar
Contents:
- ClearSpeak logo / wordmark
- current section title
- user menu or placeholder account chip
- quick action button: **Start practice**

#### B. Left navigation (desktop)
Items:
- Practice
- History
- Progress
- Settings

#### C. Main content region
- route-specific content
- max-width container
- generous whitespace

### Mobile fallback

For narrow screens:
- collapse left nav into bottom nav or hamburger
- stack panels vertically
- preserve full functionality

---

## 5. Shared UI patterns

### Score chip colors
- green: strong
- yellow: acceptable / needs attention
- red: weak / priority
- neutral gray: informational

### Result badges
- **Mispronunciation**
- **Omission**
- **Insertion**
- **Fluency**
- **Prosody**

### Empty states
Each major list screen must have an empty state with:
- one sentence explaining the state
- one primary CTA
- one optional supporting hint

### Loading states
Use clear placeholders:
- skeleton cards for lists
- progress spinner for analysis
- disabled primary actions during upload/processing

### Error states
Each critical flow must support:
- microphone permission error
- upload/network error
- unsupported audio state
- backend timeout / provider failure
- no recognition / low-quality recording case

---

## 6. Screen 1 — Practice Home / Passage Library

### Route
`/practice`

### Purpose
Give the user a clear entry point into structured practice.

### Primary user tasks
- browse passages
- filter by difficulty or focus area
- resume recent passage
- start a new practice attempt

### Layout

#### Header area
Elements:
- page title: **Practice**
- subtitle: *Choose a passage and practice your pronunciation.*
- primary CTA: **Continue last passage** if applicable

#### Section A — Recommended / resume card
A single prominent card at the top.

Card content:
- passage title
- difficulty
- last practiced date
- last pronunciation score
- CTA: **Continue practicing**

If no prior attempts exist:
- show a getting-started card instead
- CTA: **Start with a beginner passage**

#### Section B — Search and filters row
Controls:
- search input: “Search passages”
- difficulty filter: Beginner / Intermediate / Advanced
- focus filter: TH, R/L, Vowels, Rhythm, Mixed
- sort dropdown: Recommended / Recently practiced / Shortest / Newest

#### Section C — Passage library grid/list
Each passage card shows:
- title
- short preview snippet
- estimated reading time
- difficulty label
- focus tags
- last score if previously attempted
- attempts count
- CTA: **Open**

### Wireframe notes
- 2–3 column grid on wide screens
- single column stacked cards on narrow screens
- search and filters remain sticky only if easy to implement

### Empty state
If no results match filters:
- title: **No passages match your filters**
- actions: **Clear filters**

### Key interactions
- clicking a passage opens the practice workspace
- filters update results immediately
- recent score appears only if prior attempts exist

---

## 7. Screen 2 — Practice Workspace

### Route
`/practice/:passageId`

### Purpose
This is the main work surface for recording, submission, and reviewing immediate results.

### Primary user tasks
- read the chosen passage
- record audio
- submit for analysis
- review feedback
- retry immediately
- compare against previous attempt

### Layout
Two-column layout on desktop.

#### Left column — Practice input panel

##### A. Passage header
Elements:
- breadcrumb: Practice / Passage title
- passage title
- difficulty badge
- focus tags
- estimated time
- quick action: **Change passage**

##### B. Passage text card
Elements:
- section title: **Read this aloud**
- full passage text
- optional sentence splitting for readability
- helper text: *Speak clearly at a natural pace.*

Optional controls:
- text size toggle
- show line numbers toggle

##### C. Recording panel
Elements:
- microphone status
- **Record** button
- **Stop** button
- **Play** button
- **Re-record** button
- timer
- recording waveform or simple audio status bar
- **Analyze pronunciation** primary CTA

Behavior:
- Analyze disabled until a valid recording exists
- Play enabled only after recording
- Re-record replaces current unsaved recording

##### D. Last-attempt shortcut
A compact inline card beneath recording controls:
- last score
- date/time
- button: **Compare with last attempt**

#### Right column — Live result / latest feedback panel

This column changes by state.

### State: before analysis
Show placeholder panel:
- title: **Your feedback will appear here**
- bullets:
  - overall scores
  - highlighted weak words
  - next steps

### State: analyzing
Show progress panel:
- spinner
- label: **Analyzing pronunciation...**
- secondary text: *This usually takes a few seconds.*
- disable Record / Analyze actions during processing

### State: results loaded
Show the following stacked sections:

##### 1. Score summary cards
Cards:
- Pronunciation
- Accuracy
- Fluency
- Completeness
- Prosody

Each card shows:
- numeric score
- short interpretive label such as Strong / Needs work

##### 2. Summary card
One compact learner-friendly summary:
- one positive line
- one priority issue
- one next-step sentence

##### 3. Highlighted passage card
Show original passage with color-coded words:
- green = strong
- yellow = moderate
- red = weak / mispronounced
- special styling for omitted / inserted issues

Interaction:
- clicking a word opens the word detail drawer or side panel

##### 4. Word detail panel
Shown inline on desktop right rail or below highlighted text on smaller screens.

Fields:
- selected word
- score
- issue type
- phoneme list
- coaching tip
- button: **Practice this word again**

##### 5. Next steps card
Show 2–3 concise actions:
- retry full passage
- focus on 1–3 weak words
- improve rhythm on first sentence

##### 6. Save/compare actions row
Buttons:
- **Retry now**
- **Compare with previous attempt**
- **View full attempt details**

### Key interactions
- after successful analysis, attempt is saved automatically
- if prior attempt exists for same passage, comparison CTA appears prominently
- switching selected word updates word detail panel without leaving page

### Critical states

#### Mic permission denied
Show inline alert:
- explanation
- browser-specific retry hint
- CTA: **Try again**

#### Audio too short
Show validation message:
- *Please record at least X seconds of speech.*

#### Analysis failed
Show error box:
- *We couldn’t analyze this recording.*
- actions: **Retry upload**, **Re-record**

---

## 8. Screen 3 — Attempt Saved Confirmation Strip

### Scope
This is not a full page. It is an in-context UI state shown inside the Practice Workspace immediately after results are saved.

### Purpose
Reassure the user that progress is not lost.

### Placement
Top of the results column or just beneath the page header.

### Content
- icon: success
- text: **Attempt saved**
- supporting text: *You can review it later in History.*
- quick links:
  - **Open History**
  - **Compare with previous attempt**

This is small but important. It introduces v1.2’s persistence value clearly.

---

## 9. Screen 4 — History List

### Route
`/attempts`

### Purpose
Let the user browse prior attempts and re-open old results.

### Primary user tasks
- find a prior attempt
- filter by passage
- sort by date or score
- reopen details
- resume practice on the same passage

### Layout

#### Header row
- title: **History**
- subtitle: *Review your saved pronunciation attempts.*

#### Filter row
Controls:
- search by passage title
- date range filter
- score range filter
- passage filter
- sort by: Newest / Oldest / Best score / Lowest score

#### Attempt list
Each row/card shows:
- passage title
- attempt timestamp
- pronunciation score
- delta vs previous attempt on same passage if available
- short summary tag: Improved / Stable / Needs work
- actions:
  - **View details**
  - **Practice again**
  - **Compare**

### Row interaction model
Primary click opens attempt detail.
Secondary actions are explicit buttons.

### Empty state
- title: **No saved attempts yet**
- message: *Complete your first practice to start tracking progress.*
- CTA: **Go to Practice**

---

## 10. Screen 5 — Attempt Detail

### Route
`/attempts/:attemptId`

### Purpose
Show one saved assessment in full detail outside the practice workflow.

### Primary user tasks
- review scores and mistakes
- inspect highlighted text
- revisit weak words
- start a new attempt from the same passage
- compare against another attempt

### Layout

#### Header
Elements:
- breadcrumb: History / Passage / Attempt
- passage title
- timestamp
- main score pill
- buttons:
  - **Practice again**
  - **Compare**

#### Main content layout
Two columns on desktop.

##### Left column
1. Score cards
2. summary card
3. highlighted passage card

##### Right column
1. focus words list
2. selected word detail card
3. next steps card
4. metadata card
   - attempt date
   - duration if available
   - locale

### Focus words list
A ranked list of weak words:
- word
- score
- issue badge
- clicking a word updates the detail card

### Key interactions
- selecting a weak word scrolls/highlights the word inside the passage
- compare button opens compare flow with sensible baseline preselected

---

## 11. Screen 6 — Compare Attempts

### Route
`/compare/:attemptId?baseline=:baselineId`

### Purpose
Show whether the learner improved between two attempts.

### Why this matters
This is one of the highest-ROI v1.2 features because it makes progress visible and motivates repetition.

### Primary user tasks
- compare latest attempt with previous attempt on same passage
- identify improved words and remaining problems
- decide what to practice next

### Layout

#### Header
- title: **Compare attempts**
- passage title
- baseline attempt selector
- current attempt selector

Selectors default to:
- latest attempt
- previous attempt for same passage

#### Section A — Delta summary
Cards:
- Pronunciation delta
- Accuracy delta
- Fluency delta
- Completeness delta
- Prosody delta

Each card shows:
- current score
- previous score
- change arrow
- numeric delta

#### Section B — Comparison summary
Short prose summary such as:
- *Your pronunciation improved by 6 points since the last attempt.*
- *You fixed two weak words, but rhythm remains a priority.*

#### Section C — Passage comparison view
Text display where each word can show change state:
- improved
- unchanged
- worse
- new issue

Possible wireframe representations:
- subtle up/down arrows above words
- border color states
- clickable words for exact delta details

#### Section D — Improved words panel
List of words with strongest positive change.

#### Section E — Still weak words panel
List of words still below threshold.

#### Section F — Recommended next action
Examples:
- **Practice again now**
- **Switch to a shorter TH passage**
- **Focus on fluency next**

### Interaction notes
- if only one attempt exists, compare route should gracefully fall back and ask the user to create another attempt
- user can swap baseline/current selectors

---

## 12. Screen 7 — Progress Overview

### Route
`/progress`

### Purpose
Provide light-weight trend tracking without becoming a heavy analytics dashboard.

### Primary user tasks
- see whether overall pronunciation is improving
- identify recurring weak sounds or words
- understand activity level

### Layout

#### Header
- title: **Progress**
- subtitle: *Track your improvement over time.*

#### Section A — KPI strip
Cards:
- total attempts
- average pronunciation score
- best score
- passages practiced

#### Section B — Score trend chart
A simple line chart with attempt date on X-axis and pronunciation score on Y-axis.

Optional toggles:
- Pronunciation
- Accuracy
- Fluency
- Prosody

#### Section C — Passage progress table
Columns:
- passage title
- latest score
- best score
- attempts count
- trend indicator

#### Section D — Recurring weak words
Top list of words most often flagged.

Columns or chips:
- word
- times flagged
- latest score
- quick link: **Practice passage**

#### Section E — Practice consistency card
Simple habit metric:
- attempts last 7 days
- attempts last 30 days

### Empty state
If fewer than 2 attempts exist:
- message: *Progress will appear after a few saved attempts.*
- CTA: **Start practicing**

### Scope guardrail
v1.2 progress should stay basic.
Do not add:
- advanced drill plans
- phoneme mastery heatmaps
- teacher analytics
- predictive coaching engine

Those belong to later phases.

---

## 13. Screen 8 — Settings

### Route
`/settings`

### Purpose
Provide practical controls only.

### Primary user tasks
- confirm target locale
- test microphone
- manage privacy/data behavior
- understand saved-attempt behavior

### Layout

#### Section A — Practice settings
Fields:
- target language: `en-US` (read-only or single option in v1.2)
- preferred text size
- autoplay result summary toggle if applicable

#### Section B — Audio settings
- microphone test button
- input level indicator
- help text for browser permission

#### Section C — Data & privacy
- explain that attempts are saved for history/progress
- option: delete all saved attempts
- option: delete individual account data if auth exists

#### Section D — About
- app version
- build info
- help/support link

---

## 14. Cross-screen interaction flows

### Flow A — New user
1. Open Practice
2. See recommended beginner passage
3. Open passage
4. Record + analyze
5. View results
6. Attempt auto-saved
7. Prompt to view History or retry

### Flow B — Returning user
1. Open Practice
2. Resume recent passage
3. Record another attempt
4. Compare against previous
5. Open Progress later

### Flow C — Review old attempt
1. Open History
2. Filter by passage
3. Open attempt detail
4. Practice again from same passage

### Flow D — Improvement loop
1. Analyze attempt
2. Click Retry now
3. Submit second attempt
4. Open Compare view
5. See improved vs still weak words

---

## 15. State matrix by screen

## Practice Home
States:
- loading library
- loaded with recommendations
- filtered results
- empty results
- library fetch error

## Practice Workspace
States:
- initial
- recording
- recorded
- analyzing
- results loaded
- save confirmation
- mic error
- provider error

## History
States:
- loading
- populated
- empty
- filter no-match
- load error

## Attempt Detail
States:
- loading
- loaded
- not found
- deleted/missing

## Compare
States:
- loading
- ready with 2 attempts
- insufficient attempts
- selector changed
- compare error

## Progress
States:
- loading
- enough data
- too little data
- load error

---

## 16. Component inventory for v1.2

### Navigation
- `AppShell`
- `TopBar`
- `SideNav`

### Practice
- `PassageLibraryPage`
- `PassageFilterBar`
- `PassageCard`
- `PracticeWorkspacePage`
- `PassageHeader`
- `PassageTextCard`
- `RecorderPanel`
- `ScoreSummaryCards`
- `HighlightedPassage`
- `WordDetailPanel`
- `NextStepsCard`
- `AttemptSavedBanner`

### History
- `HistoryPage`
- `AttemptFilterBar`
- `AttemptList`
- `AttemptRow`
- `AttemptDetailPage`

### Compare
- `ComparePage`
- `AttemptSelectorBar`
- `DeltaSummaryCards`
- `ComparisonPassageView`
- `ImprovedWordsPanel`
- `RemainingIssuesPanel`

### Progress
- `ProgressPage`
- `KpiStrip`
- `ScoreTrendChart`
- `PassageProgressTable`
- `RecurringWeakWordsCard`

### Settings
- `SettingsPage`
- `MicrophoneTestCard`
- `DataPrivacyCard`

---

## 17. Backend/API implications for the wireframes

v1.2 wireframes assume the backend can now support:
- passage catalog retrieval
- attempt persistence
- attempt listing and filtering
- attempt detail retrieval
- compare endpoint or compare-ready data shape
- aggregate progress endpoint

Suggested API surface:
- `GET /api/passages`
- `GET /api/passages/{id}`
- `POST /api/assessments`
- `GET /api/assessments`
- `GET /api/assessments/{id}`
- `GET /api/assessments/{id}/compare?baselineId=...`
- `GET /api/progress`
- `GET /api/progress/passages`

---

## 18. UX priorities for implementation

If implementation capacity is limited, prioritize in this order:

1. Practice Home / Passage Library
2. Practice Workspace with auto-save
3. History List
4. Attempt Detail
5. Compare Attempts
6. Progress Overview
7. Settings polish

That order preserves the highest product ROI for v1.2.

---

## 19. Explicit v1.2 guardrails

Do not add in v1.2:
- advanced lesson plans
- daily streak gamification beyond simple counters
- social/community features
- multi-locale expansion
- free-speaking mode
- teacher portal
- LLM-generated coaching flows everywhere
- heavy analytics dashboard

Keep v1.2 focused on one big upgrade over v1.1:
**turning one-off pronunciation checks into a repeatable improvement loop.**

---

## 20. Final wireframe summary

v1.2 should feel like a compact practice app with five screens that matter most:
- choose passage
- practice and analyze
- review history
- inspect a saved attempt
- compare progress

The UI should still remain light, calm, and task-oriented.

The learner should always know:
- what to read
- what went wrong
- what improved
- what to do next
