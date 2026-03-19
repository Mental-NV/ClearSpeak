import '@testing-library/jest-dom/vitest'

if (!globalThis.URL.createObjectURL) {
  globalThis.URL.createObjectURL = vi.fn(() => 'blob:mock-url')
}

if (!globalThis.URL.revokeObjectURL) {
  globalThis.URL.revokeObjectURL = vi.fn()
}

Object.defineProperty(HTMLMediaElement.prototype, 'play', {
  configurable: true,
  value: vi.fn(),
})
