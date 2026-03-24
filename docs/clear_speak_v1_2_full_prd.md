# ClearSpeak v1.2 — Full Product Requirements Document

**Product:** ClearSpeak  
**Version:** 1.2  
**Status:** PRD for implementation  
**Baseline shipped version:** 1.1  
**Target market:** US English pronunciation learners

---

## 1. Document purpose

This PRD defines the full product requirements for **ClearSpeak v1.2**.

It is intentionally constrained to the **approved v1.2 scope only**. It does not include later-phase features such as adaptive curricula, advanced lesson plans, teacher tooling, free-speaking mode, native-audio comparison, or broad AI coaching.

The purpose of v1.2 is to convert ClearSpeak from a one-off pronunciation checker into a compact, repeatable pronunciation practice product built around structured passages, saved attempts, history, progress visibility, and retry/compare workflows.

---

## 2. Product summary

ClearSpeak is a web app for learners of **US English** who want fast, practical, confidence-building pronunciation feedback while reading known text aloud.

Version 1.1 established the core loop:
1. enter or choose text
2. record speech in the browser
3. submit text and audio to backend assessment
4. receive structured pronunciation feedback

Version 1.2 extends that loop with continuity features that make repeated practice valuable:
1. choose a passage from a curated library
2. complete a pronunciation attempt
3. save the result automatically
4. revisit prior attempts
5. compare attempts on the same passage
6. view simple progress signals over time

---

## 3. Problem statement

The shipped product already validates the technical assessment loop, but the current learning experience is disposable. A learner can get feedback once, but cannot easily build habit, review prior work, or tell whether they are improving.

The primary product gap is **learning continuity**, not additional analysis sophistication.

Without continuity features:
- learners face a blank-start problem when choosing what to read
- attempts disappear after feedback
- there is no durable learning record
- there is no progress visibility
- repeat practice on the same material is friction-heavy

v1.2 addresses those gaps directly.

---

## 4. Product goal

Help a learner improve **US English pronunciation** through a tight and repeatable loop:
- select a passage or use custom text
- read aloud
- receive clear feedback
- retry quickly
- review saved attempts
- see visible improvement over time

---

## 5. v1.2 success definition

v1.2 is successful when a learner can:
1. choose a passage from a library
2. record and analyze pronunciation
3. save the attempt automatically
4. revisit past attempts
5. see whether they are improving
6. retry the same passage with low friction

At that point, ClearSpeak becomes a learning product rather than only a pronunciation checker.

---

## 6. Baseline assumptions from v1.1

The following capabilities already exist and are treated as implementation baseline rather than new v1.2 scope:
- single-page pronunciation practice workflow
- editable custom reference text
- browser microphone recording
- replay and re-record before submission
- backend pronunciation assessment pipeline
- result screen with overall scores
- word-level issue feedback
- learner-friendly summary and next steps
- React + TypeScript + Vite frontend
- ASP.NET Core Minimal API backend
- Azure Speech provider support
- deterministic fake provider for development/testing

v1.2 builds on top of this foundation.

---

## 7. Target user

### Primary user
An English learner who wants to sound clearer, more natural, and more confident in **spoken American English**.

### User characteristics
- can read short English passages
- is not looking for academic phonetics study
- wants practical feedback on what to improve next
- prefers short, repeatable practice sessions
- benefits from visible evidence of improvement

### Core user motivation
The user wants a lightweight practice tool that answers:
- What should I read?
- What did I say poorly?
- What should I fix next?
- Am I improving?

---

## 8. Product principles

1. **Practice first**  
   ClearSpeak exists to support doing pronunciation practice, not browsing complexity.

2. **Immediate feedback**  
   The path from recording to result must remain fast.

3. **Actionable correction**  
   Feedback must translate scores into clear next actions.

4. **Low friction**  
   The app should stay lightweight enough to start practicing within seconds.

5. **Progress visibility**  
   The next layer of value after assessment is showing whether the learner is improving.

6. **Continuity over complexity**  
   v1.2 should prefer saved attempts, comparison, and progress over ambitious new teaching systems.

