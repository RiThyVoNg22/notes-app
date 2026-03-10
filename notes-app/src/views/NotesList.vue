<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import { useNotesStore } from '@/stores/notes'
import NoteCard from '@/components/NoteCard.vue'
import NoteForm from '@/components/NoteForm.vue'
import SearchSortFilter from '@/components/SearchSortFilter.vue'
import type { CreateNoteRequest } from '@/types/note'

const store = useNotesStore()
const showCreateForm = ref(false)
const search = ref('')
const sortBy = ref('createdAt')
const sortDesc = ref(true)

function loadNotes() {
  store.fetchNotes({
    search: search.value || undefined,
    sortBy: sortBy.value,
    sortDesc: sortDesc.value,
  })
}

onMounted(loadNotes)

watch([search, sortBy, sortDesc], () => {
  if (!showCreateForm.value) loadNotes()
})

async function handleCreate(payload: CreateNoteRequest) {
  await store.createNote(payload)
  showCreateForm.value = false
  loadNotes()
}
</script>

<template>
  <div class="space-y-8">
    <div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
      <div>
        <h1 class="text-2xl font-bold tracking-tight text-slate-900 sm:text-3xl">My Notes</h1>
        <p class="mt-1 text-sm text-slate-500">Create and manage your notes</p>
      </div>
      <button
        v-if="!showCreateForm"
        type="button"
        class="inline-flex items-center justify-center gap-2 rounded-xl bg-primary-600 px-5 py-2.5 text-sm font-semibold text-white shadow-sm transition hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-primary-500 focus:ring-offset-2"
        @click="showCreateForm = true"
      >
        <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
        </svg>
        New note
      </button>
    </div>

    <div class="rounded-xl border border-surface-200 bg-white p-4 shadow-card sm:p-5">
      <SearchSortFilter
        v-model:search="search"
        v-model:sort-by="sortBy"
        v-model:sort-desc="sortDesc"
        @apply="loadNotes"
      />
    </div>

    <div v-if="store.error" class="rounded-xl border border-red-200 bg-red-50 p-4 text-sm text-red-800">
      {{ store.error }}
      <button type="button" class="mt-2 font-medium text-red-600 underline hover:no-underline" @click="store.clearError">Dismiss</button>
    </div>

    <div v-if="showCreateForm" class="rounded-xl border border-surface-200 bg-white p-6 shadow-card sm:p-8">
      <h2 class="mb-5 text-lg font-semibold text-slate-900">Create note</h2>
      <NoteForm
        submit-label="Create note"
        :busy="store.loading"
        @submit="handleCreate"
      />
      <button
        type="button"
        class="mt-5 text-sm font-medium text-slate-500 transition hover:text-slate-700"
        @click="showCreateForm = false"
      >
        Cancel
      </button>
    </div>

    <div v-if="store.loading && !store.notes.length" class="flex flex-col items-center justify-center rounded-xl border border-surface-200 bg-white py-16 shadow-card">
      <div class="h-8 w-8 animate-spin rounded-full border-2 border-primary-200 border-t-primary-600"></div>
      <p class="mt-4 text-sm text-slate-500">Loading notes…</p>
    </div>
    <div v-else-if="!store.hasNotes" class="flex flex-col items-center justify-center rounded-xl border-2 border-dashed border-surface-200 bg-surface-50/50 py-16 text-center">
      <div class="flex h-14 w-14 items-center justify-center rounded-full bg-primary-100 text-primary-600">
        <svg xmlns="http://www.w3.org/2000/svg" class="h-7 w-7" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
        </svg>
      </div>
      <p class="mt-4 text-base font-medium text-slate-600">No notes yet</p>
      <p class="mt-1 text-sm text-slate-500">Click “New note” above to create your first note.</p>
    </div>
    <div v-else class="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
      <NoteCard v-for="note in store.notes" :key="note.id" :note="note" />
    </div>
  </div>
</template>
