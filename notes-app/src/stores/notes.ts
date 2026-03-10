import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { notesApi } from '@/api/notes'
import type { Note, CreateNoteRequest, UpdateNoteRequest } from '@/types/note'

export const useNotesStore = defineStore('notes', () => {
  const notes = ref<Note[]>([])
  const currentNote = ref<Note | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  const hasNotes = computed(() => notes.value.length > 0)

  async function fetchNotes(params?: { search?: string; sortBy?: string; sortDesc?: boolean }) {
    loading.value = true
    error.value = null
    try {
      notes.value = await notesApi.getAll(params)
    } catch (e: unknown) {
      error.value = e instanceof Error ? e.message : 'Failed to load notes'
      notes.value = []
    } finally {
      loading.value = false
    }
  }

  async function fetchNoteById(id: number) {
    loading.value = true
    error.value = null
    currentNote.value = null
    try {
      currentNote.value = await notesApi.getById(id)
      return currentNote.value
    } catch (e: unknown) {
      error.value = e instanceof Error ? e.message : 'Failed to load note'
      return null
    } finally {
      loading.value = false
    }
  }

  async function createNote(payload: CreateNoteRequest) {
    loading.value = true
    error.value = null
    try {
      const created = await notesApi.create(payload)
      notes.value = [created, ...notes.value]
      return created
    } catch (e: unknown) {
      error.value = e instanceof Error ? e.message : 'Failed to create note'
      throw e
    } finally {
      loading.value = false
    }
  }

  async function updateNote(id: number, payload: UpdateNoteRequest) {
    loading.value = true
    error.value = null
    try {
      const updated = await notesApi.update(id, payload)
      const index = notes.value.findIndex((n) => n.id === id)
      if (index !== -1) notes.value[index] = updated
      if (currentNote.value?.id === id) currentNote.value = updated
      return updated
    } catch (e: unknown) {
      error.value = e instanceof Error ? e.message : 'Failed to update note'
      throw e
    } finally {
      loading.value = false
    }
  }

  async function deleteNote(id: number) {
    loading.value = true
    error.value = null
    try {
      await notesApi.delete(id)
      notes.value = notes.value.filter((n) => n.id !== id)
      if (currentNote.value?.id === id) currentNote.value = null
    } catch (e: unknown) {
      error.value = e instanceof Error ? e.message : 'Failed to delete note'
      throw e
    } finally {
      loading.value = false
    }
  }

  function clearCurrentNote() {
    currentNote.value = null
  }

  function clearError() {
    error.value = null
  }

  return {
    notes,
    currentNote,
    loading,
    error,
    hasNotes,
    fetchNotes,
    fetchNoteById,
    createNote,
    updateNote,
    deleteNote,
    clearCurrentNote,
    clearError,
  }
})
