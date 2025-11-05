using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projectSales.Server.Authorization;
using projectSales.Server.Common;
using projectSales.Server.Contracts.Catalogs;
using projectSales.Server.Infrastructure.Persistence;

namespace projectSales.Server.Controllers;

[ApiController]
[Route("marcas")]
[Authorize]
public class MarcasController : ControllerBase
{
    private readonly SalesystemDbContext _dbContext;

    public MarcasController(SalesystemDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    [HasPermission("Marcas", "Read")]
    public async Task<ActionResult<ApiResponse<IEnumerable<MarcaDto>>>> GetMarcas(CancellationToken cancellationToken)
    {
        var marcas = await _dbContext.Marcas
            .OrderBy(m => m.Nombre)
            .ToListAsync(cancellationToken);

        var result = marcas.Select(m => new MarcaDto(m.MarcaId, m.Nombre, m.Descripcion, m.Estado));
        return Ok(ApiResponse<IEnumerable<MarcaDto>>.Ok(result));
    }

    [HttpGet("{id:int}")]
    [HasPermission("Marcas", "Read")]
    public async Task<ActionResult<ApiResponse<MarcaDto>>> GetMarca(int id, CancellationToken cancellationToken)
    {
        var marca = await _dbContext.Marcas.FirstOrDefaultAsync(m => m.MarcaId == id, cancellationToken);
        if (marca is null)
        {
            return NotFound(ApiResponse<MarcaDto>.Fail("Marca no encontrada", code: 404));
        }

        var dto = new MarcaDto(marca.MarcaId, marca.Nombre, marca.Descripcion, marca.Estado);
        return Ok(ApiResponse<MarcaDto>.Ok(dto));
    }

    [HttpPost]
    [HasPermission("Marcas", "Create")]
    public async Task<ActionResult<ApiResponse<MarcaDto>>> CreateMarca(MarcaRequest request, CancellationToken cancellationToken)
    {
        if (await _dbContext.Marcas.AnyAsync(m => m.Nombre == request.Nombre, cancellationToken))
        {
            return Conflict(ApiResponse<MarcaDto>.Fail("La marca ya existe", code: 409));
        }

        var now = DateTime.UtcNow;
        var marca = new Domain.Entities.Marca
        {
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            Estado = request.Estado,
            FechaCreacion = now,
            FechaModificacion = now
        };

        _dbContext.Marcas.Add(marca);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var dto = new MarcaDto(marca.MarcaId, marca.Nombre, marca.Descripcion, marca.Estado);
        return CreatedAtAction(nameof(GetMarca), new { id = marca.MarcaId }, ApiResponse<MarcaDto>.Ok(dto));
    }

    [HttpPut("{id:int}")]
    [HasPermission("Marcas", "Update")]
    public async Task<ActionResult<ApiResponse<MarcaDto>>> UpdateMarca(int id, MarcaRequest request, CancellationToken cancellationToken)
    {
        var marca = await _dbContext.Marcas.FirstOrDefaultAsync(m => m.MarcaId == id, cancellationToken);
        if (marca is null)
        {
            return NotFound(ApiResponse<MarcaDto>.Fail("Marca no encontrada", code: 404));
        }

        if (await _dbContext.Marcas.AnyAsync(m => m.Nombre == request.Nombre && m.MarcaId != id, cancellationToken))
        {
            return Conflict(ApiResponse<MarcaDto>.Fail("La marca ya existe", code: 409));
        }

        marca.Nombre = request.Nombre;
        marca.Descripcion = request.Descripcion;
        marca.Estado = request.Estado;
        marca.FechaModificacion = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        var dto = new MarcaDto(marca.MarcaId, marca.Nombre, marca.Descripcion, marca.Estado);
        return Ok(ApiResponse<MarcaDto>.Ok(dto));
    }

    [HttpDelete("{id:int}")]
    [HasPermission("Marcas", "Delete")]
    public async Task<ActionResult<ApiResponse<string>>> DeleteMarca(int id, CancellationToken cancellationToken)
    {
        var marca = await _dbContext.Marcas.FirstOrDefaultAsync(m => m.MarcaId == id, cancellationToken);
        if (marca is null)
        {
            return NotFound(ApiResponse<string>.Fail("Marca no encontrada", code: 404));
        }

        marca.Estado = false;
        marca.FechaModificacion = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse<string>.Ok("Marca deshabilitada"));
    }
}
