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
  <div class="space-y-6">
    <div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
      <h1 class="text-2xl font-bold text-slate-900">My Notes</h1>
      <button
        v-if="!showCreateForm"
        type="button"
        class="rounded-lg bg-primary-600 px-4 py-2 text-sm font-medium text-white shadow-sm hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-primary-500 focus:ring-offset-2"
        @click="showCreateForm = true"
      >
        + New note
      </button>
    </div>

    <SearchSortFilter
      v-model:search="search"
      v-model:sort-by="sortBy"
      v-model:sort-desc="sortDesc"
      @apply="loadNotes"
    />

    <div v-if="store.error" class="rounded-lg bg-red-50 p-4 text-sm text-red-700">
      {{ store.error }}
      <button type="button" class="ml-2 underline" @click="store.clearError">Dismiss</button>
    </div>

    <div v-if="showCreateForm" class="rounded-xl border border-surface-200 bg-white p-6 shadow-sm">
      <h2 class="mb-4 text-lg font-semibold text-slate-800">Create note</h2>
      <NoteForm
        submit-label="Create"
        :busy="store.loading"
        @submit="handleCreate"
      />
      <button
        type="button"
        class="mt-4 text-sm text-slate-500 hover:text-slate-700"
        @click="showCreateForm = false"
      >
        Cancel
      </button>
    </div>

    <div v-if="store.loading && !store.notes.length" class="py-12 text-center text-slate-500">
      Loading notes…
    </div>
    <div v-else-if="!store.hasNotes" class="rounded-xl border border-dashed border-surface-200 bg-surface-50 py-12 text-center text-slate-500">
      No notes yet. Create one above.
    </div>
    <div v-else class="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
      <NoteCard v-for="note in store.notes" :key="note.id" :note="note" />
    </div>
  </div>
</template>
