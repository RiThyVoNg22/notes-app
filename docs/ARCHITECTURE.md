# Notes Application – System Architecture

## 1. System Architecture Overview

```
┌─────────────────────────────────────────────────────────────────────────┐
│                           CLIENT (Browser)                               │
│  ┌───────────────────────────────────────────────────────────────────┐  │
│  │  Vue 3 + TypeScript + TailwindCSS + Pinia + Axios                   │  │
│  │  • Notes list, detail, create/edit forms                            │  │
│  │  • Search, filter, sort                                             │  │
│  │  • Optional: Login / Register                                       │  │
│  └───────────────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────────────┘
                                      │
                                      │ HTTPS / REST
                                      ▼
┌─────────────────────────────────────────────────────────────────────────┐
│                    ASP.NET Core Web API (Backend)                        │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  ┌─────────────────┐ │
│  │ Controllers │→ │   Services  │→ │ Repositories│→ │  Dapper + SQL   │ │
│  │ (REST API)  │  │ (Business)  │  │ (Data)      │  │  Server         │ │
│  └─────────────┘  └─────────────┘  └─────────────┘  └─────────────────┘ │
│  Optional: JWT Middleware for auth                                       │
└─────────────────────────────────────────────────────────────────────────┘
                                      │
                                      ▼
┌─────────────────────────────────────────────────────────────────────────┐
│                         SQL Server Database                             │
│  • Users (optional), Notes                                              │
└─────────────────────────────────────────────────────────────────────────┘
```

- **Frontend**: SPA with Vue 3, TypeScript, TailwindCSS; Pinia for state; Axios for HTTP.
- **Backend**: ASP.NET Core Web API, RESTful, optional JWT; Dapper for data access; SQL Server.
- **Data ownership**: When auth is enabled, notes are scoped by `UserId`; users only see their own notes.

---

## 2. Database Schema Design

### Tables

**Users** (optional – for auth)

| Column       | Type         | Constraints        |
|-------------|--------------|--------------------|
| Id          | int          | PK, IDENTITY       |
| Email       | nvarchar(256)| NOT NULL, UNIQUE   |
| PasswordHash| nvarchar(256)| NOT NULL           |
| CreatedAt   | datetime2    | NOT NULL           |

**Notes**

| Column    | Type         | Constraints                    |
|-----------|--------------|--------------------------------|
| Id        | int          | PK, IDENTITY                   |
| UserId    | int          | NULL (no auth) or FK → Users   |
| Title     | nvarchar(200)| NOT NULL                      |
| Content   | nvarchar(max)| NULL                          |
| CreatedAt | datetime2    | NOT NULL                      |
| UpdatedAt | datetime2    | NOT NULL                      |

- Without auth: `UserId` can be NULL; all notes are shared (or you can omit the column for a simpler schema).
- With auth: `UserId` is required; all queries filter by `UserId`.

---

## 3. API Endpoint Design (RESTful)

| Method   | Endpoint        | Description              | Auth   |
|----------|-----------------|--------------------------|--------|
| GET      | /api/notes      | List notes (filter, sort)| Optional |
| GET      | /api/notes/{id} | Get single note         | Optional |
| POST     | /api/notes      | Create note             | Optional |
| PUT      | /api/notes/{id} | Update note             | Optional |
| DELETE   | /api/notes/{id} | Delete note             | Optional |
| POST     | /api/auth/register | Register (optional)  | No     |
| POST     | /api/auth/login    | Login, return JWT (optional) | No  |

Query params for `GET /api/notes`: `search`, `sortBy` (e.g. CreatedAt, Title), `sortOrder` (asc/desc).

---

## 4. Backend Project Structure (ASP.NET Core)

```
NotesApi/
├── NotesApi.csproj
├── Program.cs
├── appsettings.json
├── appsettings.Development.json
├── Controllers/
│   ├── NotesController.cs
│   └── AuthController.cs          # optional
├── Models/
│   ├── Note.cs
│   ├── User.cs                    # optional
│   └── DTOs/
│       ├── CreateNoteRequest.cs
│       ├── UpdateNoteRequest.cs
│       └── NoteResponse.cs
├── Services/
│   ├── INoteRepository.cs
│   ├── NoteRepository.cs
│   └── (IAuthService, JwtService if auth)
├── Middleware/                    # optional
└── Database/
    └── Scripts/
        └── 001_CreateTables.sql
```

---

## 5. Frontend Project Structure (Vue 3 + TypeScript)

```
notes-app/
├── index.html
├── package.json
├── vite.config.ts
├── tsconfig.json
├── tailwind.config.js
├── postcss.config.js
├── src/
│   ├── main.ts
│   ├── App.vue
│   ├── api/
│   │   └── notes.ts              # Axios instance + note endpoints
│   ├── stores/
│   │   └── notes.ts               # Pinia store
│   ├── types/
│   │   └── note.ts
│   ├── views/
│   │   ├── NotesList.vue
│   │   └── NoteDetail.vue
│   ├── components/
│   │   ├── NoteCard.vue
│   │   ├── NoteForm.vue
│   │   └── SearchSortFilter.vue
│   └── router/
│       └── index.ts
└── README.md
```

---

## 6. State Management (Pinia)

- **Store**: `notes` store with state: `notes[]`, `currentNote`, `loading`, `error`.
- **Actions**: `fetchNotes`, `fetchNoteById`, `createNote`, `updateNote`, `deleteNote` (call API, then update state).
- **Getters**: Filtered/sorted list can be a getter or computed in the list view using store state + local filter/sort.
- **Reactive**: Components use the store and stay in sync; forms use v-model and commit via actions.

---

## 7. Running Locally

1. **Database**: Run SQL scripts to create tables in SQL Server (local or Docker).
2. **Backend**: Set connection string in `appsettings.Development.json`, run `dotnet run` from `NotesApi`.
3. **Frontend**: `npm install` then `npm run dev` from `notes-app`.
4. **CORS**: Backend allows the Vue dev server origin (e.g. `http://localhost:5173`).

Detailed steps are in the root README and in each project’s README.
