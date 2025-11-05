using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projectSales.Server.Authorization;
using projectSales.Server.Common;
using projectSales.Server.Contracts.Catalogs;
using projectSales.Server.Infrastructure.Persistence;

namespace projectSales.Server.Controllers;

[ApiController]
[Route("proveedores-marcas")]
[Authorize]
public class ProveedoresMarcasController : ControllerBase
{
    private readonly SalesystemDbContext _dbContext;

    public ProveedoresMarcasController(SalesystemDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    [HasPermission("Proveedores", "Read")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProveedorMarcaDto>>>> GetProveedoresMarcas(CancellationToken cancellationToken)
    {
        var items = await _dbContext.ProveedoresMarcas
            .Include(pm => pm.Marca)
            .Include(pm => pm.Proveedor)
            .OrderBy(pm => pm.Proveedor.Nombre)
            .ThenBy(pm => pm.Marca.Nombre)
            .ToListAsync(cancellationToken);

        var result = items.Select(pm => new ProveedorMarcaDto(pm.ProveedorCifId, pm.MarcaId, pm.Estado, pm.Marca.Nombre));
        return Ok(ApiResponse<IEnumerable<ProveedorMarcaDto>>.Ok(result));
    }

    [HttpPost]
    [HasPermission("Proveedores", "Create")]
    public async Task<ActionResult<ApiResponse<ProveedorMarcaDto>>> CreateProveedorMarca(ProveedorMarcaRequest request, CancellationToken cancellationToken)
    {
        if (!await _dbContext.Proveedores.AnyAsync(p => p.ProveedorCifId == request.ProveedorCifId, cancellationToken))
        {
            return BadRequest(ApiResponse<ProveedorMarcaDto>.Fail("Proveedor no válido", code: 400));
        }

        var marca = await _dbContext.Marcas.FirstOrDefaultAsync(m => m.MarcaId == request.MarcaId, cancellationToken);
        if (marca is null)
        {
            return BadRequest(ApiResponse<ProveedorMarcaDto>.Fail("Marca no válida", code: 400));
        }

        var existing = await _dbContext.ProveedoresMarcas.FirstOrDefaultAsync(pm => pm.ProveedorCifId == request.ProveedorCifId && pm.MarcaId == request.MarcaId, cancellationToken);
        if (existing is not null)
        {
            return Conflict(ApiResponse<ProveedorMarcaDto>.Fail("La relación ya existe", code: 409));
        }

        var now = DateTime.UtcNow;
        var relation = new Domain.Entities.ProveedorMarca
        {
            ProveedorCifId = request.ProveedorCifId,
            MarcaId = request.MarcaId,
            Estado = request.Estado,
            FechaCreacion = now,
            FechaModificacion = now
        };

        _dbContext.ProveedoresMarcas.Add(relation);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var dto = new ProveedorMarcaDto(relation.ProveedorCifId, relation.MarcaId, relation.Estado, marca.Nombre);
        return CreatedAtAction(nameof(GetProveedoresMarcas), ApiResponse<ProveedorMarcaDto>.Ok(dto));
    }

    [HttpPut]
    [HasPermission("Proveedores", "Update")]
    public async Task<ActionResult<ApiResponse<ProveedorMarcaDto>>> UpdateProveedorMarca(ProveedorMarcaRequest request, CancellationToken cancellationToken)
    {
        var relation = await _dbContext.ProveedoresMarcas
            .Include(pm => pm.Marca)
            .FirstOrDefaultAsync(pm => pm.ProveedorCifId == request.ProveedorCifId && pm.MarcaId == request.MarcaId, cancellationToken);

        if (relation is null)
        {
            return NotFound(ApiResponse<ProveedorMarcaDto>.Fail("Relación no encontrada", code: 404));
        }

        relation.Estado = request.Estado;
        relation.FechaModificacion = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);

        var dto = new ProveedorMarcaDto(relation.ProveedorCifId, relation.MarcaId, relation.Estado, relation.Marca.Nombre);
        return Ok(ApiResponse<ProveedorMarcaDto>.Ok(dto));
    }

    [HttpDelete]
    [HasPermission("Proveedores", "Delete")]
    public async Task<ActionResult<ApiResponse<string>>> DeleteProveedorMarca([FromQuery] string proveedorCifId, [FromQuery] int marcaId, CancellationToken cancellationToken)
    {
        var relation = await _dbContext.ProveedoresMarcas.FirstOrDefaultAsync(pm => pm.ProveedorCifId == proveedorCifId && pm.MarcaId == marcaId, cancellationToken);
        if (relation is null)
        {
            return NotFound(ApiResponse<string>.Fail("Relación no encontrada", code: 404));
        }

        relation.Estado = false;
        relation.FechaModificacion = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse<string>.Ok("Relación deshabilitada"));
    }
}
