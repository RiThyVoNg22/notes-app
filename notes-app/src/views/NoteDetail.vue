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
const showDeleteModal = ref(false)
const deleting = ref(false)

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

function openDeleteModal() {
  showDeleteModal.value = true
}

function closeDeleteModal() {
  if (!deleting.value) showDeleteModal.value = false
}

async function confirmDelete() {
  deleting.value = true
  try {
    await store.deleteNote(noteId.value)
    store.clearCurrentNote()
    showDeleteModal.value = false
    await router.push({ name: 'notes' })
  } finally {
    deleting.value = false
  }
}
</script>

<template>
  <div class="space-y-6">
    <router-link
      to="/"
      class="inline-flex items-center gap-1.5 text-sm font-medium text-slate-500 transition hover:text-primary-600"
    >
      <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18" />
      </svg>
      Back to list
    </router-link>

    <div v-if="store.error" class="rounded-xl border border-red-200 bg-red-50 p-4 text-sm text-red-800">
      {{ store.error }}
    </div>

    <div v-if="store.loading && !note" class="flex flex-col items-center justify-center rounded-xl border border-surface-200 bg-white py-16 shadow-card">
      <div class="h-8 w-8 animate-spin rounded-full border-2 border-primary-200 border-t-primary-600"></div>
      <p class="mt-4 text-sm text-slate-500">Loading…</p>
    </div>

    <div v-else-if="!note" class="rounded-xl border border-surface-200 bg-white p-12 text-center shadow-card">
      <p class="text-slate-600">Note not found.</p>
    </div>

    <div v-else class="overflow-hidden rounded-xl border border-surface-200 bg-white shadow-card">
      <div class="border-b border-surface-200 bg-surface-50/50 p-6 sm:p-8">
        <div v-if="!isEditing" class="space-y-4">
          <h1 class="text-2xl font-bold tracking-tight text-slate-900 sm:text-3xl">{{ note.title }}</h1>
          <p class="text-sm text-slate-500">
            Created {{ formattedCreated }} · Updated {{ formattedUpdated }}
          </p>
          <div class="flex flex-wrap gap-2 pt-2">
            <button
              type="button"
              class="inline-flex items-center gap-1.5 rounded-lg border border-surface-200 bg-white px-4 py-2 text-sm font-semibold text-slate-700 shadow-sm transition hover:bg-surface-50 focus:outline-none focus:ring-2 focus:ring-primary-500"
              @click="isEditing = true"
            >
              Edit
            </button>
            <button
              type="button"
              class="inline-flex items-center gap-1.5 rounded-lg border border-red-200 bg-white px-4 py-2 text-sm font-semibold text-red-600 shadow-sm transition hover:bg-red-50 focus:outline-none focus:ring-2 focus:ring-red-500"
              @click="openDeleteModal"
            >
              Delete
            </button>
          </div>
        </div>
        <div v-else>
          <NoteForm
            :title="note.title"
            :content="note.content"
            submit-label="Save changes"
            :busy="store.loading"
            @submit="handleUpdate"
          />
          <button
            type="button"
            class="mt-5 text-sm font-medium text-slate-500 transition hover:text-slate-700"
            @click="isEditing = false"
          >
            Cancel
          </button>
        </div>
      </div>
      <div v-if="!isEditing && (note.content ?? '').length" class="whitespace-pre-wrap p-6 text-slate-700 leading-relaxed sm:p-8">
        {{ note.content }}
      </div>
      <div v-else-if="!isEditing" class="p-6 text-slate-400 sm:p-8">
        No content.
      </div>
    </div>

    <!-- Delete confirmation modal -->
    <Teleport to="body">
      <Transition name="modal">
        <div
          v-if="showDeleteModal"
          class="fixed inset-0 z-50 flex items-center justify-center p-4"
          role="dialog"
          aria-modal="true"
          aria-labelledby="delete-modal-title"
        >
          <div
            class="absolute inset-0 bg-slate-900/60 backdrop-blur-sm"
            @click="closeDeleteModal"
          />
          <div
            class="relative w-full max-w-md rounded-2xl border border-surface-200 bg-white p-6 shadow-xl"
            @click.stop
          >
            <div class="flex items-start gap-4">
              <div class="flex h-12 w-12 shrink-0 items-center justify-center rounded-full bg-red-100 text-red-600">
                <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                </svg>
              </div>
              <div class="flex-1 min-w-0">
                <h2 id="delete-modal-title" class="text-lg font-semibold text-slate-900">
                  Delete note?
                </h2>
                <p class="mt-1 text-sm text-slate-500">
                  This action cannot be undone. The note will be permanently removed.
                </p>
                <div class="mt-6 flex flex-wrap gap-3">
                  <button
                    type="button"
                    class="flex-1 min-w-[100px] rounded-lg border border-surface-200 bg-white px-4 py-2.5 text-sm font-semibold text-slate-700 transition hover:bg-surface-50 focus:outline-none focus:ring-2 focus:ring-primary-500 focus:ring-offset-2 disabled:opacity-50"
                    :disabled="deleting"
                    @click="closeDeleteModal"
                  >
                    Cancel
                  </button>
                  <button
                    type="button"
                    class="flex-1 min-w-[100px] rounded-lg bg-red-600 px-4 py-2.5 text-sm font-semibold text-white transition hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-red-500 focus:ring-offset-2 disabled:opacity-50"
                    :disabled="deleting"
                    @click="confirmDelete"
                  >
                    {{ deleting ? 'Deleting…' : 'Delete' }}
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </Transition>
    </Teleport>
  </div>
</template>

<style scoped>
.modal-enter-active,
.modal-leave-active {
  transition: opacity 0.2s ease;
}
.modal-enter-active .relative,
.modal-leave-active .relative {
  transition: transform 0.2s ease;
}
.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}
.modal-enter-from .relative,
.modal-leave-to .relative {
  transform: scale(0.95);
}
</style>