---

## 9. In-scope features for v1.2

v1.2 includes only the following feature areas:

1. **Passage library**
2. **Saved attempts**
3. **History**
4. **Basic progress tracking**
5. **Retry and compare workflow**
6. **Settings page sufficient for v1.2 app shell needs**
7. **Continuation of quick practice with custom text, with saved attempt support**

---

## 10. Explicitly out of scope for v1.2

The following must not be added to v1.2 unless strictly required to support the approved scope:
- advanced lesson plans
- adaptive curriculum logic
- spaced repetition systems
- teacher dashboard or classroom tooling
- community/social features
- subscriptions and billing
- dedicated mobile app
- multi-locale support beyond `en-US`
- free-speaking or unscripted speaking mode
- native reference audio comparison
- TTS-based model pronunciation playback
- rich waveform scrubbing and advanced audio overlays
- LLM-generated coaching as a broad product layer
- heavy analytics dashboards
- deep account/profile systems beyond lightweight identification needed for persistence
- content authoring UI for passages
- user-generated passage libraries

---

## 11. Core jobs to be done

### Job 1 — Start structured practice quickly
When I open the app, I want to choose a useful passage quickly so I can start practicing without deciding what to read.

### Job 2 — Save my practice automatically
When I finish an assessment, I want the result saved so that practice is not disposable.

### Job 3 — Review previous work
When I come back later, I want to find my previous attempts so I can continue where I left off.

### Job 4 — See whether I am improving
When I practice repeatedly, I want a simple view of score trends and recurring problems.

### Job 5 — Retry the same passage easily
After seeing feedback, I want to retry immediately without resetting the whole workflow.

### Job 6 — Compare attempts
When I retry a passage, I want to compare the latest attempt with a previous one so I can see what improved and what still needs work.

---

## 12. User stories

### Passage library
- As a learner, I want to browse a curated set of passages so that I do not need to invent practice text every time.
- As a learner, I want to filter passages by difficulty and pronunciation focus so that I can choose material appropriate to my level.
- As a learner, I want to resume a recently practiced passage so that returning practice is fast.

### Practice and save
- As a learner, I want my result saved automatically after analysis so that I can review it later.
- As a learner, I want custom text attempts saved too so that ad hoc practice is still durable.

### History
- As a learner, I want to see my past attempts with date, score, and passage so that I can find relevant prior practice.
- As a learner, I want to retry from history so that I can continue work on the same text quickly.

### Progress
- As a learner, I want to see a simple score trend so that I can judge whether I am improving.
- As a learner, I want to see recurring weak words or focus areas so that I know where to concentrate.

### Compare
- As a learner, I want to compare one attempt against another on the same passage so that I can see what improved and what remains weak.

---

## 13. End-to-end user flows

### Flow A — Practice from library
1. User opens the app.
2. App routes to `/practice`.
3. User sees recommended/resume content and passage library.
4. User filters or searches passages if needed.
5. User opens a passage.
6. User records speech and submits for analysis.
7. App displays feedback.
8. Attempt is saved automatically.
9. User chooses either retry, compare, history, or progress.

### Flow B — Quick practice with custom text
1. User opens practice area.
2. User enters or edits custom text.
3. User records and analyzes.
4. Result is displayed.
5. Attempt is saved as a custom-text attempt.
6. User can retry the same text or revisit it later from history.

### Flow C — Review history
1. User opens `/attempts`.
2. User browses or filters prior attempts.
3. User opens an attempt detail page.
4. User reviews result details.
5. User retries the same text or opens compare.

### Flow D — Compare attempts
1. User completes or opens a recent attempt.
2. User clicks compare.
3. App chooses a baseline prior attempt on the same passage, or lets the user select one.
4. User sees score deltas and changed weak-word patterns.
5. User decides to retry again or return to history.

### Flow E — Review progress
1. User opens `/progress`.
2. User sees overall trend and passage-level progress summaries.
3. User sees recurring weak words and/or focus tags.
4. User jumps back into targeted practice.

---

## 14. Information architecture

