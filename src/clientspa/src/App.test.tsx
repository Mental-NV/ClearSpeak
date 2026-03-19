import { fireEvent, render, screen, waitFor } from '@testing-library/react'
import App from './App'

const mockGetUserMedia = vi.fn()

class MockMediaRecorder {
  public static isTypeSupported(type: string) {
    return type === 'audio/webm;codecs=opus' || type === 'audio/mp4'
  }

  public mimeType: string
  public state: 'inactive' | 'recording' = 'inactive'
  public ondataavailable: ((event: BlobEvent) => void) | null = null
  public onstop: (() => void) | null = null

  public constructor(_stream: MediaStream, options?: MediaRecorderOptions) {
    this.mimeType = options?.mimeType ?? 'audio/webm'
  }

  public start() {
    this.state = 'recording'
  }

  public stop() {
    this.state = 'inactive'
    this.ondataavailable?.({ data: new Blob(['audio'], { type: this.mimeType }) } as BlobEvent)
    this.onstop?.()
  }
}

class MockXmlHttpRequest {
  public static responseBody = JSON.stringify({
    scores: {
      pronunciation: 76,
      accuracy: 72,
      fluency: 81,
      completeness: 100,
      prosody: 68,
    },
    summary: 'Your speech was understandable, but a few words need correction.',
    nextSteps: ['Practice the word \'three\' several times.', 'Repeat the first sentence more smoothly.'],
    recognizedText: 'Three friends were walking through the narrow street.',
    words: [
      {
        text: 'Three',
        score: 54,
        errorType: 'Mispronunciation',
        feedback: 'The first sound should be /θ/, not /t/.',
        isFocusWord: true,
        phonemes: [{ phoneme: 'θ', score: 41, alternatives: [] }],
      },
      {
        text: 'friends',
        score: 90,
        errorType: 'None',
        feedback: "'friends' is strong.",
        isFocusWord: false,
        phonemes: [],
      },
    ],
  })

  public static statusCode = 200
  public upload = { onloadend: null as (() => void) | null }
  public onload: (() => void) | null = null
  public onerror: (() => void) | null = null
  public onabort: (() => void) | null = null
  public status = 0
  public responseText = ''

  open() {}

  send() {
    this.upload.onloadend?.()
    this.status = MockXmlHttpRequest.statusCode
    this.responseText = MockXmlHttpRequest.responseBody
    this.onload?.()
  }
}

beforeEach(() => {
  vi.stubGlobal('MediaRecorder', MockMediaRecorder)
  vi.stubGlobal('XMLHttpRequest', MockXmlHttpRequest)
  Object.defineProperty(navigator, 'mediaDevices', {
    configurable: true,
    value: {
      getUserMedia: mockGetUserMedia,
    },
  })

  mockGetUserMedia.mockResolvedValue({
    getTracks: () => [{ stop: vi.fn() }],
  })
  MockXmlHttpRequest.statusCode = 200
})

afterEach(() => {
  vi.restoreAllMocks()
})

test('renders the default text sample', () => {
  render(<App />)

  expect(screen.getByLabelText(/text to read aloud/i)).toHaveValue(
    "Three friends were walking through the narrow street when they heard a voice from the white house on the hill.\nWait for William, said Heather, or we'll miss the early train.",
  )
})

test('enables analyze after a recording exists', async () => {
  render(<App />)

  const analyzeButton = screen.getByRole('button', { name: /analyze pronunciation/i })
  expect(analyzeButton).toBeDisabled()

  fireEvent.click(screen.getByRole('button', { name: /^record$/i }))
  await waitFor(() => expect(screen.getByRole('button', { name: /^stop$/i })).toBeEnabled())
  fireEvent.click(screen.getByRole('button', { name: /^stop$/i }))

  await waitFor(() => expect(analyzeButton).toBeEnabled())
})

test('displays returned feedback after analysis', async () => {
  render(<App />)

  fireEvent.click(screen.getByRole('button', { name: /^record$/i }))
  await waitFor(() => expect(screen.getByRole('button', { name: /^stop$/i })).toBeEnabled())
  fireEvent.click(screen.getByRole('button', { name: /^stop$/i }))
  await waitFor(() => expect(screen.getByRole('button', { name: /analyze pronunciation/i })).toBeEnabled())
  fireEvent.click(screen.getByRole('button', { name: /analyze pronunciation/i }))

  expect(await screen.findByText(/your speech was understandable/i)).toBeInTheDocument()
  expect(screen.getByText('Pronunciation')).toBeInTheDocument()
  expect(screen.getByRole('button', { name: 'Three' })).toBeInTheDocument()
})

test('shows a microphone permission error', async () => {
  mockGetUserMedia.mockRejectedValueOnce(new DOMException('Denied', 'NotAllowedError'))
  render(<App />)

  fireEvent.click(screen.getByRole('button', { name: /^record$/i }))

  expect(await screen.findByText(/microphone permission was denied/i)).toBeInTheDocument()
})

test('shows an analysis error when the API fails', async () => {
  MockXmlHttpRequest.statusCode = 503
  MockXmlHttpRequest.responseBody = JSON.stringify({
    title: 'Pronunciation provider is not configured',
    detail: 'Azure endpoint is required.',
  })

  render(<App />)

  fireEvent.click(screen.getByRole('button', { name: /^record$/i }))
  await waitFor(() => expect(screen.getByRole('button', { name: /^stop$/i })).toBeEnabled())
  fireEvent.click(screen.getByRole('button', { name: /^stop$/i }))
  await waitFor(() => expect(screen.getByRole('button', { name: /analyze pronunciation/i })).toBeEnabled())
  fireEvent.click(screen.getByRole('button', { name: /analyze pronunciation/i }))

  expect(await screen.findByText(/azure endpoint is required/i)).toBeInTheDocument()
})
