using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using projectSales.Server.Domain.Entities;
using projectSales.Server.Infrastructure.Persistence;
using projectSales.Server.Services.Auth;

namespace projectSales.Server.Infrastructure.Seeding;

public static class SeedData
{
    public static async Task RunAsync(IServiceProvider serviceProvider)
    {
        var dbContext = serviceProvider.GetRequiredService<SalesystemDbContext>();
        var passwordHasher = serviceProvider.GetRequiredService<IPasswordHasher>();

        await SeedRolesAsync(dbContext);
        await SeedUsersAsync(dbContext, passwordHasher);
        await SeedUserRolesAsync(dbContext);
        await SeedModulesAsync(dbContext);
        await SeedActionsAsync(dbContext);
        await SeedRolePermissionsAsync(dbContext);
        await SeedCatalogsAsync(dbContext);
        await SeedBusinessInfoAsync(dbContext);
        await SeedDatabaseObjectsAsync(dbContext);
    }

    private static async Task SeedRolesAsync(SalesystemDbContext dbContext)
    {
        var now = DateTime.UtcNow;
        var roles = new[]
        {
            new Role { Nombre = "Master", Descripcion = "Acceso total", Activo = true, FechaCreacion = now, FechaModificacion = now },
            new Role { Nombre = "Ventas", Descripcion = "Rol de ventas", Activo = true, FechaCreacion = now, FechaModificacion = now }
        };

        foreach (var role in roles)
        {
            if (!await dbContext.Roles.AnyAsync(r => r.Nombre == role.Nombre))
            {
                dbContext.Roles.Add(role);
                await dbContext.SaveChangesAsync();
            }
        }
    }

    private static async Task SeedUsersAsync(SalesystemDbContext dbContext, IPasswordHasher passwordHasher)
    {
        var now = DateTime.UtcNow;
        if (!await dbContext.Users.AnyAsync(u => u.Username == "master"))
        {
            dbContext.Users.Add(new User
            {
                Username = "master",
                Email = "master@gessoplaca.local",
                PasswordHash = passwordHasher.HashPassword("admin"),
                Activo = true,
                FechaCreacion = now,
                FechaModificacion = now
            });
            await dbContext.SaveChangesAsync();
        }

        if (!await dbContext.Users.AnyAsync(u => u.Username == "ventas"))
        {
            dbContext.Users.Add(new User
            {
                Username = "ventas",
                Email = "ventas@gmail.com",
                PasswordHash = passwordHasher.HashPassword("@@ventas2025"),
                Activo = true,
                FechaCreacion = now,
                FechaModificacion = now
            });
            await dbContext.SaveChangesAsync();
        }
    }

    private static async Task SeedUserRolesAsync(SalesystemDbContext dbContext)
    {
        var master = await dbContext.Users.FirstAsync(u => u.Username == "master");
        var ventas = await dbContext.Users.FirstAsync(u => u.Username == "ventas");
        var masterRole = await dbContext.Roles.FirstAsync(r => r.Nombre == "Master");
        var ventasRole = await dbContext.Roles.FirstAsync(r => r.Nombre == "Ventas");
        var now = DateTime.UtcNow;

        if (!await dbContext.UserRoles.AnyAsync(ur => ur.UserId == master.Id && ur.RoleId == masterRole.Id))
        {
            dbContext.UserRoles.Add(new UserRole
            {
                UserId = master.Id,
                RoleId = masterRole.Id,
                FechaCreacion = now,
                FechaModificacion = now
            });
            await dbContext.SaveChangesAsync();
        }

        if (!await dbContext.UserRoles.AnyAsync(ur => ur.UserId == ventas.Id && ur.RoleId == ventasRole.Id))
        {
            dbContext.UserRoles.Add(new UserRole
            {
                UserId = ventas.Id,
                RoleId = ventasRole.Id,
                FechaCreacion = now,
                FechaModificacion = now
            });
            await dbContext.SaveChangesAsync();
        }
    }

    private static async Task SeedModulesAsync(SalesystemDbContext dbContext)
    {
        var now = DateTime.UtcNow;
        var modules = new (string Key, string Name)[]
        {
            ("Users", "Usuarios"),
            ("Roles", "Roles"),
            ("Permissions", "Permisos"),
            ("Materiales", "Materiales"),
            ("Proveedores", "Proveedores"),
            ("Marcas", "Marcas"),
            ("Precios", "Precios"),
            ("BusinessInfo", "Información de la empresa")
        };

        foreach (var (key, name) in modules)
        {
            var module = await dbContext.Modules.FirstOrDefaultAsync(m => m.Key == key);
            if (module is null)
            {
                dbContext.Modules.Add(new Module
                {
                    Key = key,
                    Name = name,
                    IsActive = true,
                    FechaCreacion = now,
                    FechaModificacion = now
                });
                await dbContext.SaveChangesAsync();
            }
        }
    }

