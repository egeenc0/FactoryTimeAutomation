<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
import { useRouter } from 'vue-router'
import { http, kullaniciHataOzet } from '../api/http'

const props = defineProps<{ calisanId: string }>()
const router = useRouter()

const calisanSayi = computed(() => Number.parseInt(props.calisanId, 10))

const veriTipi = ref<'Direnç' | 'Placeholder' | ''>('')
const makineTipi = ref<string>('')
const makineAdi = ref<string>('')
const direncDegeri = ref<number | null>(null)
const placeholderMetin = ref<string>('')

const tumDilimler = ref<string[]>([])
const onerilenDilim = ref('')
const alternatifDilim = ref('')
const secilenDilim = ref('')

const makineAdiHata = ref<string | null>(null)
const dilimApiHata = ref<string | null>(null)
const dilimYukleniyor = ref(false)
const gonderiliyor = ref(false)

interface SaatDilimleriResponse {
  tumDilimler: string[]
  onerilenDilim: string
  alternatifDilim: string
}

function makineAdiGecerliMi(ad: string): boolean {
  const t = ad.trim()
  if (t.length < 1) {
    return false
  }
  return /^[\p{L}\p{N}\s._-]+$/u.test(t)
}

async function dilimleriYenile(): Promise<void> {
  dilimApiHata.value = null
  dilimYukleniyor.value = true
  try {
    const { data } = await http.get<SaatDilimleriResponse>('/api/uretim/saat-dilimleri')
    tumDilimler.value = [...data.tumDilimler]
    onerilenDilim.value = data.onerilenDilim
    alternatifDilim.value = data.alternatifDilim
    if (!secilenDilim.value || !tumDilimler.value.includes(secilenDilim.value)) {
      secilenDilim.value = data.onerilenDilim
    }
  } catch (err) {
    dilimApiHata.value = kullaniciHataOzet(err)
  } finally {
    dilimYukleniyor.value = false
  }
}

function onerileniSec(): void {
  secilenDilim.value = onerilenDilim.value
}

function alternatifiSec(): void {
  secilenDilim.value = alternatifDilim.value
}

async function gonder(): Promise<void> {
  makineAdiHata.value = null
  if (!veriTipi.value) {
    alert('Ne gireceğinizi seçin (Direnç veya Placeholder).')
    return
  }
  if (!makineTipi.value) {
    alert('Makine tipini seçin.')
    return
  }
  if (!makineAdiGecerliMi(makineAdi.value)) {
    makineAdiHata.value = 'Makine adı en az 1 karakter; harf, rakam, boşluk, - _ . kullanılabilir.'
    return
  }
  if (veriTipi.value === 'Direnç') {
    if (direncDegeri.value === null || Number.isNaN(direncDegeri.value)) {
      alert('Direnç değerini (ondalıklı) girin.')
      return
    }
  } else if (veriTipi.value === 'Placeholder') {
    if (!placeholderMetin.value.trim()) {
      alert('Placeholder metnini girin.')
      return
    }
  }
  if (!secilenDilim.value) {
    alert('Saat dilimini seçin.')
    return
  }

  const payload = {
    calisanId: calisanSayi.value,
    veriTipi: veriTipi.value,
    makineTipi: makineTipi.value,
    makineAdi: makineAdi.value.trim(),
    direncDegeri: veriTipi.value === 'Direnç' ? direncDegeri.value : null,
    placeholderMetin: veriTipi.value === 'Placeholder' ? placeholderMetin.value.trim() : null,
    saatDilimi: secilenDilim.value,
  }

  gonderiliyor.value = true
  try {
    const { data } = await http.post<{ id: number }>('/api/uretim/kayitlar', payload)
    alert(`Kayıt tamamlandı (sunucu Id: ${data.id}).`)
  } catch (err: unknown) {
    alert(kullaniciHataOzet(err))
  } finally {
    gonderiliyor.value = false
  }
}

