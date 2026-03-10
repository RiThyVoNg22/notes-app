import axios from 'axios'
import type { Note, CreateNoteRequest, UpdateNoteRequest } from '@/types/note'

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL ?? (import.meta.env.DEV ? '' : '/api'),
  headers: { 'Content-Type': 'application/json' },
  timeout: 10000,
})

// Add JWT to every request so users only access their own notes
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('notes_token')
  if (token) config.headers.Authorization = `Bearer ${token}`
  return config
})

// On 401, clear auth and redirect to login
api.interceptors.response.use(
  (r) => r,
  (err) => {
    if (err.response?.status === 401) {
      localStorage.removeItem('notes_token')
      localStorage.removeItem('notes_user')
      const base = (import.meta.env.BASE_URL || '/').replace(/\/$/, '')
      window.location.href = `${base}/login`
    }
    return Promise.reject(err)
  }
)

const notesBase = '/api/notes'

export const notesApi = {
  async getAll(params?: { search?: string; sortBy?: string; sortDesc?: boolean }): Promise<Note[]> {
    const { data } = await api.get<Note[]>(notesBase, { params })
    return data
  },

  async getById(id: number): Promise<Note> {
    const { data } = await api.get<Note>(`${notesBase}/${id}`)
    return data
  },

  async create(payload: CreateNoteRequest): Promise<Note> {
    const { data } = await api.post<Note>(notesBase, payload)
    return data
  },

  async update(id: number, payload: UpdateNoteRequest): Promise<Note> {
    const { data } = await api.put<Note>(`${notesBase}/${id}`, payload)
    return data
  },

  async delete(id: number): Promise<void> {
    await api.delete(`${notesBase}/${id}`)
  },
}

export default api