### Top-level navigation
- **Practice**
- **History**
- **Progress**
- **Settings**

### Core routes
- `/` → redirect to `/practice`
- `/practice` → practice home / passage library / quick-start surface
- `/practice/:passageId` → passage practice workspace
- `/attempts` → attempt history list
- `/attempts/:attemptId` → attempt detail page
- `/compare/:attemptId?baseline=:baselineId` → compare view
- `/progress` → progress summary page
- `/settings` → app settings relevant to v1.2

### Navigation intent
The app should remain a small multi-screen practice product, not a broad platform.

---

## 15. Functional requirements

## 15.1 Practice home / passage library

### Purpose
Provide the main entry point for structured pronunciation practice.

### Required capabilities
- show featured, recommended, or resume-oriented passage content
- list active passages from backend API
- support search by passage title and preview metadata
- support filtering by difficulty
- support filtering by pronunciation focus tag
- support sorting by recommendation, recency, shortest duration, or similar lightweight ordering
- show recent score and attempt count when available
- allow opening a passage into the practice workspace
- provide a way to continue the last practiced passage

### Required UI elements
- page title and subtitle
- recommended/resume card
- search field
- difficulty filter
- focus filter
- sort dropdown
- passage cards or rows
- empty state when filters return no matches
- error state when passage retrieval fails

### Passage card minimum data
- title
- preview snippet or description
- estimated reading time
- difficulty
- focus tags
- attempts count if attempted before
- last score if previously attempted
- CTA to open

### Business rules
- only active passages appear in the normal library list
- inactive passages remain accessible indirectly through history if previously attempted
- recommendation logic in v1.2 must remain rule-based, not adaptive ML-driven

---

## 15.2 Passage practice workspace

### Purpose
Serve as the primary work surface for reading, recording, analyzing, and immediately retrying.

### Required capabilities
- show passage title and metadata
- show full passage text
- allow browser recording
- allow stop, play, and re-record
- allow analyze action once valid audio exists
- display immediate assessment result
- save attempt automatically after successful assessment response is received
- offer retry action for the same passage
- offer compare action when a valid baseline exists

### Required UI areas
- breadcrumb or contextual header
- passage metadata section
- passage text card
- recorder panel
- score summary area
- highlighted passage / word result area
- word detail panel
- next steps panel
- saved banner or confirmation state
- compare / retry actions in result view

### Baseline preservation requirement
The v1.2 workspace must preserve the core v1.1 strengths:
- fast path from recording to feedback
- simple controls
- learner-friendly summary
- word-level issue inspection

### Save trigger requirement
A successful assessment must create a saved attempt record automatically without requiring an explicit separate “Save” action.

---

## 15.3 Custom text practice

### Purpose
Preserve the original flexibility of ClearSpeak while integrating it into the saved-attempt model.

### Required capabilities
- allow user-entered custom text for practice
- submit custom text through the same pronunciation pipeline
- save custom-text attempts with the same durable result structure used for passage attempts
- label custom-text attempts clearly in history and progress views
- allow retry of the same custom text from historical attempt detail

### Constraints
- custom text remains supported
- v1.2 does not require advanced organization or naming of custom text beyond basic labeling and history display

---

## 15.4 Saved attempts

### Purpose
Persist each completed assessment outcome as durable learning data.

### Required stored fields
Each attempt record must contain, at minimum:
- unique attempt id
- user identity or lightweight profile id
- passage id when sourced from the library, otherwise null or custom-text indicator
- passage title snapshot when available
- reference text snapshot
- recognized text snapshot if available from provider output
- top-level scores
- normalized word-level results
- created timestamp

### Optional-but-allowed field
- audio retention metadata or retained audio reference

### Non-negotiable persistence rule
Even if canonical passage content later changes, the attempt must preserve the exact reference text snapshot used at attempt time.

### Save behavior
- attempts save automatically after successful analysis
- failed assessments must not create complete saved attempts
- partial/failed uploads may be logged operationally but must not appear as learner-visible saved attempts

---

## 15.5 History page

### Purpose
Give the learner a durable list of previous practice attempts.

