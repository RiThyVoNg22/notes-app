<script setup lang="ts">
import { ref, watch } from 'vue'
import type { CreateNoteRequest } from '@/types/note'

const props = defineProps<{
  title?: string
  content?: string | null
  submitLabel?: string
  busy?: boolean
}>()

const emit = defineEmits<{
  (e: 'submit', payload: CreateNoteRequest): void
}>()

const title = ref(props.title ?? '')
const content = ref(props.content ?? '')

watch(
  () => [props.title, props.content],
  ([t, c]) => {
    title.value = (t as string) ?? ''
    content.value = (c as string) ?? ''
  }
)

const canSubmit = ref(true)
function submit() {
  const t = title.value.trim()
  if (!t) return
  canSubmit.value = false
  emit('submit', { title: t, content: content.value.trim() || null })
  setTimeout(() => (canSubmit.value = true), 300)
}
</script>

<template>
  <form class="space-y-4" @submit.prevent="submit">
    <div>
      <label for="form-title" class="block text-sm font-medium text-slate-700">Title *</label>
      <input
        id="form-title"
        v-model="title"
        type="text"
        required
        maxlength="200"
        placeholder="Note title"
        class="mt-1 w-full rounded-lg border border-surface-200 bg-white px-3 py-2 shadow-sm focus:border-primary-500 focus:outline-none focus:ring-1 focus:ring-primary-500"
      />
    </div>
    <div>
      <label for="form-content" class="block text-sm font-medium text-slate-700">Content</label>
      <textarea
        id="form-content"
        v-model="content"
        rows="5"
        placeholder="Note content (optional)"
        class="mt-1 w-full rounded-lg border border-surface-200 bg-white px-3 py-2 shadow-sm focus:border-primary-500 focus:outline-none focus:ring-1 focus:ring-primary-500"
      />
    </div>
    <button
      type="submit"
      :disabled="busy || !title.trim()"
      class="w-full rounded-lg bg-primary-600 px-4 py-2 font-medium text-white shadow-sm hover:bg-primary-700 disabled:opacity-50 sm:w-auto"
    >
      {{ busy ? 'Saving…' : (submitLabel ?? 'Save') }}
    </button>
  </form>
</template>
