using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projectSales.Server.Common;
using projectSales.Server.Contracts.Rbac;
using projectSales.Server.Infrastructure.Persistence;

namespace projectSales.Server.Controllers;

[ApiController]
[Route("modules")]
[Authorize]
public class ModulesController : ControllerBase
{
    private readonly SalesystemDbContext _dbContext;

    public ModulesController(SalesystemDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<ModuleDto>>>> GetModules(CancellationToken cancellationToken)
    {
        var modules = await _dbContext.Modules
            .Include(m => m.Actions)
            .Where(m => m.IsActive)
            .OrderBy(m => m.Name)
            .ToListAsync(cancellationToken);

        var result = modules.Select(m => new ModuleDto(
            m.Id,
            m.Key,
            m.Name,
            m.IsActive,
            m.Actions
                .OrderBy(a => a.SortOrder)
                .Select(a => new ModuleActionDto(a.Id, a.Key, a.Name, a.SortOrder, a.IsActive))));

        return Ok(ApiResponse<IEnumerable<ModuleDto>>.Ok(result));
    }
}
