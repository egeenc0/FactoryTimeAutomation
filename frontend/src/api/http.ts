import axios, { isAxiosError } from 'axios'

/**
 * Geliştirmede `VITE_API_BASE_URL` boş bırakılmalı; istekler `/api/...` olarak
 * Vite dev sunucusuna gider ve `vite.config.ts` proxy’si API’ye (varsayılan 5047) iletir.
 * Ayrı bir terminalde `dotnet run --project src/backend/Fabrika.Api.csproj` çalışmalıdır.
 * Üretimde veya API’ye doğrudan ihtiyaç varsa tam kök URL verin (örn. https://api.example.com).
 */
const baseURL = import.meta.env.VITE_API_BASE_URL?.trim() || ''

export const http = axios.create({
  baseURL: baseURL.length > 0 ? baseURL : undefined,
  /** Sonsuz "Gönderiliyor…" ve takılı proxy isteklerini sınırlar. */
  timeout: 25_000,
})

/** Ağ / bağlantı hatalarında kullanıcıya gösterilecek kısa açıklama. */
export function apiErisimHataMetni(hata: unknown): string {
  if (!isAxiosError(hata)) {
    return ''
  }
  const e = hata
  if (e.code === 'ECONNABORTED' || e.message?.toLowerCase().includes('timeout')) {
    return 'Sunucu zaman aşımı. API ayakta mı kontrol edin.'
  }
  if (e.code === 'ECONNREFUSED' || e.message === 'Network Error') {
    return 'API yanıt vermiyor (port 5047). Ayrı bir terminalde backend’i çalıştırın: dotnet run --project src/backend/Fabrika.Api.csproj'
  }
  return ''
}

/**
 * 502/500/404 gibi tüm axios yanıtlarında anlamlı mesaj; proxy/API debug için.
 */
export function kullaniciHataOzet(hata: unknown): string {
  const ag = apiErisimHataMetni(hata)
  if (ag) {
    return ag
  }
  if (isAxiosError(hata)) {
    const st = hata.response?.status
    if (st === 502 || st === 503) {
      return 'Arayüz (Vite) API’ye ulaşamadı. Backend açık mı? (dotnet run — http://localhost:5047).'
    }
    if (st === 500) {
      const raw = hata.response?.data
      if (typeof raw === 'string' && raw.length) {
        const t = raw.replace(/<[^>]+>/g, ' ').replace(/\s+/g, ' ').trim().slice(0, 160)
        if (t) {
          return `Sunucu hatası (500). Ayrıntı: ${t}${raw.length > 160 ? '…' : ''}`
        }
      }
      return 'Sunucu hatası (500). SQL bağlantısı, migration ve API konsol çıktısına bakın.'
    }
    if (st === 404) {
      return 'İstek yolu bulunamadı (404). API güncel mi, adres doğru mu?'
    }
    if (st === 405) {
      return 'HTTP 405: Yöntem bu adreste izinli değil. Eski sürüm API veya proxy cache olabilir; backend’i yeniden derleyip çalıştırın.'
    }
    if (hata.response?.data) {
      const d = hata.response.data
      if (typeof d === 'object' && d !== null) {
        const o = d as { error?: string; title?: string; detail?: string; message?: string }
        if (o.error) {
          return o.error
        }
        if (o.title || o.detail) {
          return [o.title, o.detail].filter(Boolean).join(' — ')
        }
        if (o.message) {
          return String(o.message)
        }
      }
    }
    if (st) {
      return `İstek başarısız (HTTP ${st}).`
    }
    if (hata.request && hata.response === undefined) {
      return 'Ağ: Sunucu yanıtı yok. API çalışıyor mu, proxy/URL doğru mu kontrol edin.'
    }
  }
  if (hata instanceof Error) {
    return hata.message
  }
  return 'Beklenmeyen hata.'
}
