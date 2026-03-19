import type { AnalyzedWord } from '../lib/api'

type HighlightedTranscriptProps = {
  words: AnalyzedWord[]
  selectedWordIndex: number
  onSelectWord: (index: number) => void
}

export function HighlightedTranscript({
  words,
  selectedWordIndex,
  onSelectWord,
}: HighlightedTranscriptProps) {
  return (
    <div className="transcript-block">
      <h3>Highlighted text</h3>
      <p className="highlighted-text">
        {words.map((word, index) => (
          <button
            key={`${word.text}-${index}`}
            type="button"
            className={`word-chip ${getWordTone(word)} ${selectedWordIndex === index ? 'selected' : ''}`}
            onClick={() => onSelectWord(index)}
          >
            {word.text}
          </button>
        ))}
      </p>
    </div>
  )
}

function getWordTone(word: AnalyzedWord): string {
  if (word.errorType === 'Omission' || word.errorType === 'Insertion') {
    return 'tone-muted'
  }

  if (word.errorType !== 'None' || word.score < 65) {
    return 'tone-danger'
  }

  if (word.score < 82 || word.isFocusWord) {
    return 'tone-warning'
  }

  return 'tone-good'
}