### Required capabilities
- list prior attempts newest first by default
- support browsing across both passage attempts and custom-text attempts
- show the key summary data needed to identify a useful prior attempt
- support opening full attempt detail
- support retry action from history list or detail page
- support filtering or narrowing result set sufficiently for v1.2 usability

### Required history row fields
- attempt date/time
- passage title or custom-text label
- pronunciation score
- top problem summary
- retry action

### Minimum filters
- passage vs custom-text type or equivalent discriminator
- date or recency-based ordering
- lightweight text matching where feasible

### Empty state
When no attempts exist, history must explain that the learner has not saved any practice yet and provide a CTA back to practice.

---

## 15.6 Attempt detail page

### Purpose
Let the learner inspect a single saved attempt in a stable, reviewable format.

### Required capabilities
- display top-level scores
- display saved summary and next steps
- display highlighted reference text and word-level issues
- display attempt metadata such as date/time and passage label
- expose retry action
- expose compare action when valid baseline candidates exist

### Required error handling
- missing attempt id returns not-found state
- attempts tied to inactive passages must remain viewable

---

## 15.7 Compare attempts

### Purpose
Show improvement or regression between two attempts on the same passage.

### Required capabilities
- compare a selected attempt with a baseline attempt
- support direct route access with `attemptId` and `baselineId`
- show score deltas for relevant top-level metrics
- show which words improved and which remain weak
- show enough context for the learner to understand the change
- allow changing the baseline when multiple prior attempts exist

### Baseline rules
- comparison should only be available for attempts with compatible text context
- default baseline should typically be the most recent earlier attempt on the same passage
- if there is no valid earlier attempt, the UI must show an insufficient-data state rather than a broken compare experience

### Required compare output
- pronunciation delta
- other score deltas where available
- improved words panel
- remaining issues panel
- direct retry CTA

---

## 15.8 Progress page

### Purpose
Answer the learner’s core retention question: “Am I improving?”

### Required capabilities
- show overall pronunciation trend over time
- show best and latest score per passage where possible
- show recurring weak words
- show recurring weak sound/focus tags when the data model supports it
- support drilling back into practice from progress surfaces

### Constraints
- progress must remain simple and motivating
- it must not become a heavy analytics dashboard in v1.2
- it must remain understandable to a non-expert learner

### Minimum progress modules
- KPI strip or summary cards
- trend chart or trend summary
- passage-level progress table/list
- recurring weak words card

### Empty / low-data state
If the learner has too few attempts for meaningful trends, the page must state that clearly and direct them back to practice.

---

## 15.9 Settings page

### Purpose
Provide the small set of supporting settings needed for the v1.2 app shell and practice reliability.

### Required capabilities
- microphone test or microphone status support
- privacy/data messaging relevant to attempt persistence
- lightweight account state or user identity placeholder if required by implementation

### Constraints
- settings remain minimal
- settings must not expand into broad account-management scope

---

## 16. Passage library requirements

## 16.1 Library role
The passage library is a curated internal content system that supports structured pronunciation practice, saved attempts, recommendations, repeat practice, and progress tracking.

It must not behave like an unbounded content platform in v1.2.

## 16.2 Passage data model

### Required fields
- `id`
- `title`
- `text`
- `difficulty`
- `estimatedDurationSeconds`
- `focusTags`
- `topicTags`
- `isActive`

### Recommended fields
- `isFeatured`
- `sortOrder`
- `description`
- `recommendedFor`

## 16.3 Difficulty model
Supported learner-facing difficulty values:
- `Beginner`
- `Intermediate`
- `Advanced`

Difficulty should reflect pronunciation-practice complexity, influenced by:
- passage length
- sentence complexity
- lexical familiarity
- density of consonant clusters
- rhythm and stress complexity
- concentration of target pronunciation challenges

## 16.4 Controlled vocabularies

### Focus tags
Recommended controlled vocabulary:
- `TH`
- `W vs V`
- `R vs L`
- `Long vs short vowels`
- `Word stress`
- `Sentence stress`
- `Rhythm`
- `Consonant clusters`
- `Mixed review`

