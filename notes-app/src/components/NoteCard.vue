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
    class="block rounded-xl border border-surface-200 bg-white p-4 shadow-sm transition hover:border-primary-200 hover:shadow-md focus:outline-none focus:ring-2 focus:ring-primary-500"
  >
    <h3 class="font-semibold text-slate-800">{{ note.title }}</h3>
    <p class="mt-1 text-sm text-slate-500">{{ formattedDate }}</p>
    <p v-if="excerpt" class="mt-2 line-clamp-2 text-sm text-slate-600">{{ excerpt }}</p>
  </router-link>
</template>
