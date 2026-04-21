namespace Fabrika.Api.Models;

/// <summary>Üretim alanından gelen kullanıcı girişi (kalıcı kayıt).</summary>
public class UretimKaydi
{
    public int Id { get; set; }

    public int CalisanId { get; set; }

    /// <summary>"Direnç" veya "Placeholder".</summary>
    public string VeriTipi { get; set; } = string.Empty;

    public string MakineTipi { get; set; } = string.Empty;

    public string MakineAdi { get; set; } = string.Empty;

    public decimal? DirencDegeri { get; set; }

    public string? PlaceholderMetin { get; set; }

    /// <summary>Örn. "08:00-08:30".</summary>
    public string SaatDilimi { get; set; } = string.Empty;

    public DateTimeOffset OlusturulmaUtc { get; set; }
}
