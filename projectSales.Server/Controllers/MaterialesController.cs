using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projectSales.Server.Authorization;
using projectSales.Server.Common;
using projectSales.Server.Contracts.Catalogs;
using projectSales.Server.Infrastructure.Persistence;

namespace projectSales.Server.Controllers;

[ApiController]
[Route("materiales")]
[Authorize]
public class MaterialesController : ControllerBase
{
    private readonly SalesystemDbContext _dbContext;

    public MaterialesController(SalesystemDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    [HasPermission("Materiales", "Read")]
    public async Task<ActionResult<ApiResponse<IEnumerable<MaterialDto>>>> GetMateriales([FromQuery] string? search, [FromQuery] int? categoriaId, [FromQuery] bool? estado, CancellationToken cancellationToken)
    {
        var query = _dbContext.Materiales
            .Include(m => m.Categoria)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(m => m.Nombre.Contains(term) || m.Codigo.Contains(term) || (m.Sku != null && m.Sku.Contains(term)));
        }

        if (categoriaId.HasValue)
        {
            query = query.Where(m => m.CategoriaId == categoriaId.Value);
        }

        if (estado.HasValue)
        {
            query = query.Where(m => m.Estado == estado.Value);
        }

        var materiales = await query
            .OrderBy(m => m.Nombre)
            .ToListAsync(cancellationToken);

        var result = materiales.Select(m => new MaterialDto(
            m.MaterialId,
            m.Codigo,
            m.Nombre,
            m.Descripcion,
            m.UnidadMedida,
            m.Sku,
            m.CategoriaId,
            m.Estado,
            m.Categoria?.Nombre));

        return Ok(ApiResponse<IEnumerable<MaterialDto>>.Ok(result));
    }

    [HttpGet("{id:int}")]
    [HasPermission("Materiales", "Read")]
    public async Task<ActionResult<ApiResponse<MaterialDto>>> GetMaterial(int id, CancellationToken cancellationToken)
    {
        var material = await _dbContext.Materiales
            .Include(m => m.Categoria)
            .FirstOrDefaultAsync(m => m.MaterialId == id, cancellationToken);

        if (material is null)
        {
            return NotFound(ApiResponse<MaterialDto>.Fail("Material no encontrado", code: 404));
        }

        var dto = new MaterialDto(material.MaterialId, material.Codigo, material.Nombre, material.Descripcion, material.UnidadMedida, material.Sku, material.CategoriaId, material.Estado, material.Categoria?.Nombre);
        return Ok(ApiResponse<MaterialDto>.Ok(dto));
    }

    [HttpPost]
    [HasPermission("Materiales", "Create")]
    public async Task<ActionResult<ApiResponse<MaterialDto>>> CreateMaterial(MaterialRequest request, CancellationToken cancellationToken)
    {
        if (await _dbContext.Materiales.AnyAsync(m => m.Codigo == request.Codigo, cancellationToken))
        {
            return Conflict(ApiResponse<MaterialDto>.Fail("El código ya existe", code: 409));
        }

        if (!string.IsNullOrWhiteSpace(request.Sku))
        {
            var sku = request.Sku.Trim();
            if (await _dbContext.Materiales.AnyAsync(m => m.Sku == sku, cancellationToken))
            {
                return Conflict(ApiResponse<MaterialDto>.Fail("El SKU ya existe", code: 409));
            }
        }

        var now = DateTime.UtcNow;
        var material = new Domain.Entities.Material
        {
            Codigo = request.Codigo,
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            UnidadMedida = request.UnidadMedida,
            Sku = string.IsNullOrWhiteSpace(request.Sku) ? null : request.Sku.Trim(),
            CategoriaId = request.CategoriaId,
            Estado = request.Estado,
            FechaCreacion = now,
            FechaModificacion = now
        };

        _dbContext.Materiales.Add(material);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await _dbContext.Entry(material).Reference(m => m.Categoria).LoadAsync(cancellationToken);

        var dto = new MaterialDto(material.MaterialId, material.Codigo, material.Nombre, material.Descripcion, material.UnidadMedida, material.Sku, material.CategoriaId, material.Estado, material.Categoria?.Nombre);
        return CreatedAtAction(nameof(GetMaterial), new { id = material.MaterialId }, ApiResponse<MaterialDto>.Ok(dto));
    }

    [HttpPut("{id:int}")]
    [HasPermission("Materiales", "Update")]
    public async Task<ActionResult<ApiResponse<MaterialDto>>> UpdateMaterial(int id, MaterialRequest request, CancellationToken cancellationToken)
    {
        var material = await _dbContext.Materiales.FirstOrDefaultAsync(m => m.MaterialId == id, cancellationToken);
        if (material is null)
        {
            return NotFound(ApiResponse<MaterialDto>.Fail("Material no encontrado", code: 404));
        }

        if (await _dbContext.Materiales.AnyAsync(m => m.Codigo == request.Codigo && m.MaterialId != id, cancellationToken))
        {
            return Conflict(ApiResponse<MaterialDto>.Fail("El código ya existe", code: 409));
        }

        var sku = string.IsNullOrWhiteSpace(request.Sku) ? null : request.Sku.Trim();
        if (!string.IsNullOrEmpty(sku))
        {
            if (await _dbContext.Materiales.AnyAsync(m => m.Sku == sku && m.MaterialId != id, cancellationToken))
            {
                return Conflict(ApiResponse<MaterialDto>.Fail("El SKU ya existe", code: 409));
            }
        }

        material.Codigo = request.Codigo;
        material.Nombre = request.Nombre;
        material.Descripcion = request.Descripcion;
        material.UnidadMedida = request.UnidadMedida;
        material.Sku = sku;
        material.CategoriaId = request.CategoriaId;
        material.Estado = request.Estado;
        material.FechaModificacion = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        await _dbContext.Entry(material).Reference(m => m.Categoria).LoadAsync(cancellationToken);

        var dto = new MaterialDto(material.MaterialId, material.Codigo, material.Nombre, material.Descripcion, material.UnidadMedida, material.Sku, material.CategoriaId, material.Estado, material.Categoria?.Nombre);
        return Ok(ApiResponse<MaterialDto>.Ok(dto));
    }

    [HttpDelete("{id:int}")]
    [HasPermission("Materiales", "Delete")]
    public async Task<ActionResult<ApiResponse<string>>> DeleteMaterial(int id, CancellationToken cancellationToken)
    {
        var material = await _dbContext.Materiales.FirstOrDefaultAsync(m => m.MaterialId == id, cancellationToken);
        if (material is null)
        {
            return NotFound(ApiResponse<string>.Fail("Material no encontrado", code: 404));
        }

        material.Estado = false;
        material.FechaModificacion = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse<string>.Ok("Material deshabilitado"));
    }
}
