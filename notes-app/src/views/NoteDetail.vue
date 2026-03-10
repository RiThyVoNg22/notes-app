<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useNotesStore } from '@/stores/notes'
import NoteForm from '@/components/NoteForm.vue'
import type { UpdateNoteRequest } from '@/types/note'

const props = defineProps<{ id: string }>()
const route = useRoute()
const router = useRouter()
const store = useNotesStore()
const isEditing = ref(false)

const noteId = computed(() => Number(route.params.id))
const note = computed(() => store.currentNote)
const formattedCreated = computed(() =>
  note.value ? new Date(note.value.createdAt).toLocaleString(undefined, { dateStyle: 'medium', timeStyle: 'short' }) : ''
)
const formattedUpdated = computed(() =>
  note.value ? new Date(note.value.updatedAt).toLocaleString(undefined, { dateStyle: 'medium', timeStyle: 'short' }) : ''
)

onMounted(() => store.fetchNoteById(noteId.value))
watch(() => route.params.id, (id) => {
  if (id) store.fetchNoteById(Number(id))
})

async function handleUpdate(payload: UpdateNoteRequest) {
  await store.updateNote(noteId.value, payload)
  isEditing.value = false
}

async function handleDelete() {
  if (!confirm('Delete this note?')) return
  await store.deleteNote(noteId.value)
  store.clearCurrentNote()
  router.push({ name: 'notes' })
}
</script>

<template>
  <div class="space-y-6">
    <div class="flex items-center gap-2 text-sm text-slate-500">
      <router-link to="/" class=" hover:text-primary-600">← Back to list</router-link>
    </div>

    <div v-if="store.error" class="rounded-lg bg-red-50 p-4 text-sm text-red-700">
      {{ store.error }}
    </div>

    <div v-if="store.loading && !note" class="py-12 text-center text-slate-500">
      Loading…
    </div>

    <div v-else-if="!note" class="rounded-xl border border-surface-200 bg-white p-8 text-center text-slate-500">
      Note not found.
    </div>

    <div v-else class="rounded-xl border border-surface-200 bg-white shadow-sm">
      <div class="border-b border-surface-200 p-6">
        <div v-if="!isEditing" class="space-y-2">
          <h1 class="text-2xl font-bold text-slate-900">{{ note.title }}</h1>
          <p class="text-sm text-slate-500">
            Created {{ formattedCreated }} · Updated {{ formattedUpdated }}
          </p>
          <div class="flex gap-2 pt-2">
            <button
              type="button"
              class="rounded-lg border border-surface-200 bg-white px-3 py-1.5 text-sm font-medium text-slate-700 hover:bg-surface-50"
              @click="isEditing = true"
            >
              Edit
            </button>
            <button
              type="button"
              class="rounded-lg border border-red-200 bg-white px-3 py-1.5 text-sm font-medium text-red-600 hover:bg-red-50"
              @click="handleDelete"
            >
              Delete
            </button>
          </div>
        </div>
        <div v-else>
          <NoteForm
            :title="note.title"
            :content="note.content"
            submit-label="Save"
            :busy="store.loading"
            @submit="handleUpdate"
          />
          <button
            type="button"
            class="mt-4 text-sm text-slate-500 hover:text-slate-700"
            @click="isEditing = false"
          >
            Cancel
          </button>
        </div>
      </div>
      <div v-if="!isEditing && (note.content ?? '').length" class="whitespace-pre-wrap p-6 text-slate-700">
        {{ note.content }}
      </div>
      <div v-else-if="!isEditing" class="p-6 text-slate-400">
        No content.
      </div>
    </div>
  </div>
</template>
