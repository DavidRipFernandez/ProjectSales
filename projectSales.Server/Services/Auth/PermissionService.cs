using Microsoft.EntityFrameworkCore;
using projectSales.Server.Infrastructure.Persistence;

namespace projectSales.Server.Services.Auth;

public interface IPermissionService
{
    Task<bool> HasPermissionAsync(int userId, string moduleKey, string actionKey, CancellationToken cancellationToken);
    Task<Dictionary<string, List<string>>> GetPermissionsByModuleAsync(int userId, CancellationToken cancellationToken);
    Task<List<string>> GetRoleNamesForUserAsync(int userId, CancellationToken cancellationToken);
}

public class PermissionService : IPermissionService
{
    private readonly SalesystemDbContext _dbContext;

    public PermissionService(SalesystemDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> HasPermissionAsync(int userId, string moduleKey, string actionKey, CancellationToken cancellationToken)
    {
        return await _dbContext.RolePermissions
            .Where(rp => rp.Allowed && rp.Module.IsActive && rp.Action.IsActive)
            .Where(rp => rp.Module.Key == moduleKey && rp.Action.Key == actionKey)
            .AnyAsync(rp => rp.Role.UserRoles.Any(ur => ur.UserId == userId), cancellationToken);
    }

    public async Task<Dictionary<string, List<string>>> GetPermissionsByModuleAsync(int userId, CancellationToken cancellationToken)
    {
        var permissions = await _dbContext.RolePermissions
            .Where(rp => rp.Allowed && rp.Module.IsActive && rp.Action.IsActive)
            .Where(rp => rp.Role.UserRoles.Any(ur => ur.UserId == userId))
            .Select(rp => new { ModuleKey = rp.Module.Key, ActionKey = rp.Action.Key })
            .ToListAsync(cancellationToken);

        return permissions
            .GroupBy(x => x.ModuleKey)
            .ToDictionary(g => g.Key, g => g.Select(x => x.ActionKey).Distinct().ToList());
    }

    public async Task<List<string>> GetRoleNamesForUserAsync(int userId, CancellationToken cancellationToken)
    {
        return await _dbContext.UserRoles
            .Where(ur => ur.UserId == userId && ur.Role.Activo)
            .Select(ur => ur.Role.Nombre)
            .Distinct()
            .ToListAsync(cancellationToken);
    }
}
