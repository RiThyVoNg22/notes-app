<script setup lang="ts">
import { ref, watch } from 'vue'

const props = withDefaults(
  defineProps<{
    search?: string
    sortBy?: string
    sortDesc?: boolean
  }>(),
  { search: '', sortBy: 'createdAt', sortDesc: true }
)

const emit = defineEmits<{
  (e: 'update:search', v: string): void
  (e: 'update:sortBy', v: string): void
  (e: 'update:sortDesc', v: boolean): void
  (e: 'apply'): void
}>()

const searchInput = ref(props.search)
const sortByLocal = ref(props.sortBy)
const sortDescLocal = ref(props.sortDesc)

watch(searchInput, (v) => emit('update:search', v))
watch(sortByLocal, (v) => emit('update:sortBy', v))
watch(sortDescLocal, (v) => emit('update:sortDesc', v))

function apply() {
  emit('apply')
}
</script>

<template>
  <div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:gap-4">
    <div class="relative flex-1">
      <label for="search" class="sr-only">Search notes</label>
      <span class="pointer-events-none absolute left-3 top-1/2 -translate-y-1/2 text-slate-400">
        <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
        </svg>
      </span>
      <input
        id="search"
        v-model="searchInput"
        type="search"
        placeholder="Search by title or content..."
        class="w-full rounded-lg border border-surface-200 bg-surface-50 py-2.5 pl-10 pr-4 text-sm text-slate-800 placeholder-slate-400 transition focus:border-primary-500 focus:bg-white focus:outline-none focus:ring-1 focus:ring-primary-500"
        @keydown.enter="apply"
      />
    </div>
    <div class="flex flex-wrap items-center gap-2">
      <label for="sortBy" class="sr-only">Sort by</label>
      <select
        id="sortBy"
        v-model="sortByLocal"
        class="rounded-lg border border-surface-200 bg-surface-50 px-3 py-2.5 text-sm text-slate-700 transition focus:border-primary-500 focus:outline-none focus:ring-1 focus:ring-primary-500"
      >
        <option value="createdAt">Created</option>
        <option value="updatedAt">Updated</option>
        <option value="title">Title</option>
      </select>
      <button
        type="button"
        class="rounded-lg border border-surface-200 bg-surface-50 px-3 py-2.5 text-sm font-medium text-slate-700 transition hover:bg-surface-100 focus:outline-none focus:ring-1 focus:ring-primary-500"
        :aria-label="sortDescLocal ? 'Sort descending' : 'Sort ascending'"
        @click="sortDescLocal = !sortDescLocal"
      >
        {{ sortDescLocal ? '↓ Newest first' : '↑ Oldest first' }}
      </button>
      <button
        type="button"
        class="rounded-lg bg-primary-600 px-4 py-2.5 text-sm font-semibold text-white transition hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-primary-500 focus:ring-offset-2"
        @click="apply"
      >
        Apply
      </button>
    </div>
  </div>
</template>
