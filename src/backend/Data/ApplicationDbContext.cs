using Fabrika.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Fabrika.Api.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Urun> Urunler => Set<Urun>();

    public DbSet<UretimKaydi> UretimKayitlari => Set<UretimKaydi>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Urun>(entity =>
        {
            entity.ToTable("Urunler");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Ad).HasMaxLength(256).IsRequired();
            entity.Property(e => e.Fiyat).HasPrecision(18, 2);
        });

        modelBuilder.Entity<UretimKaydi>(entity =>
        {
            entity.ToTable("UretimKayitlari");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.VeriTipi).HasMaxLength(32).IsRequired();
            entity.Property(e => e.MakineTipi).HasMaxLength(64).IsRequired();
            entity.Property(e => e.MakineAdi).HasMaxLength(80).IsRequired();
            entity.Property(e => e.PlaceholderMetin).HasMaxLength(200);
            entity.Property(e => e.SaatDilimi).HasMaxLength(32).IsRequired();
            entity.Property(e => e.DirencDegeri).HasPrecision(18, 4);
        });
    }
}
