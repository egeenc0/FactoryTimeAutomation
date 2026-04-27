using Fabrika.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddSimpleConsole(o =>
{
    o.SingleLine = true;
    o.TimestampFormat = "HH:mm:ss ";
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' is missing. Set it in appsettings or user secrets.");

builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sql =>
    {
        sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
    }));

// Geliştirmede tarayıcı localhost/127.0.0.1/::1 farkı ve VITE_API_BASE_URL ile CORS sürüprizlerini kapatır.
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()));
}
else
{
    builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy
        .WithOrigins(
            "http://localhost:5173",
            "https://localhost:5173",
            "http://127.0.0.1:5173",
            "https://127.0.0.1:5173")
        .AllowAnyHeader()
        .AllowAnyMethod()));
}

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Geliştirmede Vite (http://localhost:5173) → http://localhost:5047 proxy kullanılıyor;
// UseHttpsRedirection burada 307 ile https’e yönlendirip istemci/proxy davranışını karıştırabiliyor.
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseRouting();
app.UseCors();

// Minimal API: controller keşfi / sıra sorunlarından bağımsız sabit rota
app.MapGet("/api/urunler", async (ApplicationDbContext db, CancellationToken cancellationToken) =>
{
    var liste = await db.Urunler.AsNoTracking().ToListAsync(cancellationToken);
    return Results.Ok(liste);
});

// Admin grid: aynı path’te yalnızca bu GET var (405 önlemi), controller dışı sabit uç
app.MapGet("/api/uretim/kayit-ozet", async (ApplicationDbContext db, CancellationToken cancellationToken) =>
{
    var liste = await db.UretimKayitlari
        .AsNoTracking()
        .OrderByDescending(x => x.OlusturulmaUtc)
        .ThenByDescending(x => x.Id)
        .Take(500)
        .ToListAsync(cancellationToken);
    return Results.Ok(liste);
});

app.MapControllers();

app.Run();
