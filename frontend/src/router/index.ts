import { createRouter, createWebHistory } from 'vue-router'
import CalisanGirisView from '../views/CalisanGirisView.vue'
import UretimGirisView from '../views/UretimGirisView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'calisan-giris',
      component: CalisanGirisView,
      meta: { title: 'Çalışan girişi' },
    },
    {
      path: '/admin',
      name: 'admin-panel',
      component: () => import('../views/AdminPanelView.vue'),
      meta: { title: 'Yönetici paneli' },
    },
    {
      path: '/uretim/:calisanId',
      name: 'uretim-giris',
      component: UretimGirisView,
      props: true,
      meta: { title: 'Üretim veri girişi' },
    },
    { path: '/:pathMatch(.*)*', redirect: '/' },
  ],
})

router.afterEach((to) => {
  const base = 'Fabrika veri girişi'
  document.title = to.meta.title ? `${base} — ${to.meta.title}` : base
})

export default router
