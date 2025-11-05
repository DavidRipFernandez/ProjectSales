using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projectSales.Server.Authorization;
using projectSales.Server.Common;
using projectSales.Server.Contracts.Rbac;
using projectSales.Server.Infrastructure.Persistence;

namespace projectSales.Server.Controllers;

[ApiController]
[Route("roles")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly SalesystemDbContext _dbContext;

    public RolesController(SalesystemDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    [HasPermission("Roles", "Read")]
    public async Task<ActionResult<ApiResponse<IEnumerable<RoleDto>>>> GetRoles(CancellationToken cancellationToken)
    {
        var roles = await _dbContext.Roles
            .OrderBy(r => r.Nombre)
            .ToListAsync(cancellationToken);

        var result = roles.Select(r => new RoleDto(r.Id, r.Nombre, r.Descripcion, r.Activo));
        return Ok(ApiResponse<IEnumerable<RoleDto>>.Ok(result));
    }

    [HttpGet("{id:int}")]
    [HasPermission("Roles", "Read")]
    public async Task<ActionResult<ApiResponse<RoleDto>>> GetRole(int id, CancellationToken cancellationToken)
    {
        var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        if (role is null)
        {
            return NotFound(ApiResponse<RoleDto>.Fail("Rol no encontrado", code: 404));
        }

        return Ok(ApiResponse<RoleDto>.Ok(new RoleDto(role.Id, role.Nombre, role.Descripcion, role.Activo)));
    }

    [HttpPost]
    [HasPermission("Roles", "Create")]
    public async Task<ActionResult<ApiResponse<RoleDto>>> CreateRole(RoleRequest request, CancellationToken cancellationToken)
    {
        if (await _dbContext.Roles.AnyAsync(r => r.Nombre == request.Nombre, cancellationToken))
        {
            return Conflict(ApiResponse<RoleDto>.Fail("El rol ya existe", code: 409));
        }

        var now = DateTime.UtcNow;
        var role = new Domain.Entities.Role
        {
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            Activo = request.Activo,
            FechaCreacion = now,
            FechaModificacion = now
        };

        _dbContext.Roles.Add(role);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var dto = new RoleDto(role.Id, role.Nombre, role.Descripcion, role.Activo);
        return CreatedAtAction(nameof(GetRole), new { id = role.Id }, ApiResponse<RoleDto>.Ok(dto));
    }

    [HttpPut("{id:int}")]
    [HasPermission("Roles", "Update")]
    public async Task<ActionResult<ApiResponse<RoleDto>>> UpdateRole(int id, RoleRequest request, CancellationToken cancellationToken)
    {
        var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        if (role is null)
        {
            return NotFound(ApiResponse<RoleDto>.Fail("Rol no encontrado", code: 404));
        }

        if (await _dbContext.Roles.AnyAsync(r => r.Nombre == request.Nombre && r.Id != id, cancellationToken))
        {
            return Conflict(ApiResponse<RoleDto>.Fail("El rol ya existe", code: 409));
        }

        role.Nombre = request.Nombre;
        role.Descripcion = request.Descripcion;
        role.Activo = request.Activo;
        role.FechaModificacion = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        var dto = new RoleDto(role.Id, role.Nombre, role.Descripcion, role.Activo);
        return Ok(ApiResponse<RoleDto>.Ok(dto));
    }

    [HttpDelete("{id:int}")]
    [HasPermission("Roles", "Delete")]
    public async Task<ActionResult<ApiResponse<string>>> DeleteRole(int id, CancellationToken cancellationToken)
    {
        var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        if (role is null)
        {
            return NotFound(ApiResponse<string>.Fail("Rol no encontrado", code: 404));
        }

        role.Activo = false;
        role.FechaModificacion = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Ok(ApiResponse<string>.Ok("Rol deshabilitado"));
    }

    [HttpGet("{id:int}/permissions")]
    [HasPermission("Permissions", "Read")]
    public async Task<ActionResult<ApiResponse<IEnumerable<RolePermissionDto>>>> GetRolePermissions(int id, CancellationToken cancellationToken)
    {
        var roleExists = await _dbContext.Roles.AnyAsync(r => r.Id == id, cancellationToken);
        if (!roleExists)
        {
            return NotFound(ApiResponse<IEnumerable<RolePermissionDto>>.Fail("Rol no encontrado", code: 404));
        }

        var permissions = await _dbContext.RolePermissions
            .Where(p => p.RoleId == id)
            .ToListAsync(cancellationToken);

        var result = permissions.Select(p => new RolePermissionDto(p.ModuleId, p.ActionId, p.Allowed));
        return Ok(ApiResponse<IEnumerable<RolePermissionDto>>.Ok(result));
    }

    [HttpPut("{id:int}/permissions")]
    [HasPermission("Permissions", "Update")]
    public async Task<ActionResult<ApiResponse<string>>> UpdateRolePermissions(int id, [FromBody] IEnumerable<RolePermissionRequest> request, CancellationToken cancellationToken)
    {
        var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        if (role is null)
        {
            return NotFound(ApiResponse<string>.Fail("Rol no encontrado", code: 404));
        }

        var moduleMap = await _dbContext.Modules.ToDictionaryAsync(m => m.Key, cancellationToken);
        var actionMap = await _dbContext.Actions.ToDictionaryAsync(a => (a.ModuleId, a.Key), cancellationToken);

        var existing = await _dbContext.RolePermissions
            .Where(rp => rp.RoleId == id)
            .ToListAsync(cancellationToken);

        var requested = new HashSet<(int ModuleId, int ActionId)>();

        foreach (var rp in request)
        {
            if (!moduleMap.TryGetValue(rp.ModuleKey, out var module))
            {
                return BadRequest(ApiResponse<string>.Fail($"Módulo no válido: {rp.ModuleKey}", code: 400));
            }

            if (!actionMap.TryGetValue((module.Id, rp.ActionKey), out var action))
            {
                return BadRequest(ApiResponse<string>.Fail($"Acción no válida: {rp.ActionKey}", code: 400));
            }

            requested.Add((module.Id, action.Id));

            var current = existing.FirstOrDefault(e => e.ModuleId == module.Id && e.ActionId == action.Id);
            if (current is null)
            {
                current = new Domain.Entities.RolePermission
                {
                    RoleId = id,
                    ModuleId = module.Id,
                    ActionId = action.Id,
                    Allowed = rp.Allowed,
                    FechaCreacion = DateTime.UtcNow
                };
                _dbContext.RolePermissions.Add(current);
                existing.Add(current);
            }
            else
            {
                current.Allowed = rp.Allowed;
                current.FechaModificacion = DateTime.UtcNow;
            }
        }

        foreach (var permission in existing)
        {
            if (!requested.Contains((permission.ModuleId, permission.ActionId)))
            {
                _dbContext.RolePermissions.Remove(permission);
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Ok(ApiResponse<string>.Ok("Permisos actualizados"));
    }
}
