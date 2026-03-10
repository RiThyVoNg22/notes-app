<script setup lang="ts">
import { computed } from 'vue'
import type { Note } from '@/types/note'

const props = defineProps<{ note: Note }>()

const formattedDate = computed(() => {
  const d = new Date(props.note.createdAt)
  return d.toLocaleDateString(undefined, { dateStyle: 'medium' })
})

const excerpt = computed(() => {
  const c = props.note.content ?? ''
  return c.length > 120 ? c.slice(0, 120) + '…' : c || 'No content'
})
</script>

<template>
  <router-link
    :to="{ name: 'note-detail', params: { id: note.id } }"
    class="group block rounded-xl border border-surface-200 bg-white p-5 shadow-card transition hover:border-primary-200 hover:shadow-card-hover focus:outline-none focus:ring-2 focus:ring-primary-500 focus:ring-offset-2"
  >
    <h3 class="font-semibold text-slate-900 transition group-hover:text-primary-700">{{ note.title }}</h3>
    <p class="mt-1.5 text-xs font-medium uppercase tracking-wide text-slate-400">{{ formattedDate }}</p>
    <p v-if="excerpt" class="mt-3 line-clamp-2 text-sm leading-relaxed text-slate-600">{{ excerpt }}</p>
  </router-link>
</template>