function geri(): void {
  void router.push({ name: 'calisan-giris' })
}

function calisanKontrol(): void {
  const n = calisanSayi.value
  if (!Number.isFinite(n) || n < 1) {
    void router.replace({ name: 'calisan-giris' })
  }
}

onMounted(() => {
  calisanKontrol()
  void dilimleriYenile()
})

watch(
  () => props.calisanId,
  () => {
    calisanKontrol()
    void dilimleriYenile()
  },
)
</script>

<template>
  <main v-if="Number.isFinite(calisanSayi) && calisanSayi >= 1" class="sayfa">
    <div class="ust-bar">
      <button type="button" class="btn-ghost" @click="geri">← Çalışan numarası</button>
    </div>

    <section class="kart" aria-labelledby="baslik-uretim">
      <h2 id="baslik-uretim">2 — Veri girişi (kullanıcı)</h2>

      <p v-if="dilimApiHata" class="hata-kucuk" role="alert">{{ dilimApiHata }}</p>

      <div class="grid-form">
        <div class="satir">
          <span class="hucre etiket">Çalışan ID</span>
          <span class="hucre deger vurgu">{{ calisanSayi }}</span>
        </div>

        <div class="satir">
          <span class="hucre etiket">Ne giriyorsunuz?</span>
          <span class="hucre deger">
            <select v-model="veriTipi" class="select-genis">
              <option value="" disabled>Seçiniz</option>
              <option value="Direnç">Direnç (ondalık sayı)</option>
              <option value="Placeholder">Placeholder (metin)</option>
            </select>
          </span>
        </div>

        <div class="satir">
          <span class="hucre etiket">Makine tipi</span>
          <span class="hucre deger">
            <select v-model="makineTipi" class="select-genis">
              <option value="" disabled>Makine tipi seçin</option>
              <option value="Torna">Torna</option>
              <option value="CNC">CNC</option>
              <option value="Pres">Pres</option>
            </select>
          </span>
        </div>

        <div class="satir">
          <span class="hucre etiket">Makine adı</span>
          <span class="hucre deger">
            <input
              v-model="makineAdi"
              class="input-genis"
              type="text"
              maxlength="80"
              placeholder="Örn: TRN-01, Pres-A4"
              autocomplete="off"
            />
            <span v-if="makineAdiHata" class="hata-kucuk">{{ makineAdiHata }}</span>
          </span>
        </div>

        <div v-if="veriTipi === 'Direnç'" class="satir">
          <span class="hucre etiket">Direnç</span>
          <span class="hucre deger">
            <input
              v-model.number="direncDegeri"
              class="input-genis"
              type="number"
              step="0.01"
              inputmode="decimal"
              placeholder="Örn: 14,5 veya 14.5"
            />
          </span>
        </div>

        <div v-if="veriTipi === 'Placeholder'" class="satir">
          <span class="hucre etiket">Placeholder metni</span>
          <span class="hucre deger">
            <input
              v-model="placeholderMetin"
              class="input-genis"
              type="text"
              maxlength="200"
              placeholder="Kısa açıklama / kod"
            />
          </span>
        </div>

        <div class="satir satir-dilim">
          <span class="hucre etiket">Saat dilimi</span>
          <span class="hucre deger dilim-blok">
            <p class="dilim-aciklama">
              Şu anki saate göre önerilen dilim ve bir sonraki dilim (sunucu saati). İsterseniz aşağıdan
              başka dilim seçin.
            </p>
            <div class="dilim-chipler">
              <button
                type="button"
                class="chip"
                :class="{ aktif: secilenDilim === onerilenDilim }"
                :disabled="dilimYukleniyor"
                @click="onerileniSec"
              >
                Önerilen: {{ onerilenDilim || '…' }}
              </button>
              <button
                type="button"
                class="chip"
                :class="{ aktif: secilenDilim === alternatifDilim }"
                :disabled="dilimYukleniyor"
                @click="alternatifiSec"
              >
                Alternatif: {{ alternatifDilim || '…' }}
              </button>
            </div>
            <label class="sr-only" for="dilim-select">Tüm dilimler</label>
            <select
              id="dilim-select"
              v-model="secilenDilim"
              class="select-genis dilim-select"
              :disabled="dilimYukleniyor || tumDilimler.length === 0"
            >
              <option v-for="d in tumDilimler" :key="d" :value="d">{{ d }}</option>
            </select>
          </span>
        </div>
      </div>

      <div class="aksiyon">
        <button
          type="button"
          class="btn-birincil"
          :disabled="dilimYukleniyor || !!dilimApiHata || gonderiliyor"
          @click="gonder"
        >
          {{ gonderiliyor ? 'Gönderiliyor…' : 'Kaydı tamamla' }}
        </button>
      </div>
    </section>
  </main>
