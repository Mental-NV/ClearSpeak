import type { AnalyzedWord } from '../lib/api'

type WordDetailsPanelProps = {
  word: AnalyzedWord | null
}

export function WordDetailsPanel({ word }: WordDetailsPanelProps) {
  if (!word) {
    return (
      <aside className="detail-panel">
        <h3>Word details</h3>
        <p>Select a highlighted word to inspect its score, issue type, and phoneme hints.</p>
      </aside>
    )
  }

  return (
    <aside className="detail-panel">
      <h3>Word details</h3>
      <dl className="detail-list">
        <div>
          <dt>Word</dt>
          <dd>{word.text}</dd>
        </div>
        <div>
          <dt>Score</dt>
          <dd>{word.score.toFixed(1)}</dd>
        </div>
        <div>
          <dt>Issue type</dt>
          <dd>{word.errorType}</dd>
        </div>
      </dl>
      <p className="detail-feedback">{word.feedback}</p>

      {word.phonemes.length > 0 ? (
        <div className="phoneme-list">
          <h4>Phoneme details</h4>
          <ul>
            {word.phonemes.map((phoneme) => (
              <li key={`${word.text}-${phoneme.phoneme}`}>
                <strong>/{phoneme.phoneme}/</strong> {phoneme.score.toFixed(1)}
                {phoneme.alternatives.length > 0 ? (
                  <span>
                    {' '}
                    Closest: {phoneme.alternatives.map((candidate) => `/${candidate.phoneme}/ (${candidate.score.toFixed(1)})`).join(', ')}
                  </span>
                ) : null}
              </li>
            ))}
          </ul>
        </div>
      ) : null}
    </aside>
  )
}
