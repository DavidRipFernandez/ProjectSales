using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projectSales.Server.Authorization;
using projectSales.Server.Common;
using projectSales.Server.Contracts.Rbac;
using projectSales.Server.Infrastructure.Persistence;
using projectSales.Server.Services.Auth;

namespace projectSales.Server.Controllers;

[ApiController]
[Route("users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly SalesystemDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;

    public UsersController(SalesystemDbContext dbContext, IPasswordHasher passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    [HttpGet]
    [HasPermission("Users", "Read")]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserDto>>>> GetUsers(CancellationToken cancellationToken)
    {
        var users = await _dbContext.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .OrderBy(u => u.Username)
            .ToListAsync(cancellationToken);

        var result = users.Select(u => new UserDto(
            u.Id,
            u.Username,
            u.Email,
            u.Activo,
            u.UserRoles.Select(ur => ur.Role.Nombre)));

        return Ok(ApiResponse<IEnumerable<UserDto>>.Ok(result));
    }

    [HttpGet("{id:int}")]
    [HasPermission("Users", "Read")]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetUser(int id, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user is null)
        {
            return NotFound(ApiResponse<UserDto>.Fail("Usuario no encontrado", code: 404));
        }

        var dto = new UserDto(user.Id, user.Username, user.Email, user.Activo, user.UserRoles.Select(ur => ur.Role.Nombre));
        return Ok(ApiResponse<UserDto>.Ok(dto));
    }

    [HttpPost]
    [HasPermission("Users", "Create")]
    public async Task<ActionResult<ApiResponse<UserDto>>> CreateUser(CreateUserRequest request, CancellationToken cancellationToken)
    {
        if (await _dbContext.Users.AnyAsync(u => u.Username == request.Username || u.Email == request.Email, cancellationToken))
        {
            return Conflict(ApiResponse<UserDto>.Fail("El usuario ya existe", code: 409));
        }

        var now = DateTime.UtcNow;
        var user = new Domain.Entities.User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            Activo = request.Activo,
            FechaCreacion = now,
            FechaModificacion = now
        };

        foreach (var roleId in request.RoleIds.Distinct())
        {
            if (!await _dbContext.Roles.AnyAsync(r => r.Id == roleId, cancellationToken))
            {
                return BadRequest(ApiResponse<UserDto>.Fail($"Rol no válido: {roleId}", code: 400));
            }

            user.UserRoles.Add(new Domain.Entities.UserRole
            {
                RoleId = roleId,
                User = user,
                FechaCreacion = now
            });
        }

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await _dbContext.Entry(user).Collection(u => u.UserRoles).Query().Include(ur => ur.Role).LoadAsync(cancellationToken);

        var dto = new UserDto(user.Id, user.Username, user.Email, user.Activo, user.UserRoles.Select(ur => ur.Role.Nombre));
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, ApiResponse<UserDto>.Ok(dto));
    }

    [HttpPut("{id:int}")]
    [HasPermission("Users", "Update")]
    public async Task<ActionResult<ApiResponse<UserDto>>> UpdateUser(int id, UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user is null)
        {
            return NotFound(ApiResponse<UserDto>.Fail("Usuario no encontrado", code: 404));
        }

        if (await _dbContext.Users.AnyAsync(u => (u.Username == request.Username || u.Email == request.Email) && u.Id != id, cancellationToken))
        {
            return Conflict(ApiResponse<UserDto>.Fail("El usuario ya existe", code: 409));
        }

        user.Username = request.Username;
        user.Email = request.Email;
        user.Activo = request.Activo;
        user.FechaModificacion = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            user.PasswordHash = _passwordHasher.HashPassword(request.Password);
        }

        var existingRoles = user.UserRoles.ToDictionary(ur => ur.RoleId);
        var desiredRoles = request.RoleIds.Distinct().ToHashSet();

        foreach (var roleId in desiredRoles)
        {
            if (!await _dbContext.Roles.AnyAsync(r => r.Id == roleId, cancellationToken))
            {
                return BadRequest(ApiResponse<UserDto>.Fail($"Rol no válido: {roleId}", code: 400));
            }

            if (!existingRoles.ContainsKey(roleId))
            {
                user.UserRoles.Add(new Domain.Entities.UserRole
                {
                    RoleId = roleId,
                    UserId = user.Id,
                    FechaCreacion = DateTime.UtcNow
                });
            }
        }

        foreach (var existing in existingRoles.Values)
        {
            if (!desiredRoles.Contains(existing.RoleId))
            {
                _dbContext.UserRoles.Remove(existing);
            }
            else
            {
                existing.FechaModificacion = DateTime.UtcNow;
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        await _dbContext.Entry(user).Collection(u => u.UserRoles).Query().Include(ur => ur.Role).LoadAsync(cancellationToken);

        var dto = new UserDto(user.Id, user.Username, user.Email, user.Activo, user.UserRoles.Select(ur => ur.Role.Nombre));
        return Ok(ApiResponse<UserDto>.Ok(dto));
    }

    [HttpDelete("{id:int}")]
    [HasPermission("Users", "Delete")]
    public async Task<ActionResult<ApiResponse<string>>> DeleteUser(int id, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (user is null)
        {
            return NotFound(ApiResponse<string>.Fail("Usuario no encontrado", code: 404));
        }

        user.Activo = false;
        user.FechaModificacion = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Ok(ApiResponse<string>.Ok("Usuario deshabilitado"));
    }

    [HttpGet("{id:int}/roles")]
    [HasPermission("Users", "Read")]
    public async Task<ActionResult<ApiResponse<IEnumerable<int>>>> GetUserRoles(int id, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user is null)
        {
            return NotFound(ApiResponse<IEnumerable<int>>.Fail("Usuario no encontrado", code: 404));
        }

        var roles = user.UserRoles.Select(ur => ur.RoleId);
        return Ok(ApiResponse<IEnumerable<int>>.Ok(roles));
    }

    [HttpPut("{id:int}/roles")]
    [HasPermission("Users", "Update")]
    public async Task<ActionResult<ApiResponse<string>>> UpdateUserRoles(int id, [FromBody] IEnumerable<int> roleIds, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user is null)
        {
            return NotFound(ApiResponse<string>.Fail("Usuario no encontrado", code: 404));
        }

        var validRoleIds = await _dbContext.Roles.Select(r => r.Id).ToListAsync(cancellationToken);
        var roleSet = roleIds.Distinct().ToHashSet();

        if (!roleSet.IsSubsetOf(validRoleIds))
        {
            return BadRequest(ApiResponse<string>.Fail("Roles inválidos", code: 400));
        }

        var existing = user.UserRoles.ToDictionary(ur => ur.RoleId);

        foreach (var roleId in roleSet)
        {
            if (!existing.ContainsKey(roleId))
            {
                user.UserRoles.Add(new Domain.Entities.UserRole
                {
                    RoleId = roleId,
                    UserId = user.Id,
                    FechaCreacion = DateTime.UtcNow
                });
            }
        }

        foreach (var role in existing.Keys.ToList())
        {
            if (!roleSet.Contains(role))
            {
                var relation = existing[role];
                _dbContext.UserRoles.Remove(relation);
            }
            else
            {
                existing[role].FechaModificacion = DateTime.UtcNow;
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Ok(ApiResponse<string>.Ok("Roles actualizados"));
    }
}
