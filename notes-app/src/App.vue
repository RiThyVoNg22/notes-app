<script setup lang="ts">
import { RouterView, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
const router = useRouter()
function logout() {
  auth.logout()
  router.push('/login')
}
</script>

<template>
  <div class="min-h-screen bg-gradient-to-b from-surface-50 to-white font-sans text-slate-800">
    <header class="sticky top-0 z-10 border-b border-surface-200/80 bg-white/95 backdrop-blur-sm shadow-sm">
      <div class="mx-auto flex h-14 max-w-4xl items-center justify-between px-4 sm:px-6">
        <router-link
          to="/"
          class="flex items-center gap-2 text-lg font-semibold text-slate-800 transition hover:text-primary-600"
        >
          <span class="flex h-8 w-8 items-center justify-center rounded-lg bg-primary-100 text-primary-600">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9a2 2 0 112 2v6a2 2 0 01-2 2h-2m-4-1l4 4m0 0l4-4m-4 4V3" />
            </svg>
          </span>
          Notes
        </router-link>
        <div v-if="auth.isAuthenticated" class="flex items-center gap-3">
          <span class="text-sm text-slate-600">{{ auth.user?.email }}</span>
          <button
            type="button"
            class="rounded-lg border border-surface-200 bg-white px-3 py-1.5 text-sm font-medium text-slate-700 hover:bg-surface-50"
            @click="logout"
          >
            Logout
          </button>
        </div>
      </div>
    </header>
    <main class="mx-auto max-w-4xl px-4 py-8 sm:px-6">
      <RouterView />
    </main>
  </div>
</template>
