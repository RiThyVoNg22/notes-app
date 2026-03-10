# Security Considerations – Notes Application

## 14. Protecting User Notes (Interview-Focused)

### Correct design: Users + Notes with UserId

- **Users table**: Id, Email, PasswordHash, CreatedAt.
- **Notes table**: Id, **UserId** (FK → Users), Title, Content, CreatedAt, UpdatedAt.

The **UserId** foreign key is how we enforce: **users can only read, update, and delete their own notes.**

### How it works

1. **Authentication**: User registers or logs in; backend returns a JWT containing the user’s Id (and email).
2. **Authorization**: Every Notes API request requires `[Authorize]`. The backend reads the user id from the JWT (e.g. `ClaimTypes.NameIdentifier`).
3. **Data access**: All Dapper queries include a `UserId` parameter:
   - **List notes**: `WHERE UserId = @UserId`
   - **Get by id**: `WHERE Id = @Id AND UserId = @UserId`
   - **Create**: `INSERT ... UserId = @UserId`
   - **Update**: `WHERE Id = @Id AND UserId = @UserId`
   - **Delete**: `WHERE Id = @Id AND UserId = @UserId`

So even if someone knows another note’s Id, they cannot read, update, or delete it because the repository filters by the current user’s Id.

### Security practices in this project

| Practice | Implementation |
|----------|----------------|
| Passwords | Stored only as BCrypt hash; never logged or returned in API. |
| JWT | Signed with a secret (Jwt:Key); validated on every request. Key should be long (e.g. 32+ chars) and kept secret. |
| HTTPS | Use HTTPS in production; JWT in header avoids cookie concerns. |
| Connection strings | In appsettings or .env; .env in .gitignore so secrets are not committed. |
| SQL injection | All queries use Dapper parameters (@UserId, @Id, etc.); no string concatenation of user input. |
| CORS | Backend allows only known origins (e.g. frontend dev URL). |

### Optional hardening

- Use short-lived JWTs and refresh tokens.
- Rate-limit login/register.
- Validate email format and strength (e.g. password complexity) on register.
- In production, set `Jwt:Key` via environment or secret manager, not in appsettings in repo.
