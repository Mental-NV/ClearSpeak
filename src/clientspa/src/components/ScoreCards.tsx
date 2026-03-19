type ScoreCardsProps = {
  scores: {
    pronunciation: number
    accuracy: number
    fluency: number
    completeness: number
    prosody: number
  }
}

const labels: Array<[keyof ScoreCardsProps['scores'], string]> = [
  ['pronunciation', 'Pronunciation'],
  ['accuracy', 'Accuracy'],
  ['fluency', 'Fluency'],
  ['completeness', 'Completeness'],
  ['prosody', 'Prosody'],
]

export function ScoreCards({ scores }: ScoreCardsProps) {
  return (
    <div className="score-grid">
      {labels.map(([key, label]) => (
        <article className="score-card" key={key}>
          <span>{label}</span>
          <strong>{scores[key].toFixed(1)}</strong>
        </article>
      ))}
    </div>
  )
}
