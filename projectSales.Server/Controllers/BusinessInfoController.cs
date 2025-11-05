using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projectSales.Server.Authorization;
using projectSales.Server.Common;
using projectSales.Server.Contracts.Business;
using projectSales.Server.Infrastructure.Persistence;

namespace projectSales.Server.Controllers;

[ApiController]
[Route("business-info")]
[Authorize]
public class BusinessInfoController : ControllerBase
{
    private readonly SalesystemDbContext _dbContext;

    public BusinessInfoController(SalesystemDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    [HasPermission("BusinessInfo", "Read")]
    public async Task<ActionResult<ApiResponse<BusinessInfoDto?>>> Get(CancellationToken cancellationToken)
    {
        var info = await _dbContext.BusinessInfo.FirstOrDefaultAsync(b => b.IsPrimary, cancellationToken);
        if (info is null)
        {
            return Ok(ApiResponse<BusinessInfoDto?>.Ok(null));
        }

        var dto = Map(info);
        return Ok(ApiResponse<BusinessInfoDto?>.Ok(dto));
    }

    [HttpPut]
    [HasPermission("BusinessInfo", "Update")]
    public async Task<ActionResult<ApiResponse<BusinessInfoDto>>> Update(UpdateBusinessInfoRequest request, CancellationToken cancellationToken)
    {
        var info = await _dbContext.BusinessInfo.FirstOrDefaultAsync(b => b.IsPrimary, cancellationToken);
        var now = DateTime.UtcNow;

        if (info is null)
        {
            info = new Domain.Entities.BusinessInfo
            {
                EmpresaNombre = request.EmpresaNombre,
                Nit = request.Nit,
                ContactoNombre = request.ContactoNombre,
                ContactoTelefono = request.ContactoTelefono,
                ContactoEmail = request.ContactoEmail,
                Direccion = request.Direccion,
                Ciudad = request.Ciudad,
                Pais = request.Pais,
                Web = request.Web,
                Facebook = request.Facebook,
                Instagram = request.Instagram,
                Tiktok = request.Tiktok,
                IsPrimary = true,
                FechaCreacion = now,
                FechaModificacion = now
            };
            _dbContext.BusinessInfo.Add(info);
        }
        else
        {
            info.EmpresaNombre = request.EmpresaNombre;
            info.Nit = request.Nit;
            info.ContactoNombre = request.ContactoNombre;
            info.ContactoTelefono = request.ContactoTelefono;
            info.ContactoEmail = request.ContactoEmail;
            info.Direccion = request.Direccion;
            info.Ciudad = request.Ciudad;
            info.Pais = request.Pais;
            info.Web = request.Web;
            info.Facebook = request.Facebook;
            info.Instagram = request.Instagram;
            info.Tiktok = request.Tiktok;
            info.IsPrimary = true;
            info.FechaModificacion = now;
        }

        var others = await _dbContext.BusinessInfo.Where(b => b.Id != info.Id && b.IsPrimary).ToListAsync(cancellationToken);
        foreach (var other in others)
        {
            other.IsPrimary = false;
            other.FechaModificacion = now;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        var dto = Map(info);
        return Ok(ApiResponse<BusinessInfoDto>.Ok(dto));
    }

    private static BusinessInfoDto Map(Domain.Entities.BusinessInfo info) => new(
        info.Id,
        info.EmpresaNombre,
        info.Nit,
        info.ContactoNombre,
        info.ContactoTelefono,
        info.ContactoEmail,
        info.Direccion,
        info.Ciudad,
        info.Pais,
        info.Web,
        info.Facebook,
        info.Instagram,
        info.Tiktok,
        info.IsPrimary);
}
