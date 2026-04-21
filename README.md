# FabrikaOtomasyonu — Vue + ASP.NET Core + MSSQL şablonu

Bu depo, **Vue 3 (Vite + TypeScript)** istemcisi ve **.NET 10 Web API** + **Entity Framework Core** + **SQL Server** backend’i için başlangıç iskeletidir. Bağlantı bilgileri ve iş kurallarını sen tamamlarsın.

## Yapı

| Klasör | Açıklama |
|--------|----------|
| `client/` | Vue uygulaması; geliştirmede `/api` istekleri `vite.config.ts` proxy ile `http://localhost:5047` adresine gider. |
| `src/Fabrika.Api/` | REST API, `Controllers/`, `Data/ApplicationDbContext.cs`, `Models/`. |

## Ön koşullar

- [.NET SDK](https://dotnet.microsoft.com/download) (şablonda `net10.0`)
- [Node.js LTS](https://nodejs.org/) (Vite için)
- Çalışan bir **SQL Server** (LocalDB, Docker veya tam sunucu)

## 1) Veritabanı bağlantısı

`src/Fabrika.Api/appsettings.Development.json` içindeki `ConnectionStrings:DefaultConnection` değerini kendi sunucuna göre düzenle (kullanıcı/şifre veya Windows auth için connection string formatını kullan).

## 2) Migration ve veritabanı

Proje kökünden (yerel `dotnet-ef` araç bildirimi depoda):

```powershell
dotnet tool restore
dotnet tool run dotnet-ef -- migrations add Initial --project src/Fabrika.Api --startup-project src/Fabrika.Api
dotnet tool run dotnet-ef -- database update --project src/Fabrika.Api --startup-project src/Fabrika.Api
```

Araç bildirimi yoksa alternatif: `dotnet tool install --global dotnet-ef` ve ardından `dotnet ef migrations add ...`.

## 3) API’yi çalıştır

```powershell
dotnet run --project src/Fabrika.Api
```

Varsayılan HTTP adresi: `http://localhost:5047` (`Properties/launchSettings.json`).

## 4) Vue istemcisini çalıştır

```powershell
cd client
npm install
npm run dev
```

Tarayıcıda Vite adresi (genelde `http://localhost:5173`) açıldığında ana sayfa API’den health ve örnek liste yanıtını dener.

## Üretim / farklı API adresi

`client/.env.example` dosyasını kopyalayıp `VITE_API_BASE_URL` ile tam API kökünü ver; proxy sadece geliştirme içindir.

## Doldurman için ipuçları

- Yeni tablolar: `Models/` + `ApplicationDbContext` içinde `DbSet` ve `OnModelCreating`.
- Yeni uçlar: `Controllers/` altında `[ApiController]` sınıfları.
- Vue tarafında HTTP: `client/src/api/http.ts` üzerinden `axios` kullan.