    private static async Task SeedActionsAsync(SalesystemDbContext dbContext)
    {
        var now = DateTime.UtcNow;
        var actions = new (string Key, string Name, int SortOrder)[]
        {
            ("Read", "Read", 0),
            ("Create", "Create", 1),
            ("Update", "Update", 2),
            ("Delete", "Delete", 3)
        };

        var modules = await dbContext.Modules.ToListAsync();
        foreach (var module in modules)
        {
            foreach (var (key, name, sortOrder) in actions)
            {
                if (!await dbContext.Actions.AnyAsync(a => a.ModuleId == module.Id && a.Key == key))
                {
                    dbContext.Actions.Add(new ModuleAction
                    {
                        ModuleId = module.Id,
                        Key = key,
                        Name = name,
                        SortOrder = sortOrder,
                        IsActive = true,
                        FechaCreacion = now,
                        FechaModificacion = now
                    });
                    await dbContext.SaveChangesAsync();
                }
            }
        }
    }

    private static async Task SeedRolePermissionsAsync(SalesystemDbContext dbContext)
    {
        var now = DateTime.UtcNow;
        var masterRole = await dbContext.Roles.FirstAsync(r => r.Nombre == "Master");
        var ventasRole = await dbContext.Roles.FirstAsync(r => r.Nombre == "Ventas");
        var modules = await dbContext.Modules.Include(m => m.Actions).ToListAsync();

        foreach (var module in modules)
        {
            foreach (var action in module.Actions)
            {
                await EnsurePermissionAsync(dbContext, masterRole.Id, module.Id, action.Id, true, now);
            }
        }

        var ventasPermissions = new Dictionary<string, string[]>
        {
            ["Materiales"] = new[] { "Read", "Create", "Update" },
            ["Proveedores"] = new[] { "Read", "Create", "Update" },
            ["Marcas"] = new[] { "Read" },
            ["Precios"] = new[] { "Read", "Create", "Update" },
            ["BusinessInfo"] = new[] { "Read" }
        };

        foreach (var module in modules)
        {
            var allowedActions = ventasPermissions.TryGetValue(module.Key, out var keys)
                ? keys
                : Array.Empty<string>();

            foreach (var action in module.Actions)
            {
                var allowed = allowedActions.Contains(action.Key);
                await EnsurePermissionAsync(dbContext, ventasRole.Id, module.Id, action.Id, allowed, now);
            }
        }
    }

