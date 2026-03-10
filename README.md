# Notes Application – Full Stack (Vue 3 + ASP.NET Core + PostgreSQL)

A clean, professional Notes CRUD application for a Full Stack Developer interview assignment.

## Tech Stack

| Layer    | Technology                    |
|----------|-------------------------------|
| Frontend | Vue 3, TypeScript, TailwindCSS, Pinia, Axios, Vue Router |
| Backend  | ASP.NET Core 8 Web API (C#)   |
| Database | PostgreSQL                    |
| ORM      | Dapper                        |

## Repository Layout

```
├── docs/
│   └── ARCHITECTURE.md       # System design, schema, API, structure
├── database/
│   └── Scripts/
│       ├── 001_CreateTables.sql        # SQL Server (legacy)
│       └── 002_PostgreSQL_CreateTables.sql
├── NotesApi/                 # ASP.NET Core backend
└── notes-app/                # Vue 3 frontend
```

## Prerequisites

- **.NET 8 SDK** – [Download](https://dotnet.microsoft.com/download)
- **Node.js 18+** and npm
- **PostgreSQL** – local install or Docker (see below)

## Step-by-Step: Run Locally

### 1. PostgreSQL database

Create the database and run the schema script.

**Option A – Local PostgreSQL**

```bash
# Create database (macOS/Linux with psql)
createdb notesdb

# Run schema
psql -d notesdb -f database/Scripts/002_PostgreSQL_CreateTables.sql
```

**Option B – Docker**

```bash
docker run --name notes-pg -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=notesdb -p 5432:5432 -d postgres:16
# Then run the schema (from project root):
psql -h localhost -p 5432 -U postgres -d notesdb -f database/Scripts/002_PostgreSQL_CreateTables.sql
# Password: postgres
```

### 2. Backend connection string

Edit `NotesApi/appsettings.Development.json` (and optionally `appsettings.json`) and set:

- **Default (local postgres, user postgres)**:
  `Host=localhost;Database=notesdb;Username=postgres;Password=postgres;`
- **Docker**: same if you used the command above.
- **Custom**: `Host=your-host;Database=notesdb;Username=your-user;Password=your-password;`

### 3. Run backend

```bash
cd NotesApi
dotnet restore
dotnet run
```

API runs at **http://localhost:5000**. Swagger UI: http://localhost:5000/swagger

### 4. Run frontend

```bash
cd notes-app
npm install
npm run dev
```

App runs at **http://localhost:5173**. Vite proxies `/api` to the backend, so no CORS issues in development.

### 5. Verify

- Open http://localhost:5173
- Create a note (title required, content optional)
- Use search and sort on the list
- Open a note, edit it, delete it

---

## Features Delivered

| Requirement        | Implementation |
|--------------------|----------------|
| Create Note        | POST /api/notes; Title required, Content optional; CreatedAt/UpdatedAt server-side |
| Read Notes         | GET /api/notes (list), GET /api/notes/{id} (detail); list shows title + date, click for full content |
| Update Note        | PUT /api/notes/{id}; UpdatedAt set automatically |
| Delete Note        | DELETE /api/notes/{id}; list refreshes |
| Search             | Query param `search` (title/content); UI in SearchSortFilter |
| Filter & Sort      | Query params `sortBy` (createdAt, updatedAt, title), `sortDesc`; UI dropdown + button |
| Responsive UI      | Tailwind breakpoints (sm, lg), grid layout |
| API integration    | Axios in `src/api/notes.ts` |
| State management   | Pinia store `stores/notes.ts` |
| Auth (optional)    | Schema and repo support UserId; JWT can be added (see ARCHITECTURE.md) |

---

## Design Decisions (Interview Talking Points)

1. **Dapper over EF Core** – Lightweight, explicit SQL, no migration tooling; good fit for a small API and clear control over queries (including search/sort and future user scoping).
2. **Repository pattern** – `INoteRepository` abstracts data access; easy to test and swap implementations. All note access goes through the repository with optional `userId` for “users only see their own notes.”
3. **RESTful API** – Resource-based URLs, correct HTTP verbs and status codes (201 Created, 204 No Content, 404 Not Found).
4. **Pinia for state** – Centralized notes list and current note; components stay simple and consistent. Loading and error state in one place.
5. **Tailwind** – Utility-first, responsive, no custom CSS files; theme extended with primary/surface for a consistent look.
6. **TypeScript** – Shared types for Note and request DTOs; fewer runtime bugs and better IDE support.
7. **Separation of concerns** – API layer (Axios) → Store (Pinia) → Views/Components; backend: Controller → Repository → SQL Server.

---

## Optional: Add JWT Auth Later

- Implement `POST /api/auth/register` and `POST /api/auth/login`; issue JWT with user id in claims.
- In `NotesController`, resolve user id from JWT and pass to repository; repository already filters by `UserId`.
- Frontend: store token, send `Authorization: Bearer <token>`; optional login/register page.

---

## Best Practices for an Interview Demo

1. **Code quality** – Consistent naming, small focused methods, minimal duplication.
2. **Error handling** – API returns 404 for missing notes; frontend shows messages and dismiss.
3. **Validation** – Backend: `[Required]`, `[StringLength]` on DTOs; frontend: required title, optional content.
4. **Security** – No secrets in repo; connection string in appsettings (and env in production). With auth, never log or expose passwords.
5. **Documentation** – ARCHITECTURE.md for design; README for setup and run; Swagger for API.
6. **Git** – Commit in small steps (e.g. “Add Notes API”, “Add Vue list view”, “Add search/sort”). Clear commit messages.

---

## Build for Production

**Backend**

```bash
cd NotesApi
dotnet publish -c Release -o ./publish
```

**Frontend**

```bash
cd notes-app
npm run build
```

Serve the `notes-app/dist` folder with your web server; configure it to proxy `/api` to the backend, or set the frontend’s API base URL to the backend origin (e.g. via env).
