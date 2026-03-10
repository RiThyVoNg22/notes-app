export interface Note {
  id: number
  title: string
  content: string | null
  createdAt: string
  updatedAt: string
}

export interface CreateNoteRequest {
  title: string
  content?: string | null
}

export interface UpdateNoteRequest {
  title: string
  content?: string | null
}
