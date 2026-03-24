# ClearSpeak Passage Library Documentation

## Purpose

The passage library is the structured content layer introduced for ClearSpeak v1.2. Its role is to provide learners with ready-to-practice reading passages that support repeatable pronunciation assessment, saved attempts, comparison across attempts, and progress tracking.

The library should function as a curated internal content system, not as an unbounded content repository.

## Product role

The passage library supports the primary practice loop:

1. select passage
2. read aloud
3. receive pronunciation feedback
4. retry the same passage
5. compare results over time

It also supports discovery, filtering, recommendation, and future curriculum features.

## Scope

The library contains curated passages for US English pronunciation practice.

Supported dimensions:
- difficulty
- pronunciation focus tags
- topic tags
- estimated reading duration
- featured/recommended status
- active/inactive publishing status

Out of scope for the current version:
- user-generated library entries
- content authoring UI
- teacher-managed libraries
- automatic passage generation
- lesson-plan sequencing logic

## Content model

Each passage should include the following fields.

### Required fields
- `id`: stable unique identifier
- `title`: learner-facing passage title
- `text`: full passage text
- `difficulty`: `Beginner`, `Intermediate`, or `Advanced`
- `estimatedDurationSeconds`: approximate reading duration
- `focusTags`: pronunciation-focused tags
- `topicTags`: browse-oriented topic tags
- `isActive`: whether the passage is available in the product

### Recommended fields
- `isFeatured`: whether the passage is prioritized in featured/recommended surfaces
- `sortOrder`: deterministic ordering value
- `description`: short learner-facing explanation
- `recommendedFor`: optional learner segment hint

## Difficulty model

The passage library uses three learner-facing difficulty levels:
- Beginner
- Intermediate
- Advanced

Difficulty should reflect pronunciation-practice complexity rather than general academic reading difficulty. It should be influenced by:
- passage length
- sentence complexity
- lexical familiarity
- density of consonant clusters
- rhythm and stress complexity
- concentration of target pronunciation challenges

## Tagging model

### Focus tags

Focus tags represent pronunciation targets and power filtering, recommendations, and progress summaries.

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

Topic tags support natural browsing and content discovery.

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

## Library organization

The library should be organized around a compact curated catalog rather than deep hierarchical navigation.

Recommended user-facing groupings:
- Getting started
- Sound practice
- Quick practice
- Challenge practice
- Recently practiced
- Recommended for you

These groupings may be produced by filtering and ranking existing passage records rather than by separate storage models.

## Retrieval and filtering requirements

The application should support the following retrieval capabilities:
- list all active passages
- filter by difficulty
- filter by focus tag
- filter by topic tag
- search by title and text preview metadata
- sort by recommendation, recency, duration, or difficulty-related ordering
- retrieve a single passage by id

## Recommendation behavior

The library should support lightweight recommendation logic.

### New learner recommendation behavior
- prioritize beginner passages
- prioritize short passages
- include at least one mixed-review passage
- include at least one single-focus passage

### Returning learner recommendation behavior
- prioritize recently practiced passages
- prioritize passages aligned to recurring weak focus tags
- provide at least one easier retry option
- provide at least one unattempted passage for variety

Recommendation logic should remain rule-based in the current phase.

## Persistence and relationships

The passage library should integrate with saved attempts and progress tracking.

### Core relationships
- one passage can have many attempts
- one user can attempt many passages
- progress views aggregate attempt data by passage and by focus tag

### Required linkage fields in attempt records
- `passageId` when the attempt comes from the library
- snapshot of reference text at attempt time
- optional passage title snapshot for display stability

The product should treat the passage text used during an attempt as immutable historical context even if the canonical passage is later edited.

## Versioning and editing policy

Passages may need minor editorial updates over time. The library should support safe evolution.

Recommended policy:
- preserve stable passage ids for non-breaking edits
- create a new passage id when the text changes materially enough to invalidate historical comparison
- never retroactively overwrite the reference text stored in historical attempts

## Publishing states

At minimum, passages should support:
- active
- inactive
- featured

Inactive passages should not appear in normal browse surfaces but historical attempts linked to them must remain readable.

## UI integration requirements

The passage library must support the following UI surfaces.

### Practice home
- featured/recommended passage cards
- filters for difficulty and focus tags
- recent passage resume card

### Practice workspace
- passage title
- passage metadata
- full text display
- retry of same passage

### History
- display passage title for saved attempts
- enable retry from a historical attempt

### Progress
- aggregate results by passage
- show best/latest score per passage
- associate weak patterns with passage focus tags

## Analytics requirements

The library should support the following product analytics events:
- passage viewed
- passage opened from library
- passage practice started
- passage attempt completed
- passage retried
- passage abandoned before submission
- filter usage
- recommendation click-through

These events should support later product analysis without requiring changes to the passage schema.

## Operational requirements

### Content storage
The initial library may be stored in a static JSON catalog or seed-backed database table. The runtime application should expose passage data through the backend API rather than embedding business logic in the frontend.

### API expectations
Minimum API support:
- `GET /api/passages`
- `GET /api/passages/{id}`

Recommended query support:
- difficulty
- focus tag
- topic tag
- search text
- featured only
- limit/offset or cursor pagination if needed later

## Non-functional requirements

The passage library should be:
- deterministic in ordering
- fast to retrieve
- simple to cache
- stable across app versions
- compatible with saved-attempt history and compare flows

## Future extensions

The data model should remain compatible with later additions such as:
- curriculum sequences
- adaptive recommendations
- teacher-assigned passage sets
- passage collections or playlists
- model pronunciation playback
- passage-level mastery indicators

These are future-facing extensions and are not required in the current library implementation.

## Summary

The ClearSpeak passage library is a curated, tagged catalog of pronunciation-practice passages that enables structured selection, repeat practice, saved history, and progress tracking. It should remain compact, consistent, and tightly integrated with the assessment workflow.
