namespace Fabrika.Api.Services;

/// <summary>
/// Gün içi 30 dakikalık dilim etiketleri ve “şu an / sonraki dilim” önerisi.
/// İstemci saatine güvenilmediğinde tek kaynak sunucu saati olur.
/// </summary>
public static class SaatDilimiService
{
    /// <summary>Gün içi dakika → "HH:MM" (üst sınır 24:00).</summary>
    public static string DakikayiSaate(int dk)
    {
        var d = Math.Clamp(dk, 0, 24 * 60);
        var h = d / 60;
        var m = d % 60;
        if (h >= 24)
        {
            return "24:00";
        }

        return $"{h:D2}:{m:D2}";
    }

    /// <summary>30 dk’lık dilim etiketi: "08:00-08:30".</summary>
    public static string DilimEtiketi(int baslangicDk)
    {
        var bitis = baslangicDk + 30;
        return $"{DakikayiSaate(baslangicDk)}-{DakikayiSaate(bitis)}";
    }

    /// <summary>Örn. 06:00–24:00 arası tüm 30 dk dilimleri (son dilim 23:30–24:00).</summary>
    public static IReadOnlyList<string> TumYariSaatDilimleri(int gunBaslangicSaat = 6, int gunBitisSaat = 24)
    {
        var liste = new List<string>();
        for (var dk = gunBaslangicSaat * 60; dk < gunBitisSaat * 60; dk += 30)
        {
            liste.Add(DilimEtiketi(dk));
        }

        return liste;
    }

    /// <summary>Şu anki saate göre içinde bulunulan dilim ve bir sonraki dilim.</summary>
    public static (string Birincil, string Alternatif) SimdikiDilimVeSonraki(DateTimeOffset? an = null)
    {
        var t = an ?? DateTimeOffset.Now;
        var toplamDk = t.Hour * 60 + t.Minute;
        var dilimBaslangic = toplamDk / 30 * 30;
        return (DilimEtiketi(dilimBaslangic), DilimEtiketi(dilimBaslangic + 30));
    }

    /// <summary>Vue’daki <c>dilimleriYenile</c> ile aynı sıra: önce önerilen/alternatif listede yoksa başa eklenir.</summary>
    public static IReadOnlyList<string> SecilebilirDilimler(DateTimeOffset? an = null)
    {
        var liste = TumYariSaatDilimleri().ToList();
        var (birincil, alternatif) = SimdikiDilimVeSonraki(an);
        foreach (var ek in new[] { birincil, alternatif })
        {
            if (!string.IsNullOrEmpty(ek) && !liste.Contains(ek))
            {
                liste.Insert(0, ek);
            }
        }

        return liste;
    }
}
