# ClearSpeak — Product Design

**Product:** ClearSpeak  
**Current shipped version:** 1.1  
**Next planned version:** 1.2  
**Document status:** Updated after Phase 1 completion

---

## 1. Product overview

**ClearSpeak** is a web app for US English learners who want to improve pronunciation by reading a known text aloud and receiving clear, structured feedback.

The core loop remains:
1. choose or enter text
2. read it aloud into a microphone
3. send text and audio to the backend
4. receive pronunciation feedback
5. practice the weak words and try again

The original simplified design served as the starting point. Phase 1 has now been completed, so this document no longer treats the product as a hypothetical MVP. Instead, it treats the existing app as the baseline and defines the next high-ROI expansion.

---

## 2. Product goal

Help a learner improve US English pronunciation through a tight practice loop:
- read aloud
- detect likely mispronunciations
- understand what to improve
- repeat the same or similar text
- measure progress over time

---

## 3. Product status and roadmap framing

## Phase 1 — completed (v1.1)
Phase 1 is complete.

It established the first working pronunciation-checking product that allows a learner to:
- enter or edit reference text
- record speech in the browser
- submit audio to the backend
- receive pronunciation feedback

This is now the **baseline product**, not the target state.

## Phase 2 — next (v1.2)
Phase 2 should focus on the highest-ROI features that move ClearSpeak from a one-off checker into a repeatable learning product.

The purpose of v1.2 is **not** to add every missing feature. The purpose is to add the smallest set of features that make repeated practice substantially more valuable.

---

## 4. Phase 1 completed features (v1.1)

The following features are considered shipped in v1.1.

### Product features completed
- single-page web experience for pronunciation practice
- editable text input for custom reading text
- browser-based microphone recording
- ability to stop, replay, and re-record before analysis
- pronunciation analysis triggered from the UI
- result screen with overall pronunciation feedback
- word-level issue feedback
- learner-friendly feedback summary
- immediate read-aloud to feedback workflow

### Technical capabilities completed
- React + TypeScript + Vite frontend
- ASP.NET Core Minimal API backend
- backend integration with the extracted pronunciation pipeline
- Azure Speech provider support
- deterministic fake provider for development/testing
- upload normalization to mono 16-bit PCM WAV at 16 kHz
- support for `audio/webm;codecs=opus`, `audio/mp4`, and `audio/wav`
- static frontend hosting from the ASP.NET app
- automated build, test, publish, and Azure App Service deployment pipeline
- backend and frontend test coverage foundations

### Constraints that remain after v1.1
- no passage library
- no saved attempts/history
- no progress tracking
- no structured repeat-practice workflow
- no personalized practice recommendations across attempts
- no lesson or curriculum layer

---

## 5. Target user

An English learner who wants fast, practical pronunciation feedback for **US English**.

Primary characteristics:
- can read short English passages
- wants clearer pronunciation, not academic phonetics study
- needs concrete feedback on what to improve next
- wants short, repeatable practice sessions
- benefits from visible progress over time

---

## 6. Product principles

1. **Practice first**  
   ClearSpeak is for doing pronunciation practice, not browsing content.

2. **Immediate feedback**  
   The user should get from recording to feedback quickly.

3. **Actionable correction**  
   Feedback must tell the learner what to do next, not just what score they got.

4. **Low friction**  
   The product should remain simple enough that a learner can start practicing within seconds.

5. **Progress visibility**  
   Once the core checker works, the next most valuable layer is showing whether the learner is improving.

---

## 7. Why Phase 2 should focus on a small set of high-ROI features

The current product already proves the core technical loop. The biggest missing value is not more acoustic sophistication; it is **learning continuity**.

Right now, the learner can get a result once. What is missing is the product layer that makes them come back, repeat practice, and see improvement.

That is why Phase 2 should prioritize:
1. a **passage library** so the learner has something structured to practice
2. **saved attempts and history** so practice is not disposable
3. **basic progress tracking** so the learner can see improvement and recurring weak areas
4. a **repeat-practice flow** so the learner can immediately retry the same passage after feedback

These features materially improve user value while still staying close to the current architecture.

---

## 8. Phase 2 scope (v1.2)

## Included in v1.2

### 8.1 Passage library
Add a curated library of reading passages.