</template>

<style scoped>
.sayfa {
  max-width: 48rem;
  margin: 0 auto;
  padding: 0 1rem 2rem;
}
.ust-bar {
  margin-bottom: 0.75rem;
}
.btn-ghost {
  background: transparent;
  border: none;
  color: #334155;
  cursor: pointer;
  font-size: 0.95rem;
  padding: 0.35rem 0;
  text-decoration: underline;
}
.btn-ghost:hover {
  color: #0f172a;
}
.kart {
  background: #f8fafc;
  border: 1px solid #e2e8f0;
  border-radius: 10px;
  padding: 1.25rem 1.5rem;
}
.kart h2 {
  margin: 0 0 1rem;
  font-size: 1.1rem;
}
.grid-form {
  display: flex;
  flex-direction: column;
}
.satir {
  display: grid;
  grid-template-columns: minmax(8rem, 11rem) 1fr;
  gap: 0.5rem 1rem;
  align-items: start;
  padding: 0.7rem 0;
  border-bottom: 1px solid #e2e8f0;
}
.satir:last-of-type {
  border-bottom: none;
}
.etiket {
  font-weight: 600;
  color: #334155;
  padding-top: 0.35rem;
}
.vurgu {
  font-size: 1.25rem;
  font-weight: 700;
  color: #0f172a;
}
.input-genis,
.select-genis {
  width: 100%;
  max-width: 22rem;
  font-size: 1rem;
  padding: 0.5rem 0.55rem;
  border: 1px solid #94a3b8;
  border-radius: 6px;
}
.dilim-blok {
  max-width: 28rem;
}
.dilim-aciklama {
  margin: 0 0 0.5rem;
  font-size: 0.88rem;
  color: #64748b;
  line-height: 1.4;
}
.dilim-chipler {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  margin-bottom: 0.65rem;
}
.chip {
  cursor: pointer;
  border: 2px solid #cbd5e1;
  background: #fff;
  border-radius: 8px;
  padding: 0.45rem 0.75rem;
  font-size: 0.9rem;
}
.chip.aktif {
  border-color: #0f172a;
  background: #e2e8f0;
}
.chip:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}
.dilim-select {
  max-width: 22rem;
}
.hata-kucuk {
  display: block;
  color: #b91c1c;
  font-size: 0.82rem;
  margin-top: 0.35rem;
}
.aksiyon {
  margin-top: 1.25rem;
}
.btn-birincil {
  font-size: 1rem;
  padding: 0.65rem 1.25rem;
  border-radius: 6px;
  border: none;
  cursor: pointer;
  background: #0f172a;
  color: #f8fafc;
}
.btn-birincil:hover:not(:disabled) {
  background: #1e293b;
}
.btn-birincil:disabled {
  opacity: 0.65;
  cursor: not-allowed;
}
.sr-only {
  position: absolute;
  width: 1px;
  height: 1px;
  padding: 0;
  margin: -1px;
  overflow: hidden;
  clip: rect(0, 0, 0, 0);
  white-space: nowrap;
  border: 0;
}
</style>
