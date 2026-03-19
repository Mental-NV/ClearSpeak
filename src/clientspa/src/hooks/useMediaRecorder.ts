import { useEffect, useMemo, useRef, useState } from 'react'

export type RecorderStatus =
  | 'idle'
  | 'requesting microphone'
  | 'recording'
  | 'recorded'
  | 'error'

export type UseMediaRecorderResult = {
  status: RecorderStatus
  durationSeconds: number
  audioBlob: Blob | null
  audioUrl: string | null
  mimeType: string | null
  errorMessage: string | null
  startRecording: () => Promise<void>
  stopRecording: () => void
  resetRecording: () => void
}

const mimeCandidates = ['audio/webm;codecs=opus', 'audio/mp4']

export function getPreferredMimeType(): string | undefined {
  if (typeof MediaRecorder === 'undefined') {
    return undefined
  }

  return mimeCandidates.find((candidate) => MediaRecorder.isTypeSupported(candidate))
}

export function useMediaRecorder(): UseMediaRecorderResult {
  const [status, setStatus] = useState<RecorderStatus>('idle')
  const [durationSeconds, setDurationSeconds] = useState(0)
  const [audioBlob, setAudioBlob] = useState<Blob | null>(null)
  const [mimeType, setMimeType] = useState<string | null>(null)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)

  const mediaRecorderRef = useRef<MediaRecorder | null>(null)
  const streamRef = useRef<MediaStream | null>(null)
  const chunksRef = useRef<Blob[]>([])
  const timerRef = useRef<number | null>(null)

  const audioUrl = useMemo(() => {
    if (!audioBlob) {
      return null
    }

    return URL.createObjectURL(audioBlob)
  }, [audioBlob])

  useEffect(() => {
    return () => {
      if (audioUrl) {
        URL.revokeObjectURL(audioUrl)
      }

      stopTracks(streamRef.current)
      if (timerRef.current !== null) {
        window.clearInterval(timerRef.current)
      }
    }
  }, [audioUrl])

  async function startRecording() {
    resetRecording()
    setStatus('requesting microphone')
    setErrorMessage(null)

    if (!navigator.mediaDevices?.getUserMedia) {
      setStatus('error')
      setErrorMessage('This browser does not support microphone recording.')
      return
    }

    try {
      const stream = await navigator.mediaDevices.getUserMedia({ audio: true })
      const preferredMimeType = getPreferredMimeType()
      const recorder = preferredMimeType
        ? new MediaRecorder(stream, { mimeType: preferredMimeType })
        : new MediaRecorder(stream)

      chunksRef.current = []
      streamRef.current = stream
      mediaRecorderRef.current = recorder
      setMimeType(preferredMimeType ?? recorder.mimeType ?? 'audio/webm')
      setDurationSeconds(0)

      recorder.ondataavailable = (event) => {
        if (event.data.size > 0) {
          chunksRef.current.push(event.data)
        }
      }

      recorder.onstop = () => {
        const nextMimeType = preferredMimeType ?? recorder.mimeType ?? 'audio/webm'
        setAudioBlob(new Blob(chunksRef.current, { type: nextMimeType }))
        setStatus('recorded')
        if (timerRef.current !== null) {
          window.clearInterval(timerRef.current)
          timerRef.current = null
        }

        stopTracks(streamRef.current)
      }

      recorder.start()
      setStatus('recording')
      timerRef.current = window.setInterval(() => {
        setDurationSeconds((previous) => previous + 1)
      }, 1000)
    } catch (error) {
      setStatus('error')
      setErrorMessage(getPermissionErrorMessage(error))
    }
  }

  function stopRecording() {
    const recorder = mediaRecorderRef.current
    if (!recorder || recorder.state === 'inactive') {
      return
    }

    recorder.stop()
  }

  function resetRecording() {
    if (mediaRecorderRef.current && mediaRecorderRef.current.state !== 'inactive') {
      mediaRecorderRef.current.stop()
    }

    if (timerRef.current !== null) {
      window.clearInterval(timerRef.current)
      timerRef.current = null
    }

    stopTracks(streamRef.current)
    mediaRecorderRef.current = null
    streamRef.current = null
    chunksRef.current = []
    setDurationSeconds(0)
    setAudioBlob(null)
    setMimeType(null)
    setErrorMessage(null)
    setStatus('idle')
  }

  return {
    status,
    durationSeconds,
    audioBlob,
    audioUrl,
    mimeType,
    errorMessage,
    startRecording,
    stopRecording,
    resetRecording,
  }
}

function stopTracks(stream: MediaStream | null) {
  stream?.getTracks().forEach((track) => track.stop())
}

function getPermissionErrorMessage(error: unknown): string {
  if (error instanceof DOMException && error.name === 'NotAllowedError') {
    return 'Microphone permission was denied. Allow access and try again.'
  }

  return 'Unable to access the microphone. Check browser permissions and device input settings.'
}
