namespace Fabrika.Api.Models;

/// <summary>Ürünler tablosu — Id, Ad, Fiyat.</summary>
public class Urun
{
    public int Id { get; set; }

    public string Ad { get; set; } = string.Empty;

    public decimal Fiyat { get; set; }
}
