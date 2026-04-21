<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router'
</script>

<template>
  <div class="app">
    <header class="ust">
      <div class="ust-ic">
        <h1 class="marka">Fabrika veri girişi</h1>
        <nav class="nav">
          <RouterLink to="/" class="nav-link">Çalışan girişi</RouterLink>
        </nav>
      </div>
    </header>
    <RouterView />
  </div>
</template>

<style scoped>
.app {
  min-height: 100vh;
  background: #fff;
  color: #0f172a;
}
.ust {
  border-bottom: 1px solid #e2e8f0;
  background: #f1f5f9;
}
.ust-ic {
  max-width: 52rem;
  margin: 0 auto;
  padding: 0.75rem 1rem;
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  justify-content: space-between;
  gap: 0.5rem;
}
.marka {
  margin: 0;
  font-size: 1.15rem;
  font-weight: 700;
}
.nav {
  display: flex;
  gap: 1rem;
}
.nav-link {
  color: #334155;
  font-weight: 500;
  text-decoration: none;
}
.nav-link:hover {
  text-decoration: underline;
}
</style>
