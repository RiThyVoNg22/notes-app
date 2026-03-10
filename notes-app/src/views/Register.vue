<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const auth = useAuthStore()
const email = ref('')
const password = ref('')
const confirmPassword = ref('')
const error = ref('')
const loading = ref(false)

async function submit() {
  error.value = ''
  if (!email.value.trim() || !password.value) {
    error.value = 'Email and password are required.'
    return
  }
  if (password.value.length < 6) {
    error.value = 'Password must be at least 6 characters.'
    return
  }
  if (password.value !== confirmPassword.value) {
    error.value = 'Passwords do not match.'
    return
  }
  loading.value = true
  try {
    await auth.register({ email: email.value.trim(), password: password.value })
    await router.push('/')
  } catch (e: unknown) {
    const err = e as { response?: { status?: number; data?: { message?: string } }; message?: string }
    if (err.response?.status === 404) {
      error.value = 'API not found. Is the backend running? Start it with: cd NotesApi && dotnet run'
    } else if (err.response?.data?.message) {
      error.value = err.response.data.message
    } else {
      error.value = err.message?.includes('Network') ? 'Cannot reach server. Is the API running on port 5000?' : 'Registration failed.'
    }
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="mx-auto max-w-sm space-y-8 pt-8">
    <div class="text-center">
      <h1 class="text-2xl font-bold text-slate-900">Create account</h1>
      <p class="mt-1 text-sm text-slate-500">Register to start saving notes</p>
    </div>
    <form class="space-y-5 rounded-xl border border-surface-200 bg-white p-6 shadow-card" @submit.prevent="submit">
      <div v-if="error" class="rounded-lg bg-red-50 p-3 text-sm text-red-700">
        {{ error }}
      </div>
      <div>
        <label for="reg-email" class="block text-sm font-semibold text-slate-700">Email</label>
        <input
          id="reg-email"
          v-model="email"
          type="email"
          required
          autocomplete="email"
          class="mt-2 w-full rounded-lg border border-surface-200 bg-surface-50 px-4 py-2.5 focus:border-primary-500 focus:bg-white focus:outline-none focus:ring-1 focus:ring-primary-500"
        />
      </div>
      <div>
        <label for="reg-password" class="block text-sm font-semibold text-slate-700">Password (min 6)</label>
        <input
          id="reg-password"
          v-model="password"
          type="password"
          required
          minlength="6"
          autocomplete="new-password"
          class="mt-2 w-full rounded-lg border border-surface-200 bg-surface-50 px-4 py-2.5 focus:border-primary-500 focus:bg-white focus:outline-none focus:ring-1 focus:ring-primary-500"
        />
      </div>
      <div>
        <label for="reg-confirm" class="block text-sm font-semibold text-slate-700">Confirm password</label>
        <input
          id="reg-confirm"
          v-model="confirmPassword"
          type="password"
          required
          class="mt-2 w-full rounded-lg border border-surface-200 bg-surface-50 px-4 py-2.5 focus:border-primary-500 focus:bg-white focus:outline-none focus:ring-1 focus:ring-primary-500"
        />
      </div>
      <button
        type="submit"
        :disabled="loading"
        class="w-full rounded-lg bg-primary-600 py-2.5 font-semibold text-white transition hover:bg-primary-700 disabled:opacity-50"
      >
        {{ loading ? 'Creating account…' : 'Register' }}
      </button>
    </form>
    <p class="text-center text-sm text-slate-500">
      Already have an account?
      <router-link to="/login" class="font-medium text-primary-600 hover:underline">Sign in</router-link>
    </p>
  </div>
</template>
