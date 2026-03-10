import { createRouter, createWebHistory } from 'vue-router'
import NotesList from '@/views/NotesList.vue'
import NoteDetail from '@/views/NoteDetail.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    { path: '/', name: 'notes', component: NotesList },
    { path: '/notes/:id', name: 'note-detail', component: NoteDetail, props: true },
  ],
})

export default router