    private static async Task EnsurePermissionAsync(SalesystemDbContext dbContext, int roleId, int moduleId, int actionId, bool allowed, DateTime now)
    {
        var permission = await dbContext.RolePermissions.FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.ModuleId == moduleId && rp.ActionId == actionId);
        if (permission is null)
        {
            dbContext.RolePermissions.Add(new RolePermission
            {
                RoleId = roleId,
                ModuleId = moduleId,
                ActionId = actionId,
                Allowed = allowed,
                FechaCreacion = now,
                FechaModificacion = now
            });
            await dbContext.SaveChangesAsync();
        }
        else if (permission.Allowed != allowed)
        {
            permission.Allowed = allowed;
            permission.FechaModificacion = now;
            await dbContext.SaveChangesAsync();
        }
    }

    private static async Task SeedCatalogsAsync(SalesystemDbContext dbContext)
    {
        var now = DateTime.UtcNow;

        if (!await dbContext.Marcas.AnyAsync(m => m.Nombre == "GessoLux"))
        {
            dbContext.Marcas.Add(new Marca { Nombre = "GessoLux", Descripcion = "Línea premium de placas de yeso", Estado = true, FechaCreacion = now, FechaModificacion = now });
            await dbContext.SaveChangesAsync();
        }

        if (!await dbContext.Marcas.AnyAsync(m => m.Nombre == "Construmax"))
        {
            dbContext.Marcas.Add(new Marca { Nombre = "Construmax", Descripcion = "Marca de perfiles y accesorios", Estado = true, FechaCreacion = now, FechaModificacion = now });
            await dbContext.SaveChangesAsync();
        }

        var categorias = new (string Nombre, string Descripcion)[]
        {
            ("Cemento", "Productos cementicios"),
            ("Yeso", "Productos a base de yeso"),
            ("Placas", "Paneles y placas"),
            ("Perfiles", "Perfiles metálicos")
        };

        foreach (var (nombre, descripcion) in categorias)
        {
            if (!await dbContext.CategoriasMateriales.AnyAsync(c => c.Nombre == nombre))
            {
                dbContext.CategoriasMateriales.Add(new CategoriaMaterial
                {
                    Nombre = nombre,
                    Descripcion = descripcion,
                    Estado = true,
                    FechaCreacion = now,
                    FechaModificacion = now
                });
                await dbContext.SaveChangesAsync();
            }
        }

        var categoriasLookup = await dbContext.CategoriasMateriales.ToDictionaryAsync(c => c.Nombre, c => c.CategoriaId);

        var materiales = new[]
        {
            new { Codigo = "MAT-001", Nombre = "Cemento Portland 50kg", UM = "KG", SKU = "CEM50", Categoria = "Cemento" },
            new { Codigo = "MAT-002", Nombre = "Yeso Construcción 40kg", UM = "KG", SKU = "YESO40", Categoria = "Yeso" },
            new { Codigo = "MAT-003", Nombre = "Placa de yeso 1.2x2.4m", UM = "M2", SKU = "PLG1224", Categoria = "Placas" },
            new { Codigo = "MAT-004", Nombre = "Perfil metálico 2.4m", UM = "UND", SKU = "PERF24", Categoria = "Perfiles" }
        };

        foreach (var material in materiales)
        {
            if (!await dbContext.Materiales.AnyAsync(m => m.Codigo == material.Codigo))
            {
                dbContext.Materiales.Add(new Material
                {
                    Codigo = material.Codigo,
                    Nombre = material.Nombre,
                    UnidadMedida = material.UM,
                    Sku = material.SKU,
                    CategoriaId = categoriasLookup[material.Categoria],
                    Estado = true,
                    FechaCreacion = now,
                    FechaModificacion = now
                });
                await dbContext.SaveChangesAsync();
            }
        }

        var proveedores = new[]
        {
            new { Id = "NIT-PA-001", Nombre = "Proveedora Andina S.A.", Domicilio = "Av. 4to Anillo #456" },
            new { Id = "NIT-DSC-002", Nombre = "Distribuidora Santa Cruz", Domicilio = "Av. Banzer #789" }
        };

        foreach (var proveedor in proveedores)
        {
            if (!await dbContext.Proveedores.AnyAsync(p => p.ProveedorCifId == proveedor.Id))
            {
                dbContext.Proveedores.Add(new Proveedor
                {
                    ProveedorCifId = proveedor.Id,
                    Nombre = proveedor.Nombre,
                    DomicilioSocial = proveedor.Domicilio,
                    Estado = true,
                    FechaCreacion = now,
                    FechaModificacion = now
                });
                await dbContext.SaveChangesAsync();
            }
        }

        var contactos = new[]
        {
            new { Proveedor = "NIT-PA-001", Nombre = "Laura Pérez", Correo = "laura@proandina.bo", Telefono = "+591 710-11111", Descripcion = "Comercial" },
            new { Proveedor = "NIT-DSC-002", Nombre = "Carlos Rojas", Correo = "c.rojas@dsc.bo", Telefono = "+591 720-22222", Descripcion = "Compras" }
        };

        foreach (var contacto in contactos)
        {
            if (!await dbContext.Contactos.AnyAsync(c => c.ProveedorCifId == contacto.Proveedor && c.Nombre == contacto.Nombre))
            {
                dbContext.Contactos.Add(new Contacto
                {
                    ProveedorCifId = contacto.Proveedor,
                    Nombre = contacto.Nombre,
                    Correo = contacto.Correo,
                    Telefono = contacto.Telefono,
                    Descripcion = contacto.Descripcion,
                    Estado = true,
                    FechaCreacion = now,
                    FechaModificacion = now
                });
                await dbContext.SaveChangesAsync();
            }
        }

        var marcasLookup = await dbContext.Marcas.ToDictionaryAsync(m => m.Nombre, m => m.MarcaId);

        var proveedoresMarcas = new (string Proveedor, string Marca)[]
        {
            ("NIT-PA-001", "GessoLux"),
            ("NIT-PA-001", "Construmax"),
            ("NIT-DSC-002", "Construmax")
        };

        foreach (var (proveedor, marcaNombre) in proveedoresMarcas)
        {
            var marcaId = marcasLookup[marcaNombre];
            if (!await dbContext.ProveedoresMarcas.AnyAsync(pm => pm.ProveedorCifId == proveedor && pm.MarcaId == marcaId))
            {
                dbContext.ProveedoresMarcas.Add(new ProveedorMarca
                {
                    ProveedorCifId = proveedor,
                    MarcaId = marcaId,
                    Estado = true,
                    FechaCreacion = now,
                    FechaModificacion = now
                });
                await dbContext.SaveChangesAsync();
            }
        }

        var materialesLookup = await dbContext.Materiales.ToDictionaryAsync(m => m.Codigo, m => m.MaterialId);

        var precios = new[]
        {
            new { Material = "MAT-001", Proveedor = "NIT-PA-001", Marca = "GessoLux", Precio = 68.00m },
            new { Material = "MAT-003", Proveedor = "NIT-PA-001", Marca = "GessoLux", Precio = 88.00m },
            new { Material = "MAT-004", Proveedor = "NIT-PA-001", Marca = "Construmax", Precio = 28.00m },
            new { Material = "MAT-002", Proveedor = "NIT-DSC-002", Marca = "Construmax", Precio = 49.50m },
            new { Material = "MAT-003", Proveedor = "NIT-DSC-002", Marca = "Construmax", Precio = 90.00m }
        };

        foreach (var precio in precios)
        {
            var materialId = materialesLookup[precio.Material];
            var marcaId = marcasLookup[precio.Marca];
            if (!await dbContext.PreciosTarifas.AnyAsync(p => p.MaterialId == materialId && p.ProveedorCifId == precio.Proveedor && p.MarcaId == marcaId))
            {
                dbContext.PreciosTarifas.Add(new PrecioTarifa
                {
                    MaterialId = materialId,
                    ProveedorCifId = precio.Proveedor,
                    MarcaId = marcaId,
                    Precio = precio.Precio,
                    Estado = true,
                    FechaCreacion = now,
                    FechaModificacion = now
                });
                await dbContext.SaveChangesAsync();
            }
        }
    }

    private static async Task SeedBusinessInfoAsync(SalesystemDbContext dbContext)
    {
        if (await dbContext.BusinessInfo.AnyAsync(b => b.IsPrimary))
        {
            return;
        }

        var now = DateTime.UtcNow;
        dbContext.BusinessInfo.Add(new BusinessInfo
        {
            EmpresaNombre = "Gessoplaca S.R.L.",
            Nit = "123456789",
            ContactoNombre = "Juan Pérez",
            ContactoTelefono = "+591 700-00000",
            ContactoEmail = "info@gessoplaca.bo",
            Direccion = "Av. Cristo Redentor, Santa Cruz",
            Ciudad = "Santa Cruz de la Sierra",
            Pais = "Bolivia",
            Web = "https://gessoplaca.bo",
            IsPrimary = true,
            FechaCreacion = now,
            FechaModificacion = now
        });
        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedDatabaseObjectsAsync(SalesystemDbContext dbContext)
    {
        const string viewSql = @"
IF NOT EXISTS (SELECT * FROM sys.views WHERE name = 'vw_ActiveSessions' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    EXEC('CREATE VIEW dbo.vw_ActiveSessions AS SELECT s.* FROM dbo.UserSessions s WHERE s.RevokedAt IS NULL AND s.ExpiresAt > SYSUTCDATETIME()');
END";

        const string procedureSql = @"
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'usp_PurgeAuthArtifacts' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    EXEC('CREATE PROCEDURE dbo.usp_PurgeAuthArtifacts AS
    BEGIN
        SET NOCOUNT ON;
        DELETE FROM dbo.AccessTokenRevocations WHERE ExpiresAt <= SYSUTCDATETIME();
        DELETE FROM dbo.RefreshTokens WHERE ExpiresAt <= SYSUTCDATETIME() OR RevokedAt IS NOT NULL;
        DELETE FROM dbo.UserSessions WHERE ExpiresAt <= SYSUTCDATETIME() AND NOT EXISTS (SELECT 1 FROM dbo.RefreshTokens rt WHERE rt.SessionId = UserSessions.Id AND rt.RevokedAt IS NULL AND rt.ExpiresAt > SYSUTCDATETIME());
    END');
END";

        await dbContext.Database.ExecuteSqlRawAsync(viewSql);
        await dbContext.Database.ExecuteSqlRawAsync(procedureSql);
    }
}
