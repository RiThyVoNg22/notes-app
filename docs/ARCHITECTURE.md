# Notes Application – System Architecture

## 1. System Architecture Overview

```
┌─────────────────────────────────────────────────────────────────────────┐
│                           CLIENT (Browser)                               │
│  ┌───────────────────────────────────────────────────────────────────┐  │
│  │  Vue 3 + TypeScript + TailwindCSS + Pinia + Axios                   │  │
│  │  • Notes list, detail, create/edit forms                            │  │
│  │  • Search, filter, sort                                             │  │
│  │  • Login / Register (auth store, JWT in Axios)                      │  │
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
│  JWT Middleware: [Authorize] on Notes; userId from token → own notes only │
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

**Users** (required for auth; enforces note ownership)

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
| UserId    | int          | FK → Users (enforces “own notes only”) |
| Title     | nvarchar(200)| NOT NULL                      |
| Content   | nvarchar(max)| NULL                          |
| CreatedAt | datetime2    | NOT NULL                      |
| UpdatedAt | datetime2    | NOT NULL                      |

- **UserId** is the key: every note is tied to a user. All list/get/update/delete queries filter by `UserId` from the JWT so users only access their own notes.

---

## 3. API Endpoint Design (RESTful)

| Method   | Endpoint        | Description              | Auth   |
|----------|-----------------|--------------------------|--------|
| GET      | /api/notes      | List current user’s notes (filter, sort) | Bearer JWT |
| GET      | /api/notes/{id} | Get single note (own only)              | Bearer JWT |
| POST     | /api/notes      | Create note (UserId from JWT)           | Bearer JWT |
| PUT      | /api/notes/{id} | Update note (own only)                  | Bearer JWT |
| DELETE   | /api/notes/{id} | Delete note (own only)                  | Bearer JWT |
| POST     | /api/auth/register | Register; returns JWT                 | No         |
| POST     | /api/auth/login    | Login; returns JWT                     | No         |

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
│   └── AuthController.cs          # Register, Login (JWT)
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
│   ├── IUserRepository.cs, UserRepository.cs
│   └── JwtService.cs
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
│   │   ├── notes.ts              # Axios + Bearer token; note endpoints
│   │   └── auth.ts               # login, register
│   ├── stores/
│   │   ├── notes.ts              # Pinia store
│   │   └── auth.ts               # token, user, login, register, logout
│   ├── types/
│   │   └── note.ts
│   ├── views/
│   │   ├── Login.vue, Register.vue
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

## 7. Security (UserId = Own Notes Only)

- Notes API uses `[Authorize]`; JWT carries the user id.
- Repository methods take `userId` and filter: `WHERE UserId = @UserId` (and for get/update/delete also by Id).
- Create sets `UserId` to the current user. So users can only read, update, and delete their own notes. See `docs/SECURITY.md`.

---

## 8. Running Locally

1. **Database**: Run SQL scripts to create tables in SQL Server (local or Docker).
2. **Backend**: Set connection string in `appsettings.Development.json`, run `dotnet run` from `NotesApi`.
3. **Frontend**: `npm install` then `npm run dev` from `notes-app`.
4. **CORS**: Backend allows the Vue dev server origin (e.g. `http://localhost:5173`).

Detailed steps are in the root README and in each project’s README.
