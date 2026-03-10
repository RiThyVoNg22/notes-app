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
│   ├── ARCHITECTURE.md       # System design, schema, API, structure
│   └── SECURITY.md           # How UserId enforces "own notes only"
├── database/
│   └── Scripts/
│       ├── 001_CreateTables.sql                    # SQL Server (Users + Notes)
│       ├── 002_PostgreSQL_CreateTables.sql         # PostgreSQL (Users + Notes + FK)
│       └── 003_SQLServer_UsersAndNotes_CorrectDesign.sql  # Interview design: UserId NOT NULL + FK
├── NotesApi/                 # ASP.NET Core backend (JWT auth, Dapper)
└── notes-app/                # Vue 3 frontend (Login/Register, Pinia, Axios)
```

## For reviewers

After cloning, copy `NotesApi/.env.example` to `NotesApi/.env` and set your database connection and JWT secret. No secrets are committed.

## GitHub Pages (live demo)

The Vue app can be deployed to GitHub Pages for a public URL.

1. **Enable Pages:** In your repo on GitHub go to **Settings → Pages**. Under "Build and deployment", set **Source** to **GitHub Actions**.
2. **Deploy:** Push to `main` (or run the "Deploy to GitHub Pages" workflow manually). After it finishes, your app is at:
   - **`https://<your-username>.github.io/notes-app/`**
3. **Note:** Only the frontend is deployed. Login and notes will work only if the API is running elsewhere and the app is configured to use it; otherwise the UI will load but API calls will fail.

### Making the backend work on the live site

To have login and notes work on **https://rithyvong22.github.io/notes-app/**:

1. **Deploy the API** to a host that supports ASP.NET Core and gives you a public URL, for example:
   - [Azure App Service](https://azure.microsoft.com/en-us/products/app-service/) (free tier available)
   - [Railway](https://railway.app/) or [Fly.io](https://fly.io/) (free tiers)
   - [Render](https://render.com/) (free tier)

   On the server, set the same config as locally: database connection string (e.g. a hosted PostgreSQL), `Jwt__Key`, `Jwt__Issuer`, `Jwt__Audience`. Use the repo’s `NotesApi/.env.example` as a reference.

2. **Allow the frontend origin in CORS:** The API in this repo already allows `https://rithyvong22.github.io`. If your Pages URL is different, add it in `NotesApi/Program.cs` in the `WithOrigins(...)` call.

3. **Tell the frontend where the API is:** In your GitHub repo go to **Settings → Secrets and variables → Actions**. Add a secret named **`VITE_API_BASE_URL`** with the value set to your API’s base URL **with no trailing slash**, e.g. `https://your-app.azurewebsites.net` or `https://your-app.fly.dev`.

4. **Redeploy the frontend:** In the **Actions** tab, open “Deploy to GitHub Pages” and click **Run workflow** (or push a commit to `main`). The next build will use the secret and the live site will call your deployed API.

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

### 2. Backend: connection string + JWT

Edit `NotesApi/appsettings.Development.json` or `NotesApi/.env`:

- **Connection string**: e.g. `Host=localhost;Database=notesdb;Username=postgres;Password=postgres;`
- **JWT** (required for auth): set `Jwt__Key` (long secret, e.g. 32+ chars), `Jwt__Issuer`, `Jwt__Audience`. See `NotesApi/.env.example`.

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

- Open http://localhost:5173 → you should see **Login** (notes require auth).
- **Register** a new account, then you’re signed in.
- Create a note (title required, content optional).
- Use search and sort on the list.
- Open a note, edit it, delete it. Only your own notes are visible (UserId from JWT).

---

## Features Delivered

| Requirement        | Implementation |
|--------------------|----------------|
| Create Note        | POST /api/notes; Title required, Content optional; CreatedAt/UpdatedAt server-side; UserId from JWT |
| Read Notes         | GET /api/notes (list), GET /api/notes/{id} (detail); **only current user’s notes** (UserId filter) |
| Update Note        | PUT /api/notes/{id}; UpdatedAt set automatically; own notes only |
| Delete Note        | DELETE /api/notes/{id}; own notes only |
| **Users only own notes** | Users + Notes tables; Notes.UserId FK; all queries filter by userId from JWT |
| Login/Register     | POST /api/auth/login, /api/auth/register; JWT returned; frontend Login.vue, Register.vue |
| Search             | Query param `search` (title/content); UI in SearchSortFilter |
| Filter & Sort      | Query params `sortBy` (createdAt, updatedAt, title), `sortDesc`; UI dropdown + button |
| Responsive UI      | Tailwind breakpoints (sm, lg), grid layout |
| API integration    | Axios in `src/api/notes.ts` + Bearer token; `src/api/auth.ts` for login/register |
| State management   | Pinia: `stores/notes.ts`, `stores/auth.ts` (token, user, login, register, logout) |
| Security           | JWT auth; BCrypt passwords; see `docs/SECURITY.md` |

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

## Run with SQL Server

The backend supports **both PostgreSQL and SQL Server**. To use SQL Server:

1. **Create the database**  
   Create a database named `NotesDb` in SQL Server (LocalDB, Docker, or full instance).

2. **Run the correct-design script**  
   Execute `database/Scripts/003_SQLServer_UsersAndNotes_CorrectDesign.sql` in `NotesDb` (Users + Notes with UserId FK).

3. **Configure the API**  
   In `NotesApi/appsettings.Development.json` or `NotesApi/.env` set:
   - `Database__Provider=SqlServer`
   - `ConnectionStrings__DefaultConnection` to your SQL Server connection string, e.g.  
     `Server=localhost;Database=NotesDb;Trusted_Connection=True;TrustServerCertificate=True;`  
     or with SQL auth:  
     `Server=localhost;Database=NotesDb;User Id=sa;Password=YourPassword;TrustServerCertificate=True;`

4. **Run the API**  
   `dotnet run` from `NotesApi`. The app will use the SQL Server repositories and the same JWT auth.

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
dd