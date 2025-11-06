using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projectSales.Server.Authorization;
using projectSales.Server.Common;
using projectSales.Server.Contracts.Catalogs;
using projectSales.Server.Infrastructure.Persistence;

namespace projectSales.Server.Controllers;

[ApiController]
[Route("proveedores/{proveedorId}/contactos")]
[Authorize]
public class ContactosController : ControllerBase
{
    private readonly SalesystemDbContext _dbContext;

    public ContactosController(SalesystemDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    [HasPermission("Proveedores", "Read")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ContactoDto>>>> GetContactos(string proveedorId, CancellationToken cancellationToken)
    {
        if (!await _dbContext.Proveedores.AnyAsync(p => p.ProveedorCifId == proveedorId, cancellationToken))
        {
            return NotFound(ApiResponse<IEnumerable<ContactoDto>>.Fail("Proveedor no encontrado", code: 404));
        }

        var contactos = await _dbContext.Contactos
            .Where(c => c.ProveedorCifId == proveedorId)
            .OrderBy(c => c.Nombre)
            .ToListAsync(cancellationToken);

        var result = contactos.Select(c => new ContactoDto(c.ContactoId, c.Nombre, c.Correo, c.Telefono, c.Descripcion, c.Estado));
        return Ok(ApiResponse<IEnumerable<ContactoDto>>.Ok(result));
    }

    [HttpGet("{id:int}")]
    [HasPermission("Proveedores", "Read")]
    public async Task<ActionResult<ApiResponse<ContactoDto>>> GetContacto(string proveedorId, int id, CancellationToken cancellationToken)
    {
        var contacto = await _dbContext.Contactos.FirstOrDefaultAsync(c => c.ContactoId == id && c.ProveedorCifId == proveedorId, cancellationToken);
        if (contacto is null)
        {
            return NotFound(ApiResponse<ContactoDto>.Fail("Contacto no encontrado", code: 404));
        }

        var dto = new ContactoDto(contacto.ContactoId, contacto.Nombre, contacto.Correo, contacto.Telefono, contacto.Descripcion, contacto.Estado);
        return Ok(ApiResponse<ContactoDto>.Ok(dto));
    }

    [HttpPost]
    [HasPermission("Proveedores", "Create")]
    public async Task<ActionResult<ApiResponse<ContactoDto>>> CreateContacto(string proveedorId, ContactoRequest request, CancellationToken cancellationToken)
    {
        var proveedor = await _dbContext.Proveedores.FirstOrDefaultAsync(p => p.ProveedorCifId == proveedorId, cancellationToken);
        if (proveedor is null)
        {
            return NotFound(ApiResponse<ContactoDto>.Fail("Proveedor no encontrado", code: 404));
        }

        var now = DateTime.UtcNow;
        var contacto = new Domain.Entities.Contacto
        {
            ProveedorCifId = proveedorId,
            Nombre = request.Nombre,
            Correo = request.Correo,
            Telefono = request.Telefono,
            Descripcion = request.Descripcion,
            Estado = request.Estado,
            FechaCreacion = now,
            FechaModificacion = now
        };

        _dbContext.Contactos.Add(contacto);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var dto = new ContactoDto(contacto.ContactoId, contacto.Nombre, contacto.Correo, contacto.Telefono, contacto.Descripcion, contacto.Estado);
        return CreatedAtAction(nameof(GetContacto), new { proveedorId, id = contacto.ContactoId }, ApiResponse<ContactoDto>.Ok(dto));
    }

    [HttpPut("{id:int}")]
    [HasPermission("Proveedores", "Update")]
    public async Task<ActionResult<ApiResponse<ContactoDto>>> UpdateContacto(string proveedorId, int id, ContactoRequest request, CancellationToken cancellationToken)
    {
        var contacto = await _dbContext.Contactos.FirstOrDefaultAsync(c => c.ContactoId == id && c.ProveedorCifId == proveedorId, cancellationToken);
        if (contacto is null)
        {
            return NotFound(ApiResponse<ContactoDto>.Fail("Contacto no encontrado", code: 404));
        }

        contacto.Nombre = request.Nombre;
        contacto.Correo = request.Correo;
        contacto.Telefono = request.Telefono;
        contacto.Descripcion = request.Descripcion;
        contacto.Estado = request.Estado;
        contacto.FechaModificacion = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        var dto = new ContactoDto(contacto.ContactoId, contacto.Nombre, contacto.Correo, contacto.Telefono, contacto.Descripcion, contacto.Estado);
        return Ok(ApiResponse<ContactoDto>.Ok(dto));
    }

    [HttpDelete("{id:int}")]
    [HasPermission("Proveedores", "Delete")]
    public async Task<ActionResult<ApiResponse<string>>> DeleteContacto(string proveedorId, int id, CancellationToken cancellationToken)
    {
        var contacto = await _dbContext.Contactos.FirstOrDefaultAsync(c => c.ContactoId == id && c.ProveedorCifId == proveedorId, cancellationToken);
        if (contacto is null)
        {
            return NotFound(ApiResponse<string>.Fail("Contacto no encontrado", code: 404));
        }

        contacto.Estado = false;
        contacto.FechaModificacion = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Ok(ApiResponse<string>.Ok("Contacto deshabilitado"));
    }
}