Each passage should have:
- id
- title
- text
- difficulty level
- estimated duration
- optional pronunciation focus tags, such as:
  - TH
  - W vs V
  - R vs L
  - vowel length
  - sentence stress

Why this is high ROI:
- removes the blank-state problem
- gives learners structured material immediately
- enables repeatability and comparison across attempts
- creates a natural basis for future curriculum and recommendations

### 8.2 Saved attempts
Persist assessment attempts.

Each attempt should save:
- user identity or lightweight profile id
- passage id or custom-text flag
- reference text snapshot
- recognized text snapshot
- top-level scores
- normalized word-level results
- created timestamp

Audio retention can be optional in v1.2. The important part is preserving the **assessment outcome**.

Why this is high ROI:
- turns each analysis into durable learning data
- makes history and progress possible
- enables retry/compare flows

### 8.3 Practice history
Add a page or panel that shows previous attempts.

History should show:
- date/time
- passage title or custom text label
- pronunciation score
- top problem summary
- retry action

Why this is high ROI:
- gives users a sense of continuity
- reduces repeated setup cost
- supports habit formation

### 8.4 Progress tracking
Add a simple progress view.

Show:
- overall pronunciation trend over time
- best and latest score for a passage
- most frequent weak words
- most frequent weak sound-focus tags, if available

This should remain simple in v1.2. It does not need to become a full analytics dashboard.

Why this is high ROI:
- lets the learner answer the core question: “Am I improving?”
- increases retention more than cosmetic UI work

### 8.5 Repeat-practice workflow
After feedback, the learner should be able to:
- retry the same passage
- compare latest score to previous attempt
- quickly re-record without re-entering text

This should be a first-class action in the result screen.

Why this is high ROI:
- directly supports the learning loop
- converts passive feedback into active correction

---

## 9. Explicitly not in v1.2

The following features remain valuable, but should stay in later phases.

### Later phase features
- advanced lesson plans
- adaptive curricula
- spaced repetition for weak words/sounds
- AI-generated coaching beyond rule-based feedback
- free speaking mode
- teacher dashboard
- social or classroom features
- deep account/profile systems with subscriptions and billing
- mobile app
- advanced audio playback overlays and waveform scrubbing
- comparison to native reference audio
- TTS-based model pronunciation playback
- multi-locale support beyond `en-US`

These features should not be pulled into v1.2 unless one directly unlocks a selected Phase 2 feature.

---

## 10. Updated user experience vision

## Core product structure after v1.2
The product should evolve from one page into a small but clear practice app.

### Primary navigation
- **Practice**
- **Library**
- **History**
- **Progress**

This is still lightweight. It is not a large application yet.

---

## 11. Updated user flows

## Flow A — practice from library
1. User opens the app.
2. User goes to **Library**.
3. User picks a passage.
4. User sees the passage text and pronunciation focus.
5. User records speech.
6. User submits for analysis.
7. User receives feedback.
8. User clicks **Try again**.
9. The new attempt is saved.
10. Progress updates.

## Flow B — quick practice with custom text
1. User opens **Practice**.
2. User enters custom text.
3. User records and analyzes.
4. Result is saved as a custom-text attempt.
5. User can retry or view it in history.

## Flow C — review progress
1. User opens **Progress**.
2. User sees score trend.
3. User sees recurring weak words or passages.
4. User jumps back into a targeted retry.

---

## 12. Frontend design for v1.2

## Practice page
Purpose: keep the original fast loop.

Elements:
- text panel
- recording controls
- analyze action
- result panel
- retry action
- compare-to-last-attempt summary

## Library page
Purpose: provide structured practice material.

Elements:
- passage cards or list
- difficulty filter
- sound-focus filter
- estimated duration
- start practice action

## History page
Purpose: show saved attempts.

Elements:
- attempts list
- date/time
- passage/custom text label
- pronunciation score
- top issue summary
- retry button

## Progress page
Purpose: show simple learning progress.

Elements:
- overall score trend
- attempts count
- best score
- latest score
- weakest recurring words or passages

---

## 13. Backend design for v1.2

## Backend responsibilities
The backend should now:
1. receive audio and text
2. evaluate pronunciation
3. normalize the result
4. generate learner-friendly feedback
5. persist the attempt record
6. return the response
7. expose history/progress/library endpoints

## New backend capabilities required
- passage catalog storage and retrieval
- attempt persistence
- progress aggregation queries
- retry-friendly retrieval by passage or recent attempt

