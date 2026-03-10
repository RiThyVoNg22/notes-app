import axios from 'axios'
import type { AuthResponse, LoginRequest, RegisterRequest } from '@/types/auth'

// In dev we use '' (Vite proxies /api). In prod set VITE_API_BASE_URL to your deployed API origin.
const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL ?? '',
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
