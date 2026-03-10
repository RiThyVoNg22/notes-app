import axios from 'axios'
import type { AuthResponse, LoginRequest, RegisterRequest } from '@/types/auth'

// Same origin in dev (Vite proxies /api to backend); relative path so proxy works
const api = axios.create({
  baseURL: '',
  headers: { 'Content-Type': 'application/json' },
  timeout: 10000,
})

export const authApi = {
  async login(payload: LoginRequest): Promise<AuthResponse> {
    const { data } = await api.post<AuthResponse>('/api/auth/login', payload)
    return data
  },

  async register(payload: RegisterRequest): Promise<AuthResponse> {
    const { data } = await api.post<AuthResponse>('/api/auth/register', payload)
    return data
  },
}
