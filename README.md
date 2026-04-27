# Fabrika Otomasyonu (Factory Automation)

A small **shop-floor data entry** web app for factories that want to replace paper forms with a browser-based flow. Workers enter an employee number, then record what they measured (e.g. resistance values or short placeholder text), which machine was involved, and the **time slot** for the entry. The backend validates everything, generates **30-minute time slots** using **server time** (so client clocks cannot fake the shift), and persists records to **SQL Server**.

**Stack:** Vue 3 (Vite, TypeScript) · ASP.NET Core Web API (.NET 10) · Entity Framework Core · Microsoft SQL Server

---

## What it does

1. **Employee step** — Enter a positive numeric employee ID to continue.
2. **Production step** — Choose data type (resistance decimal or free-text placeholder), machine type (e.g. lathe, CNC, press), machine name, and a time slot. Recommended and alternate slots are suggested from the server; a full list is available in a dropdown.
3. **API** — `GET /api/uretim/saat-dilimleri` returns slot labels; `POST /api/uretim/kayitlar` saves a row; `GET /api/uretim/kayit-ozet` lists the latest production rows (admin grid). `GET /api/urunler` and `GET /api/health` are sample checks.

---

## Repository layout

| Path | Purpose |
|------|---------|
| `frontend/` | Vue SPA; in dev, `/api` is proxied to the API (see `vite.config.ts`). |
| `src/backend/` | ASP.NET Core project (`Fabrika.Api`): controllers, EF Core `ApplicationDbContext`, models, migrations. |

---

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (targeting `net10.0` in this repo)
- [Node.js LTS](https://nodejs.org/) (for Vite)
- A running **SQL Server** instance (LocalDB, Docker, or full server)

---

## Configuration (SQL Server)

**Default (Windows):** `appsettings.Development.json` uses **SQL Server LocalDB** (`(localdb)\MSSQLLocalDB`) and **Windows authentication** — no `sa` password. Install [SQL Server Express LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb) (often already present with Visual Studio) or the full **SQL Server / Express** stack.

- **Custom server (Docker, named instance, `sa` user):** override the connection string without committing secrets:
  ```powershell
  cd src/backend
  dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=FabrikaDb_Dev;User Id=sa;Password=***;TrustServerCertificate=True;Encrypt=True;"
  ```
- **Linux / macOS:** LocalDB is not available; use [SQL Server in Docker](https://learn.microsoft.com/en-us/sql/linux/sql-server-linux-docker-container-setup) and point `ConnectionStrings:DefaultConnection` at that instance (or user secrets as above).

---

## Database

From the repository root (local `dotnet-ef` is declared in `dotnet-tools.json`):

```powershell
dotnet tool restore
dotnet tool run dotnet-ef -- database update --project src/backend/Fabrika.Api.csproj --startup-project src/backend/Fabrika.Api.csproj
```

If you add model changes later:

```powershell
dotnet tool run dotnet-ef -- migrations add <Name> --project src/backend/Fabrika.Api.csproj --startup-project src/backend/Fabrika.Api.csproj
dotnet tool run dotnet-ef -- database update --project src/backend/Fabrika.Api.csproj --startup-project src/backend/Fabrika.Api.csproj
```

---

## Run the API

```powershell
dotnet run --project src/backend/Fabrika.Api.csproj
```

Default HTTP URL is typically `http://localhost:5047` (see `src/backend/Properties/launchSettings.json`). CORS allows the Vite dev origin (`http://localhost:5173`).

---

## Run the frontend

```powershell
cd frontend
npm install
npm run dev
```

Open the URL Vite prints (usually `http://localhost:5173`).

---

## Production / custom API URL

Copy `frontend/.env.example` to `frontend/.env.production` (or similar) and set `VITE_API_BASE_URL` to your public API base URL. The dev proxy only applies during `npm run dev`.

---

## Extending the app

- New tables: add `Models/`, register `DbSet<T>` and mapping in `Data/ApplicationDbContext.cs`, then add a migration.
- New endpoints: add controllers under `src/backend/Controllers/` or minimal APIs in `Program.cs`.
- HTTP client: `frontend/src/api/http.ts` (axios).
