/// <reference types="vite/client" />

interface ImportMetaEnv {
  /** Boş bırakılırsa istekler aynı origin üzerinden gider (Vite `proxy` ile `/api`). */
  readonly VITE_API_BASE_URL?: string
}

interface ImportMeta {
  readonly env: ImportMetaEnv
}
