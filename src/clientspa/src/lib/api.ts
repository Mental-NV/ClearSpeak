export type AnalyzePronunciationResponse = {
  scores: {
    pronunciation: number
    accuracy: number
    fluency: number
    completeness: number
    prosody: number
  }
  summary: string
  nextSteps: string[]
  recognizedText: string
  sessionId?: string
  words: AnalyzedWord[]
}

export type AnalyzedWord = {
  text: string
  score: number
  errorType: string
  feedback: string
  isFocusWord: boolean
  offsetTicks?: number
  durationTicks?: number
  phonemes: AnalyzedPhoneme[]
}

export type AnalyzedPhoneme = {
  phoneme: string
  score: number
  alternatives: PhonemeAlternative[]
}

export type PhonemeAlternative = {
  phoneme: string
  score: number
}

export type AnalyzeRequest = {
  text: string
  locale: string
  audioBlob: Blob
  filename: string
  onStatusChange?: (status: 'uploading' | 'analyzing') => void
}

type ProblemDetails = {
  title?: string
  detail?: string
}

export async function analyzePronunciation({
  text,
  locale,
  audioBlob,
  filename,
  onStatusChange,
}: AnalyzeRequest): Promise<AnalyzePronunciationResponse> {
  const formData = new FormData()
  formData.append('text', text)
  formData.append('locale', locale)
  formData.append('audio', audioBlob, filename)

  return new Promise<AnalyzePronunciationResponse>((resolve, reject) => {
    const xhr = new XMLHttpRequest()
    xhr.open('POST', '/api/pronunciation/analyze')
    onStatusChange?.('uploading')

    xhr.upload.onloadend = () => onStatusChange?.('analyzing')

    xhr.onerror = () => reject(new Error('The request failed before the server returned a response.'))
    xhr.onabort = () => reject(new Error('The request was canceled.'))
    xhr.onload = () => {
      if (xhr.status >= 200 && xhr.status < 300) {
        resolve(JSON.parse(xhr.responseText) as AnalyzePronunciationResponse)
        return
      }

      try {
        const problem = JSON.parse(xhr.responseText) as ProblemDetails
        reject(new Error(problem.detail ?? problem.title ?? 'Pronunciation analysis failed.'))
      } catch {
        reject(new Error('Pronunciation analysis failed.'))
      }
    }

    xhr.send(formData)
  })
}
