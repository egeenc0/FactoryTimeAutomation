<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const calisanId = ref<string>('')
const hata = ref<string | null>(null)

function devam(): void {
  hata.value = null
  const id = Number.parseInt(calisanId.value.trim(), 10)
  if (!Number.isFinite(id) || id < 1) {
    hata.value = 'Geçerli bir çalışan numarası girin (pozitif tam sayı).'
    return
  }
  void router.push({ name: 'uretim-giris', params: { calisanId: String(id) } })
}
</script>

<template>
  <main class="sayfa">
    <p class="aciklama">
      Kağıt form yerine bu sayfadan giriş yapın. Önce çalışan numaranızı girin; sonraki ekranda
      makine ve ölçüm bilgilerini seçeceksiniz.
    </p>

    <section class="kart" aria-labelledby="baslik-giris">
      <h2 id="baslik-giris">1 — Çalışan numarası</h2>

      <div class="grid-form">
        <div class="satir baslik-satir">
          <span class="hucre etiket">Çalışan ID</span>
          <span class="hucre deger">
            <input
              v-model="calisanId"
              class="buyuk-input"
              type="text"
              inputmode="numeric"
              autocomplete="username"
              placeholder="Örn: 1045"
              @keydown.enter.prevent="devam"
            />
          </span>
        </div>
      </div>

      <p v-if="hata" class="hata" role="alert">{{ hata }}</p>

      <div class="aksiyon">
        <button type="button" class="btn-birincil" @click="devam">Devam — veri girişi</button>
      </div>
    </section>
  </main>
</template>

<style scoped>
.sayfa {
  max-width: 42rem;
  margin: 0 auto;
  padding: 1rem 1rem 2rem;
}
.aciklama {
  color: #475569;
  font-size: 0.95rem;
  line-height: 1.5;
  margin-bottom: 1.25rem;
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
  gap: 0;
}
.satir {
  display: grid;
  grid-template-columns: minmax(7rem, 10rem) 1fr;
  gap: 0.75rem 1rem;
  align-items: center;
  padding: 0.65rem 0;
  border-bottom: 1px solid #e2e8f0;
}
.satir:last-of-type {
  border-bottom: none;
}
.baslik-satir {
  font-weight: 600;
}
.etiket {
  color: #334155;
}
.buyuk-input {
  width: 100%;
  max-width: 16rem;
  font-size: 1.25rem;
  padding: 0.55rem 0.65rem;
  border: 1px solid #94a3b8;
  border-radius: 6px;
}
.hata {
  color: #b91c1c;
  margin: 0.75rem 0 0;
  font-size: 0.9rem;
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
.btn-birincil:hover {
  background: #1e293b;
}
</style>
