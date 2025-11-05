using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projectSales.Server.Authorization;
using projectSales.Server.Common;
using projectSales.Server.Contracts.Catalogs;
using projectSales.Server.Infrastructure.Persistence;

namespace projectSales.Server.Controllers;

[ApiController]
[Route("categorias")]
[Authorize]
public class CategoriasController : ControllerBase
{
    private readonly SalesystemDbContext _dbContext;

    public CategoriasController(SalesystemDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    [HasPermission("Materiales", "Read")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CategoriaDto>>>> GetCategorias(CancellationToken cancellationToken)
    {
        var categorias = await _dbContext.CategoriasMateriales
            .OrderBy(c => c.Nombre)
            .ToListAsync(cancellationToken);

        var result = categorias.Select(c => new CategoriaDto(c.CategoriaId, c.Nombre, c.Descripcion, c.Estado));
        return Ok(ApiResponse<IEnumerable<CategoriaDto>>.Ok(result));
    }

    [HttpGet("{id:int}")]
    [HasPermission("Materiales", "Read")]
    public async Task<ActionResult<ApiResponse<CategoriaDto>>> GetCategoria(int id, CancellationToken cancellationToken)
    {
        var categoria = await _dbContext.CategoriasMateriales.FirstOrDefaultAsync(c => c.CategoriaId == id, cancellationToken);
        if (categoria is null)
        {
            return NotFound(ApiResponse<CategoriaDto>.Fail("Categoría no encontrada", code: 404));
        }

        var dto = new CategoriaDto(categoria.CategoriaId, categoria.Nombre, categoria.Descripcion, categoria.Estado);
        return Ok(ApiResponse<CategoriaDto>.Ok(dto));
    }

    [HttpPost]
    [HasPermission("Materiales", "Create")]
    public async Task<ActionResult<ApiResponse<CategoriaDto>>> CreateCategoria(CategoriaRequest request, CancellationToken cancellationToken)
    {
        if (await _dbContext.CategoriasMateriales.AnyAsync(c => c.Nombre == request.Nombre, cancellationToken))
        {
            return Conflict(ApiResponse<CategoriaDto>.Fail("La categoría ya existe", code: 409));
        }

        var now = DateTime.UtcNow;
        var categoria = new Domain.Entities.CategoriaMaterial
        {
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            Estado = request.Estado,
            FechaCreacion = now,
            FechaModificacion = now
        };

        _dbContext.CategoriasMateriales.Add(categoria);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var dto = new CategoriaDto(categoria.CategoriaId, categoria.Nombre, categoria.Descripcion, categoria.Estado);
        return CreatedAtAction(nameof(GetCategoria), new { id = categoria.CategoriaId }, ApiResponse<CategoriaDto>.Ok(dto));
    }

    [HttpPut("{id:int}")]
    [HasPermission("Materiales", "Update")]
    public async Task<ActionResult<ApiResponse<CategoriaDto>>> UpdateCategoria(int id, CategoriaRequest request, CancellationToken cancellationToken)
    {
        var categoria = await _dbContext.CategoriasMateriales.FirstOrDefaultAsync(c => c.CategoriaId == id, cancellationToken);
        if (categoria is null)
        {
            return NotFound(ApiResponse<CategoriaDto>.Fail("Categoría no encontrada", code: 404));
        }

        if (await _dbContext.CategoriasMateriales.AnyAsync(c => c.Nombre == request.Nombre && c.CategoriaId != id, cancellationToken))
        {
            return Conflict(ApiResponse<CategoriaDto>.Fail("La categoría ya existe", code: 409));
        }

        categoria.Nombre = request.Nombre;
        categoria.Descripcion = request.Descripcion;
        categoria.Estado = request.Estado;
        categoria.FechaModificacion = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        var dto = new CategoriaDto(categoria.CategoriaId, categoria.Nombre, categoria.Descripcion, categoria.Estado);
        return Ok(ApiResponse<CategoriaDto>.Ok(dto));
    }

    [HttpDelete("{id:int}")]
    [HasPermission("Materiales", "Delete")]
    public async Task<ActionResult<ApiResponse<string>>> DeleteCategoria(int id, CancellationToken cancellationToken)
    {
        var categoria = await _dbContext.CategoriasMateriales.FirstOrDefaultAsync(c => c.CategoriaId == id, cancellationToken);
        if (categoria is null)
        {
            return NotFound(ApiResponse<string>.Fail("Categoría no encontrada", code: 404));
        }

        categoria.Estado = false;
        categoria.FechaModificacion = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse<string>.Ok("Categoría deshabilitada"));
    }
}
