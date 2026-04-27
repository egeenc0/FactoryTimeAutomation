<template>
  <main class="sayfa">
    <div class="ust-satir">
      <h1 class="baslik">Üretim kayıtları</h1>
      <div class="ust-aksiyon">
        <button type="button" class="btn-yenile" :disabled="yukleniyor" @click="yukle">Yenile</button>
        <RouterLink to="/" class="link">← Çalışan girişi</RouterLink>
      </div>
    </div>

    <p v-if="hata" class="hata" role="alert">{{ hata }}</p>
    <pre v-if="hataAyrinti" class="ayrinti">{{ hataAyrinti }}</pre>
    <p v-else-if="yukleniyor && satirlar.length === 0" class="yukleniyor-tek">Yükleniyor…</p>
    <p v-else-if="!yukleniyor && satirlar.length === 0" class="bos">Henüz kayıt yok.</p>

    <div v-else class="tablo-wrap">
      <table class="grid">
        <thead>
          <tr>
            <th>Id</th>
            <th>Tarih (UTC)</th>
            <th>Çalışan</th>
            <th>Veri</th>
            <th>Makine</th>
            <th>Ad</th>
            <th>Değer</th>
            <th>Saat dilimi</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="r in satirlar" :key="r.id">
            <td class="num">{{ r.id }}</td>
            <td class="tarih">{{ formatTarih(r.olusturulmaUtc) }}</td>
            <td class="num">{{ r.calisanId }}</td>
            <td>{{ r.veriTipi }}</td>
            <td>{{ r.makineTipi }}</td>
            <td>{{ r.makineAdi }}</td>
            <td>{{ degerHucre(r) }}</td>
            <td>{{ r.saatDilimi }}</td>
          </tr>
        </tbody>
      </table>
    </div>
  </main>
</template>

<script setup lang="ts">
import { isAxiosError } from 'axios'
import { onMounted, ref } from 'vue'
import { RouterLink } from 'vue-router'
import { http, kullaniciHataOzet } from '../api/http'

export interface UretimKayitSatiri {
  id: number
  calisanId: number
  veriTipi: string
  makineTipi: string
  makineAdi: string
  direncDegeri: number | null
  placeholderMetin: string | null
  saatDilimi: string
  olusturulmaUtc: string
}

const satirlar = ref<UretimKayitSatiri[]>([])
const yukleniyor = ref(true)
const hata = ref<string | null>(null)
/** Sadece geliştirme: axios ham yanıtı. */
const hataAyrinti = ref('')

const devAyrinti = import.meta.env.DEV

function formatTarih(iso: string): string {
  const d = new Date(iso)
  if (Number.isNaN(d.getTime())) {
    return iso
  }
  return d.toLocaleString('tr-TR', { timeZone: 'UTC' })
}

function degerHucre(r: UretimKayitSatiri): string {
  if (r.veriTipi === 'Direnç' && r.direncDegeri != null) {
    return String(r.direncDegeri)
  }
  if (r.placeholderMetin) {
    return r.placeholderMetin
  }
  return '—'
}

async function yukle(): Promise<void> {
  hata.value = null
  hataAyrinti.value = ''
  yukleniyor.value = true
  try {
    const { data } = await http.get<UretimKayitSatiri[]>('/api/uretim/kayit-ozet')
    if (!Array.isArray(data)) {
      hata.value = 'API beklenmeyen bir cevap döndü (dizi değil).'
      satirlar.value = []
      return
    }
    satirlar.value = data
  } catch (err) {
    hata.value = kullaniciHataOzet(err)
    if (devAyrinti && isAxiosError(err) && err.response) {
      try {
        hataAyrinti.value = JSON.stringify(
          { status: err.response.status, data: err.response.data },
          null,
          2,
        ).slice(0, 2000)
      } catch {
        hataAyrinti.value = String(err)
      }
    }
    satirlar.value = []
  } finally {
    yukleniyor.value = false
  }
}

onMounted(() => {
  void yukle()
})
</script>

<style scoped>
.sayfa {
  max-width: 72rem;
  margin: 0 auto;
  padding: 1.25rem 1rem 2.5rem;
}
.ust-satir {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
  margin-bottom: 1rem;
}
.baslik {
  font-size: 1.2rem;
  margin: 0;
  color: #0f172a;
}
.ust-aksiyon {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 0.75rem 1rem;
}
.btn-yenile {
  font-size: 0.9rem;
  padding: 0.4rem 0.85rem;
  border-radius: 6px;
  border: 1px solid #94a3b8;
  background: #fff;
  color: #0f172a;
  cursor: pointer;
}
.btn-yenile:hover:not(:disabled) {
  background: #f1f5f9;
}
.btn-yenile:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}
.link {
  color: #334155;
  font-weight: 500;
  text-decoration: none;
}
.link:hover {
  text-decoration: underline;
}
.hata {
  color: #b91c1c;
  margin: 0 0 0.75rem;
}
.ayrinti {
  background: #f1f5f9;
  border: 1px solid #e2e8f0;
  border-radius: 6px;
  color: #475569;
  font-size: 0.7rem;
  line-height: 1.3;
  margin: 0 0 0.75rem;
  max-height: 12rem;
  overflow: auto;
  padding: 0.5rem 0.6rem;
  white-space: pre-wrap;
  word-break: break-word;
}
.bos {
  color: #64748b;
  margin: 0;
}
.yukleniyor-tek {
  color: #64748b;
  margin: 0.5rem 0 0;
}
.tablo-wrap {
  overflow-x: auto;
  border: 1px solid #e2e8f0;
  border-radius: 8px;
  background: #fff;
}
.grid {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.875rem;
}
.grid th,
.grid td {
  text-align: left;
  padding: 0.55rem 0.65rem;
  border-bottom: 1px solid #e2e8f0;
  vertical-align: top;
}
.grid th {
  background: #f1f5f9;
  font-weight: 600;
  color: #334155;
  white-space: nowrap;
}
.grid tbody tr:hover {
  background: #f8fafc;
}
.grid tbody tr:last-child td {
  border-bottom: none;
}
.tarih {
  white-space: nowrap;
}
.num {
  font-variant-numeric: tabular-nums;
}
</style>