### Topic tags
Recommended controlled vocabulary:
- `Daily life`
- `Travel`
- `Work`
- `School`
- `Food`
- `Story`
- `Conversation`
- `Presentation`
- `Home`
- `Review`

## 16.5 Retrieval requirements
The backend must support:
- list all active passages
- retrieve passage by id
- filter by difficulty
- filter by focus tag
- filter by topic tag
- search by title and preview metadata
- sort by recommendation, recency proxy, duration, or difficulty-related order

## 16.6 Recommendation behavior
Recommendation logic in v1.2 must remain rule-based.

### New learner recommendations
- prioritize beginner passages
- prioritize short passages
- include at least one mixed-review passage
- include at least one single-focus passage

### Returning learner recommendations
- prioritize recently practiced passages
- prioritize passages aligned to recurring weak focus tags
- provide at least one easier retry option
- provide at least one unattempted passage for variety

## 16.7 Publishing states
At minimum, passages must support:
- active
- inactive
- featured

## 16.8 Versioning rules
- preserve stable passage ids for non-breaking edits
- create a new passage id when the text changes materially enough to invalidate historical comparison
- never retroactively overwrite attempt snapshots

## 16.9 Operational constraints
- library may initially come from static JSON or seeded database records
- runtime access must be provided through backend APIs, not hardcoded frontend business logic
- ordering should be deterministic
- retrieval should be fast and cache-friendly

---

## 17. Data requirements

## 17.1 Attempt entity
Minimum conceptual fields:
- `id`
- `userId`
- `passageId` nullable
- `attemptType` (`Passage` or `CustomText`)
- `passageTitleSnapshot` nullable
- `referenceTextSnapshot`
- `recognizedTextSnapshot` nullable
- `scores`
- `summary`
- `nextSteps`
- `wordResults`
- `createdAt`

## 17.2 Word result structure
Each saved word result should preserve enough information for attempt detail and compare views. Minimum fields:
- word text
- score
- error type
- learner-facing feedback
- optional positional or phoneme detail when available

## 17.3 Passage relationships
- one passage has many attempts
- one learner has many attempts
- progress views aggregate by learner, passage, and focus tag

## 17.4 Historical integrity
Attempt records are historical artifacts and must remain stable even if:
- passage metadata is later edited
- passages are deactivated
- recommendation rules change

---

## 18. Backend and API requirements

## 18.1 General backend responsibilities
The backend must support the existing pronunciation assessment pipeline plus new v1.2 responsibilities:
- passage catalog retrieval
- attempt persistence
- attempt listing and filtering
- attempt detail retrieval
- compare-ready data retrieval or compare endpoint support
- aggregate progress retrieval

## 18.2 Recommended API surface
- `GET /api/passages`
- `GET /api/passages/{id}`
- `POST /api/pronunciation/analyze`
- `GET /api/attempts`
- `GET /api/attempts/{id}`
- `GET /api/attempts/{id}/compare?baselineId=...`
- `GET /api/progress`
- `GET /api/progress/passages`

## 18.3 Passage API requirements
`GET /api/passages` should support lightweight query parameters for:
- difficulty
- focus tag
- topic tag
- search text
- featured-only filtering
- pagination support if needed later

Topic tag filtering is **backend-supported in v1.2** so that the data model and API remain future-ready, but it is **not required to be surfaced in the v1.2 user interface**.

## 18.4 Assessment submission requirements
Assessment submission must continue to use the existing `POST /api/pronunciation/analyze` endpoint and must:
- accept text and recorded audio
- optionally accept passage metadata such as `passageId` when the attempt comes from the library
- run the pronunciation pipeline
- return immediate response data for the current attempt
- persist saved-attempt data on success so the result is available through `/api/attempts`

## 18.5 Compare requirements
The backend may either:
- provide a dedicated compare endpoint, or
- provide sufficient attempt detail data for frontend comparison

The implementation must still support:
- score deltas
- word-level improvement vs remaining-issue analysis
- validation that compared attempts are compatible

