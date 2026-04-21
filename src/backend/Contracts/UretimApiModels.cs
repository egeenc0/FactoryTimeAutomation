namespace Fabrika.Api.Contracts;

public record SaatDilimleriResponse(
    IReadOnlyList<string> TumDilimler,
    string OnerilenDilim,
    string AlternatifDilim);

public record UretimKayitIstek(
    int CalisanId,
    string VeriTipi,
    string MakineTipi,
    string MakineAdi,
    decimal? DirencDegeri,
    string? PlaceholderMetin,
    string SaatDilimi);

public record UretimKayitYanit(int Id);
