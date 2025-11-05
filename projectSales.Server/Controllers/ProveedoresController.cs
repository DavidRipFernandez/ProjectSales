using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projectSales.Server.Authorization;
using projectSales.Server.Common;
using projectSales.Server.Contracts.Catalogs;
using projectSales.Server.Infrastructure.Persistence;

namespace projectSales.Server.Controllers;

[ApiController]
[Route("proveedores")]
[Authorize]
public class ProveedoresController : ControllerBase
{
    private readonly SalesystemDbContext _dbContext;

    public ProveedoresController(SalesystemDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    [HasPermission("Proveedores", "Read")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProveedorDto>>>> GetProveedores(CancellationToken cancellationToken)
    {
        var proveedores = await _dbContext.Proveedores
            .OrderBy(p => p.Nombre)
            .ToListAsync(cancellationToken);

        var result = proveedores.Select(p => new ProveedorDto(p.ProveedorCifId, p.Nombre, p.DomicilioSocial, p.Estado));
        return Ok(ApiResponse<IEnumerable<ProveedorDto>>.Ok(result));
    }

    [HttpGet("{id}")]
    [HasPermission("Proveedores", "Read")]
    public async Task<ActionResult<ApiResponse<ProveedorDto>>> GetProveedor(string id, CancellationToken cancellationToken)
    {
        var proveedor = await _dbContext.Proveedores.FirstOrDefaultAsync(p => p.ProveedorCifId == id, cancellationToken);
        if (proveedor is null)
        {
            return NotFound(ApiResponse<ProveedorDto>.Fail("Proveedor no encontrado", code: 404));
        }

        var dto = new ProveedorDto(proveedor.ProveedorCifId, proveedor.Nombre, proveedor.DomicilioSocial, proveedor.Estado);
        return Ok(ApiResponse<ProveedorDto>.Ok(dto));
    }

    [HttpPost]
    [HasPermission("Proveedores", "Create")]
    public async Task<ActionResult<ApiResponse<ProveedorDto>>> CreateProveedor(ProveedorRequest request, CancellationToken cancellationToken)
    {
        if (await _dbContext.Proveedores.AnyAsync(p => p.ProveedorCifId == request.ProveedorCifId, cancellationToken))
        {
            return Conflict(ApiResponse<ProveedorDto>.Fail("El proveedor ya existe", code: 409));
        }

        var now = DateTime.UtcNow;
        var proveedor = new Domain.Entities.Proveedor
        {
            ProveedorCifId = request.ProveedorCifId,
            Nombre = request.Nombre,
            DomicilioSocial = request.DomicilioSocial,
            Estado = request.Estado,
            FechaCreacion = now,
            FechaModificacion = now
        };

        _dbContext.Proveedores.Add(proveedor);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var dto = new ProveedorDto(proveedor.ProveedorCifId, proveedor.Nombre, proveedor.DomicilioSocial, proveedor.Estado);
        return CreatedAtAction(nameof(GetProveedor), new { id = proveedor.ProveedorCifId }, ApiResponse<ProveedorDto>.Ok(dto));
    }

    [HttpPut("{id}")]
    [HasPermission("Proveedores", "Update")]
    public async Task<ActionResult<ApiResponse<ProveedorDto>>> UpdateProveedor(string id, ProveedorRequest request, CancellationToken cancellationToken)
    {
        var proveedor = await _dbContext.Proveedores.FirstOrDefaultAsync(p => p.ProveedorCifId == id, cancellationToken);
        if (proveedor is null)
        {
            return NotFound(ApiResponse<ProveedorDto>.Fail("Proveedor no encontrado", code: 404));
        }

        proveedor.Nombre = request.Nombre;
        proveedor.DomicilioSocial = request.DomicilioSocial;
        proveedor.Estado = request.Estado;
        proveedor.FechaModificacion = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        var dto = new ProveedorDto(proveedor.ProveedorCifId, proveedor.Nombre, proveedor.DomicilioSocial, proveedor.Estado);
        return Ok(ApiResponse<ProveedorDto>.Ok(dto));
    }

    [HttpDelete("{id}")]
    [HasPermission("Proveedores", "Delete")]
    public async Task<ActionResult<ApiResponse<string>>> DeleteProveedor(string id, CancellationToken cancellationToken)
    {
        var proveedor = await _dbContext.Proveedores.FirstOrDefaultAsync(p => p.ProveedorCifId == id, cancellationToken);
        if (proveedor is null)
        {
            return NotFound(ApiResponse<string>.Fail("Proveedor no encontrado", code: 404));
        }

        proveedor.Estado = false;
        proveedor.FechaModificacion = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse<string>.Ok("Proveedor deshabilitado"));
    }
}
