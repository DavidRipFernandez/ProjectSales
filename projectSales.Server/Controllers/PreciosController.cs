using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projectSales.Server.Authorization;
using projectSales.Server.Common;
using projectSales.Server.Contracts.Catalogs;
using projectSales.Server.Infrastructure.Persistence;

namespace projectSales.Server.Controllers;

[ApiController]
[Route("precios")]
[Authorize]
public class PreciosController : ControllerBase
{
    private readonly SalesystemDbContext _dbContext;

    public PreciosController(SalesystemDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    [HasPermission("Precios", "Read")]
    public async Task<ActionResult<ApiResponse<IEnumerable<PrecioTarifaDto>>>> GetPrecios(CancellationToken cancellationToken)
    {
        var precios = await _dbContext.PreciosTarifas
            .Include(p => p.Material)
            .Include(p => p.Proveedor)
            .Include(p => p.Marca)
            .OrderBy(p => p.Material.Nombre)
            .ThenBy(p => p.Proveedor.Nombre)
            .ToListAsync(cancellationToken);

        var result = precios.Select(p => new PrecioTarifaDto(
            p.MaterialId,
            p.Material.Codigo,
            p.Material.Nombre,
            p.ProveedorCifId,
            p.Proveedor.Nombre,
            p.MarcaId,
            p.Marca.Nombre,
            p.Precio,
            p.Estado));

        return Ok(ApiResponse<IEnumerable<PrecioTarifaDto>>.Ok(result));
    }

    [HttpGet("material/{materialId:int}/proveedor/{proveedorId}/marca/{marcaId:int}")]
    [HasPermission("Precios", "Read")]
    public async Task<ActionResult<ApiResponse<PrecioTarifaDto>>> GetPrecio(int materialId, string proveedorId, int marcaId, CancellationToken cancellationToken)
    {
        var precio = await _dbContext.PreciosTarifas
            .Include(p => p.Material)
            .Include(p => p.Proveedor)
            .Include(p => p.Marca)
            .FirstOrDefaultAsync(p => p.MaterialId == materialId && p.ProveedorCifId == proveedorId && p.MarcaId == marcaId, cancellationToken);

        if (precio is null)
        {
            return NotFound(ApiResponse<PrecioTarifaDto>.Fail("Precio no encontrado", code: 404));
        }

        var dto = new PrecioTarifaDto(precio.MaterialId, precio.Material.Codigo, precio.Material.Nombre, precio.ProveedorCifId, precio.Proveedor.Nombre, precio.MarcaId, precio.Marca.Nombre, precio.Precio, precio.Estado);
        return Ok(ApiResponse<PrecioTarifaDto>.Ok(dto));
    }

    [HttpPost]
    [HasPermission("Precios", "Create")]
    public async Task<ActionResult<ApiResponse<PrecioTarifaDto>>> CreatePrecio(PrecioTarifaRequest request, CancellationToken cancellationToken)
    {
        if (request.Precio < 0)
        {
            return BadRequest(ApiResponse<PrecioTarifaDto>.Fail("El precio debe ser mayor o igual a 0", code: 400));
        }

        var material = await _dbContext.Materiales.FirstOrDefaultAsync(m => m.MaterialId == request.MaterialId, cancellationToken);
        if (material is null)
        {
            return BadRequest(ApiResponse<PrecioTarifaDto>.Fail("Material no válido", code: 400));
        }

        var proveedor = await _dbContext.Proveedores.FirstOrDefaultAsync(p => p.ProveedorCifId == request.ProveedorCifId, cancellationToken);
        if (proveedor is null)
        {
            return BadRequest(ApiResponse<PrecioTarifaDto>.Fail("Proveedor no válido", code: 400));
        }

        var marca = await _dbContext.Marcas.FirstOrDefaultAsync(m => m.MarcaId == request.MarcaId, cancellationToken);
        if (marca is null)
        {
            return BadRequest(ApiResponse<PrecioTarifaDto>.Fail("Marca no válida", code: 400));
        }

        var existing = await _dbContext.PreciosTarifas.FirstOrDefaultAsync(p => p.MaterialId == request.MaterialId && p.ProveedorCifId == request.ProveedorCifId && p.MarcaId == request.MarcaId, cancellationToken);
        if (existing is not null)
        {
            return Conflict(ApiResponse<PrecioTarifaDto>.Fail("El precio ya existe", code: 409));
        }

        var now = DateTime.UtcNow;
        var precio = new Domain.Entities.PrecioTarifa
        {
            MaterialId = request.MaterialId,
            ProveedorCifId = request.ProveedorCifId,
            MarcaId = request.MarcaId,
            Precio = request.Precio,
            Estado = request.Estado,
            FechaCreacion = now,
            FechaModificacion = now
        };

        _dbContext.PreciosTarifas.Add(precio);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var dto = new PrecioTarifaDto(precio.MaterialId, material.Codigo, material.Nombre, precio.ProveedorCifId, proveedor.Nombre, precio.MarcaId, marca.Nombre, precio.Precio, precio.Estado);
        return CreatedAtAction(nameof(GetPrecio), new { materialId = precio.MaterialId, proveedorId = precio.ProveedorCifId, marcaId = precio.MarcaId }, ApiResponse<PrecioTarifaDto>.Ok(dto));
    }

    [HttpPut]
    [HasPermission("Precios", "Update")]
    public async Task<ActionResult<ApiResponse<PrecioTarifaDto>>> UpdatePrecio(PrecioTarifaRequest request, CancellationToken cancellationToken)
    {
        if (request.Precio < 0)
        {
            return BadRequest(ApiResponse<PrecioTarifaDto>.Fail("El precio debe ser mayor o igual a 0", code: 400));
        }

        var precio = await _dbContext.PreciosTarifas
            .Include(p => p.Material)
            .Include(p => p.Proveedor)
            .Include(p => p.Marca)
            .FirstOrDefaultAsync(p => p.MaterialId == request.MaterialId && p.ProveedorCifId == request.ProveedorCifId && p.MarcaId == request.MarcaId, cancellationToken);

        if (precio is null)
        {
            return NotFound(ApiResponse<PrecioTarifaDto>.Fail("Precio no encontrado", code: 404));
        }

        precio.Precio = request.Precio;
        precio.Estado = request.Estado;
        precio.FechaModificacion = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);

        var dto = new PrecioTarifaDto(precio.MaterialId, precio.Material.Codigo, precio.Material.Nombre, precio.ProveedorCifId, precio.Proveedor.Nombre, precio.MarcaId, precio.Marca.Nombre, precio.Precio, precio.Estado);
        return Ok(ApiResponse<PrecioTarifaDto>.Ok(dto));
    }

    [HttpDelete]
    [HasPermission("Precios", "Delete")]
    public async Task<ActionResult<ApiResponse<string>>> DeletePrecio([FromQuery] int materialId, [FromQuery] string proveedorId, [FromQuery] int marcaId, CancellationToken cancellationToken)
    {
        var precio = await _dbContext.PreciosTarifas.FirstOrDefaultAsync(p => p.MaterialId == materialId && p.ProveedorCifId == proveedorId && p.MarcaId == marcaId, cancellationToken);
        if (precio is null)
        {
            return NotFound(ApiResponse<string>.Fail("Precio no encontrado", code: 404));
        }

        precio.Estado = false;
        precio.FechaModificacion = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse<string>.Ok("Precio deshabilitado"));
    }
}
