using Fabrika.Api.Contracts;
using Fabrika.Api.Data;
using Fabrika.Api.Models;
using Fabrika.Api.Services;
using Fabrika.Api.Validation;
using Microsoft.AspNetCore.Mvc;

namespace Fabrika.Api.Controllers;

[ApiController]
[Route("api/uretim")]
public class UretimController(ApplicationDbContext db) : ControllerBase
{
    /// <summary>
    /// Sunucu saatine göre seçilebilir dilim listesi ve önerilen iki dilim.
    /// İstemci bu yanıtı gösterir; böylece dilim mantığı tek yerde kalır.
    /// </summary>
    [HttpGet("saat-dilimleri")]
    [ProducesResponseType(typeof(SaatDilimleriResponse), StatusCodes.Status200OK)]
    public ActionResult<SaatDilimleriResponse> GetSaatDilimleri()
    {
        var tumDilimler = SaatDilimiService.SecilebilirDilimler();
        var (onerilenDilim, alternatifDilim) = SaatDilimiService.SimdikiDilimVeSonraki();
        return Ok(new SaatDilimleriResponse(tumDilimler, onerilenDilim, alternatifDilim));
    }

    /// <summary>
    /// Form doğrulaması sunucuda tekrarlanır; geçerliyse veritabanına yazılır.
    /// </summary>
    [HttpPost("kayitlar")]
    [ProducesResponseType(typeof(UretimKayitYanit), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UretimKayitYanit>> PostKayit(
        [FromBody] UretimKayitIstek body,
        CancellationToken cancellationToken)
    {
        var hata = UretimKayitValidator.Validate(body);
        if (hata is not null)
        {
            return BadRequest(new { error = hata });
        }

        var makineAdi = body.MakineAdi.Trim();
        var now = DateTimeOffset.UtcNow;

        var kayit = new UretimKaydi
        {
            CalisanId = body.CalisanId,
            VeriTipi = body.VeriTipi,
            MakineTipi = body.MakineTipi,
            MakineAdi = makineAdi,
            DirencDegeri = body.VeriTipi == "Direnç" ? body.DirencDegeri : null,
            PlaceholderMetin = body.VeriTipi == "Placeholder" ? body.PlaceholderMetin!.Trim() : null,
            SaatDilimi = body.SaatDilimi.Trim(),
            OlusturulmaUtc = now,
        };

        db.UretimKayitlari.Add(kayit);
        await db.SaveChangesAsync(cancellationToken);

        return Ok(new UretimKayitYanit(kayit.Id));
    }
}
