using System.Text.RegularExpressions;
using Fabrika.Api.Contracts;
using Fabrika.Api.Services;

namespace Fabrika.Api.Validation;

public static class UretimKayitValidator
{
    private static readonly Regex MakineAdiRegex = new(
        @"^[\p{L}\p{N}\s._-]+$",
        RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly HashSet<string> MakineTipleri = new(StringComparer.Ordinal)
    {
        "Torna",
        "CNC",
        "Pres",
    };

    private const string VeriTipiDirenc = "Direnç";
    private const string VeriTipiPlaceholder = "Placeholder";

    /// <summary>Hata mesajı veya null = geçerli.</summary>
    public static string? Validate(UretimKayitIstek body)
    {
        if (body.CalisanId < 1)
        {
            return "Çalışan numarası pozitif tam sayı olmalıdır.";
        }

        if (body.VeriTipi != VeriTipiDirenc && body.VeriTipi != VeriTipiPlaceholder)
        {
            return "Veri tipi Direnç veya Placeholder olmalıdır.";
        }

        if (!MakineTipleri.Contains(body.MakineTipi))
        {
            return "Geçerli bir makine tipi seçin (Torna, CNC, Pres).";
        }

        var makineAdi = body.MakineAdi.Trim();
        if (makineAdi.Length < 1)
        {
            return "Makine adı en az 1 karakter olmalıdır.";
        }

        if (makineAdi.Length > 80)
        {
            return "Makine adı en fazla 80 karakter olabilir.";
        }

        if (!MakineAdiRegex.IsMatch(makineAdi))
        {
            return "Makine adında yalnızca harf, rakam, boşluk, - _ . kullanılabilir.";
        }

        if (body.VeriTipi == VeriTipiDirenc)
        {
            if (body.DirencDegeri is null)
            {
                return "Direnç değeri girilmelidir.";
            }
        }
        else
        {
            var metin = body.PlaceholderMetin?.Trim() ?? string.Empty;
            if (metin.Length < 1)
            {
                return "Placeholder metni girilmelidir.";
            }

            if (metin.Length > 200)
            {
                return "Placeholder metni en fazla 200 karakter olabilir.";
            }
        }

        var dilim = body.SaatDilimi?.Trim() ?? string.Empty;
        if (dilim.Length < 1)
        {
            return "Saat dilimi seçilmelidir.";
        }

        var izinli = new HashSet<string>(SaatDilimiService.SecilebilirDilimler(), StringComparer.Ordinal);
        if (!izinli.Contains(dilim))
        {
            return "Seçilen saat dilimi geçerli değil veya güncel dilim listesinde yok.";
        }

        return null;
    }
}
