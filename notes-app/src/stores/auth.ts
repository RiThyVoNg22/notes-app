import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { authApi } from '@/api/auth'
import type { LoginRequest, RegisterRequest } from '@/types/auth'

const TOKEN_KEY = 'notes_token'
const USER_KEY = 'notes_user'

function getStoredToken(): string | null {
  return localStorage.getItem(TOKEN_KEY)
}

function getStoredUser(): { userId: number; email: string } | null {
  const raw = localStorage.getItem(USER_KEY)
  if (!raw) return null
  try {
    return JSON.parse(raw) as { userId: number; email: string }
  } catch {
    return null
  }
}

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(getStoredToken())
  const user = ref<{ userId: number; email: string } | null>(getStoredUser())

  const isAuthenticated = computed(() => !!token.value)

  function setAuth(t: string, u: { userId: number; email: string }) {
    token.value = t
    user.value = u
    localStorage.setItem(TOKEN_KEY, t)
    localStorage.setItem(USER_KEY, JSON.stringify(u))
  }

  function clearAuth() {
    token.value = null
    user.value = null
    localStorage.removeItem(TOKEN_KEY)
    localStorage.removeItem(USER_KEY)
  }

  async function login(payload: LoginRequest) {
    const res = await authApi.login(payload)
    setAuth(res.token, { userId: res.userId, email: res.email })
  }

  async function register(payload: RegisterRequest) {
    const res = await authApi.register(payload)
    setAuth(res.token, { userId: res.userId, email: res.email })
  }

  function logout() {
    clearAuth()
  }

  function getAuthHeader(): { Authorization: string } | {} {
    return token.value ? { Authorization: `Bearer ${token.value}` } : {}
  }

  return {
    token,
    user,
    isAuthenticated,
    login,
    register,
    logout,
    getAuthHeader,
  }
})
