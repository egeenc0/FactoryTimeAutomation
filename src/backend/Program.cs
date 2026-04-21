using Fabrika.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",
                "https://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddOpenApi();

var app = builder.Build();

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

app.UseCors();

// Minimal API: controller keşfi / sıra sorunlarından bağımsız sabit rota
app.MapGet("/api/urunler", async (ApplicationDbContext db, CancellationToken cancellationToken) =>
{
    var liste = await db.Urunler.AsNoTracking().ToListAsync(cancellationToken);
    return Results.Ok(liste);
});

app.MapControllers();

app.Run();