## 18.6 Progress requirements
Progress endpoints must support:
- aggregate score trend data
- passage-level latest/best summaries
- recurring weak-word aggregation
- focus-tag aggregation where available

---

## 19. UX requirements and screen behavior

## 19.1 Global layout
Desktop-first, with mobile fallback.

### App shell regions
- top bar with product branding and quick start action
- optional left navigation on wide screens
- centered content region with readable width

### Mobile behavior
- stacked panels or collapsed navigation
- no loss of core functionality

## 19.2 Shared states
All major screens must support:
- loading state
- empty state where relevant
- error state

Critical flows must support explicit error handling for:
- microphone permission failure
- upload/network error
- unsupported audio state
- backend timeout or provider failure
- no recognition / low-quality recording

## 19.3 State matrix expectations

### Practice home
- loading library
- loaded with recommendations
- filtered results
- empty results
- library fetch error

### Practice workspace
- initial
- recording
- recorded
- analyzing
- results loaded
- save confirmation
- mic error
- provider error

### History
- loading
- populated
- empty
- filter no-match
- load error

### Attempt detail
- loading
- loaded
- not found
- deleted or missing reference context handled gracefully

### Compare
- loading
- ready
- insufficient attempts
- selector changed
- compare error

### Progress
- loading
- enough data
- too little data
- load error

---

## 20. Analytics requirements

v1.2 should emit enough analytics to measure product value and support iteration.

### Passage/library events
- passage viewed
- passage opened from library
- passage practice started
- passage attempt completed
- passage retried
- passage abandoned before submission
- filter usage
- recommendation click-through

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

## 21. Non-functional requirements

### Performance
- passage retrieval should feel fast
- history and progress should load within normal web-app expectations for compact data views
- comparison should not require heavy processing visible to the learner

### Reliability
- saved attempts must be durable once confirmation is shown
- assessment failures must surface clearly
- historical attempts must remain readable even if linked passages change state

### Data consistency
- attempt snapshots are immutable historical records
- compare logic must not silently compare incompatible text contexts
- passage ordering should be deterministic

### Simplicity
- v1.2 should remain a compact app
- information density should stay learner-friendly
- progress views must avoid expert-only jargon overload

### Compatibility
- solution must remain compatible with later personalization layers, but must not implement them now

---

## 22. Acceptance criteria by feature area

## 22.1 Passage library
v1.2 passes this area when:
- active passages can be listed from backend
- learner can filter by difficulty and focus tags
- learner can open a passage into practice
- returning learner can resume a recent passage
- no-match empty state is handled cleanly

## 22.2 Saved attempts
v1.2 passes this area when:
- successful assessments create saved attempt records automatically
- both passage-based and custom-text attempts are saved
- saved attempts preserve reference text snapshot and score/result data

## 22.3 History
v1.2 passes this area when:
- learner can open a history list of prior attempts
- each attempt row shows identifying metadata and score
- learner can open detail and retry from history

## 22.4 Progress
v1.2 passes this area when:
- learner can view a simple score trend
- learner can view passage-level latest/best summaries
- learner can view recurring weak words and/or focus areas
- low-data states are handled clearly

## 22.5 Compare and retry
v1.2 passes this area when:
- learner can retry the same passage immediately after feedback
- learner can compare latest attempt to a prior compatible attempt
- compare view shows clear improvement and remaining-issue signals

---

## 23. Delivery priorities

If implementation capacity is constrained, build in this order:
1. passage library
2. saved attempts
3. history list
4. attempt detail
5. compare workflow
6. progress summary
7. settings polish

This preserves the highest ROI path for v1.2.

---

## 24. Final product statement

**ClearSpeak v1.2** is a compact web app for **US English pronunciation practice**.

It builds on the shipped v1.1 pronunciation-checking loop and adds the minimum high-value features needed to make practice structured, durable, and measurable:
- curated passage selection
- automatic saved attempts
- history review
- simple progress visibility
- retry and compare workflows

The defining requirement of v1.2 is not breadth. It is turning one-off pronunciation checks into a repeatable improvement loop.
