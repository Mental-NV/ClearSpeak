import { useMemo, useRef, useState } from 'react'
import { analyzePronunciation, type AnalyzePronunciationResponse, type AnalyzedWord } from './lib/api'
import { useMediaRecorder } from './hooks/useMediaRecorder'
import { HighlightedTranscript } from './components/HighlightedTranscript'
import { ScoreCards } from './components/ScoreCards'
import { WordDetailsPanel } from './components/WordDetailsPanel'

const defaultText = `Three friends were walking through the narrow street when they heard a voice from the white house on the hill.
Wait for William, said Heather, or we'll miss the early train.`

type AnalysisStatus = 'idle' | 'uploading' | 'analyzing' | 'error'

function App() {
  const [text, setText] = useState(defaultText)
  const [analysis, setAnalysis] = useState<AnalyzePronunciationResponse | null>(null)
  const [analysisStatus, setAnalysisStatus] = useState<AnalysisStatus>('idle')
  const [errorMessage, setErrorMessage] = useState<string | null>(null)
  const [selectedWordIndex, setSelectedWordIndex] = useState<number>(0)

  const audioRef = useRef<HTMLAudioElement | null>(null)

  const recorder = useMediaRecorder()
  const currentStatus = useMemo(() => {
    if (analysisStatus === 'uploading' || analysisStatus === 'analyzing') {
      return analysisStatus
    }

    if (analysisStatus === 'error' || recorder.status === 'error') {
      return 'error'
    }

    return recorder.status
  }, [analysisStatus, recorder.status])

  const selectedWord: AnalyzedWord | null = analysis?.words[selectedWordIndex] ?? null
  const isBusy = analysisStatus === 'uploading' || analysisStatus === 'analyzing'
  const canAnalyze = text.trim().length > 0 && recorder.audioBlob !== null && !isBusy

  async function handleAnalyze() {
    if (!recorder.audioBlob || text.trim().length === 0) {
      return
    }

    setErrorMessage(null)
    setAnalysis(null)
    setSelectedWordIndex(0)

    try {
      const result = await analyzePronunciation({
        text,
        locale: 'en-US',
        audioBlob: recorder.audioBlob,
        filename: buildRecordingFilename(recorder.mimeType),
        onStatusChange: setAnalysisStatus,
      })

      setAnalysis(result)
      const nextSelectedIndex = result.words.findIndex((word) => word.isFocusWord)
      setSelectedWordIndex(nextSelectedIndex >= 0 ? nextSelectedIndex : 0)
      setAnalysisStatus('idle')
    } catch (error) {
      setAnalysisStatus('error')
      setErrorMessage(error instanceof Error ? error.message : 'Pronunciation analysis failed.')
    }
  }

  function handlePlay() {
    void audioRef.current?.play()
  }

  function handleRerecord() {
    recorder.resetRecording()
    setAnalysis(null)
    setAnalysisStatus('idle')
    setErrorMessage(null)
  }

  return (
    <main className="page-shell">
      <section className="hero-panel">
        <p className="eyebrow">US English pronunciation coach</p>
        <h1>ClearSpeak</h1>
        <p className="subtitle">Read text aloud and get pronunciation feedback.</p>
      </section>

      <section className="content-grid">
        <div className="panel">
          <div className="section-heading">
            <h2>Text to read</h2>
            <p>Edit the text if you want, then read it aloud when you're ready.</p>
          </div>
          <textarea
            aria-label="Text to read aloud"
            className="script-input"
            value={text}
            onChange={(event) => setText(event.target.value)}
            rows={7}
          />
          <p className="helper-note">Tip: speak at a steady pace and keep the microphone close enough for a clean recording.</p>
        </div>

        <div className="panel">
          <div className="section-heading">
            <h2>Recording</h2>
            <p>Record your voice, then listen back before you analyze it.</p>
          </div>
          <div className="recorder-controls">
            <button type="button" className="primary-button" onClick={() => void recorder.startRecording()} disabled={recorder.status === 'recording' || isBusy}>
              Record
            </button>
            <button type="button" className="secondary-button" onClick={recorder.stopRecording} disabled={recorder.status !== 'recording'}>
              Stop
            </button>
            <button type="button" className="secondary-button" onClick={handleRerecord} disabled={recorder.audioBlob === null && recorder.status !== 'error'}>
              Re-record
            </button>
            <button type="button" className="secondary-button" onClick={handlePlay} disabled={!recorder.audioUrl}>
              Play recording
            </button>
          </div>

          <div className="status-card" data-status={currentStatus}>
            <div>
              <span className="status-label">Status</span>
              <strong>{currentStatus}</strong>
            </div>
            <div>
              <span className="status-label">Timer</span>
              <strong>{formatDuration(recorder.durationSeconds)}</strong>
            </div>
          </div>

          {recorder.errorMessage ? <p className="error-banner">{recorder.errorMessage}</p> : null}
          {errorMessage ? <p className="error-banner">{errorMessage}</p> : null}
          <audio ref={audioRef} src={recorder.audioUrl ?? undefined} />

          <button type="button" className="analyze-button" onClick={() => void handleAnalyze()} disabled={!canAnalyze}>
            Analyze pronunciation
          </button>
        </div>
      </section>

      <section className="panel feedback-panel">
        <div className="section-heading">
          <h2>Feedback</h2>
          <p>{analysis ? analysis.summary : 'Record yourself, then send the audio to get feedback.'}</p>
        </div>

        {analysis ? (
          <>
            <ScoreCards scores={analysis.scores} />
            <div className="feedback-grid">
              <div>
                <HighlightedTranscript
                  words={analysis.words}
                  selectedWordIndex={selectedWordIndex}
                  onSelectWord={setSelectedWordIndex}
                />
                <div className="next-steps">
                  <h3>Next steps</h3>
                  <ul>
                    {analysis.nextSteps.map((step) => (
                      <li key={step}>{step}</li>
                    ))}
                  </ul>
                </div>
              </div>

              <WordDetailsPanel word={selectedWord} />
            </div>
          </>
        ) : (
          <div className="empty-state">
            <p>No feedback yet. The analysis response will show scores, weak words, phoneme detail, and a short practice plan.</p>
          </div>
        )}
      </section>
    </main>
  )
}

function buildRecordingFilename(mimeType: string | null): string {
  if (mimeType?.includes('mp4')) {
    return 'recording.m4a'
  }

  if (mimeType?.includes('wav')) {
    return 'recording.wav'
  }

  return 'recording.webm'
}

function formatDuration(durationSeconds: number): string {
  const minutes = Math.floor(durationSeconds / 60)
  const seconds = durationSeconds % 60
  return `${minutes}:${seconds.toString().padStart(2, '0')}`
}

export default App
