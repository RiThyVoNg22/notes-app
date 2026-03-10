<script setup lang="ts">
import { RouterView } from 'vue-router'
</script>

<template>
  <div class="min-h-screen bg-surface-50 font-sans text-slate-800">
    <header class="border-b border-surface-200 bg-white shadow-sm">
      <div class="mx-auto flex h-14 max-w-4xl items-center justify-between px-4 sm:px-6">
        <router-link to="/" class="text-xl font-semibold text-primary-600 hover:text-primary-700">
          Notes
        </router-link>
      </div>
    </header>
    <main class="mx-auto max-w-4xl px-4 py-6 sm:px-6">
      <RouterView />
    </main>
  </div>
</template>