---

## 14. API evolution

## Existing core endpoint
`POST /api/pronunciation/analyze`

This remains the central analysis endpoint.

## New v1.2 endpoints

### Passage library
- `GET /api/passages`
- `GET /api/passages/{id}`

### Saved attempts
- `GET /api/attempts`
- `GET /api/attempts/{id}`

### Progress
- `GET /api/progress/summary`
- `GET /api/progress/passages/{passageId}`

### Retry support
- `POST /api/passages/{id}/attempts`
- or reuse the main analysis endpoint with `passageId`

The API should stay simple and avoid over-design in v1.2.

---

## 15. Data model additions for v1.2

### Passage
- id
- title
- text
- difficulty
- focusTags
- estimatedDurationSeconds
- isActive

### Attempt
- id
- userId or profileId
- passageId nullable
- customTextLabel nullable
- referenceText
- recognizedText
- pronunciationScore
- accuracyScore
- fluencyScore
- completenessScore
- prosodyScore
- summary
- createdAt

### AttemptWord
- id
- attemptId
- text
- score
- errorType
- feedback
- offset nullable
- duration nullable

### ProgressSummary
- userId or profileId
- attemptsCount
- latestScore
- bestScore
- averageScore
- recentTrend

---

## 16. Identity and persistence strategy for v1.2

v1.2 requires persistence. That means the product needs at least a lightweight identity strategy.

Recommended approach for v1.2:
- keep authentication minimal
- allow a lightweight user/profile concept sufficient to attach attempts to a person
- avoid full subscription/billing complexity

Possible implementation options:
1. simple sign-in
2. magic-link auth
3. temporary anonymous profile with upgrade later

The exact auth method is less important than enabling **saved practice history**.

---

## 17. Feedback model in v1.2

The core feedback model remains the same:
- overall scores
- summary
- word-level issues
- next steps

v1.2 should extend it with context:
- comparison to previous attempt on the same passage
- “improved / declined / unchanged” messaging
- persistent weak-word tracking

Example additions:
- “Your pronunciation score improved by 6 points since the last attempt.”
- “The word ‘three’ is still a recurring weak word.”
- “You improved fluency on this passage, but TH sounds still need work.”

This produces more value than adding exotic feedback formats.

---

## 18. Success criteria for v1.2

v1.2 is successful if a learner can:
1. choose a passage from a library
2. record and analyze pronunciation
3. save the attempt
4. revisit past attempts
5. see whether they are improving
6. retry the same passage easily

That is the threshold where ClearSpeak becomes a learning product, not just a pronunciation checker.

---

## 19. Metrics for v1.2

### Product metrics
- practice sessions per active user
- retry rate after first feedback
- library passage starts
- history revisit rate
- progress page usage

### Learning metrics
- change in average pronunciation score over repeated attempts
- percentage of users with at least 3 attempts on the same passage
- reduction in repeated weak words over time

### Retention metrics
- day 1 to day 7 repeat usage
- weekly active learners
- percentage of learners with more than one saved attempt

---

## 20. Recommended implementation order for Phase 2

Implement in this order:

### Step 1 — passage library
Lowest product friction, immediate UX value.

### Step 2 — saved attempts
Unlocks durability and future progress views.

### Step 3 — history page
Lets users benefit from persistence immediately.

### Step 4 — progress summary
Turns saved attempts into motivation and learning visibility.

### Step 5 — compare-and-retry flow
Closes the learning loop and strengthens habit formation.

---

## 21. Later phases

## Phase 3
Focus on personalized learning depth:
- targeted drills
- weak-sound practice packs
- model pronunciation playback
- recommendations based on recurring mistakes

## Phase 4
Focus on broader learning modes:
- unscripted speaking
- richer coaching
- lesson paths
- advanced personalization

## Phase 5
Focus on scale and monetization:
- subscriptions
- teacher features
- team/classroom features
- mobile experiences

---

## 22. Product statement

**ClearSpeak** is a pronunciation practice app for **US English** learners.

Phase 1 established the core ability to read a known text aloud and receive clear pronunciation feedback.

Phase 2 should build on that completed foundation by adding the smallest, highest-ROI product features that make practice structured, repeatable, and measurable:
- a passage library
- saved attempts
- history
- progress tracking
- retry and compare workflows

That is the right next step for version **1.2**.
