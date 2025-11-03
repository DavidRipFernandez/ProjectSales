using Microsoft.EntityFrameworkCore;
using projectSales.Server.Domain.Entities;

namespace projectSales.Server.Infrastructure.Persistence;

public class SalesystemDbContext : DbContext
{
    public SalesystemDbContext(DbContextOptions<SalesystemDbContext> options)
        : base(options)
    {
    }

    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Module> Modules => Set<Module>();
    public DbSet<ModuleAction> Actions => Set<ModuleAction>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<AccessTokenRevocation> AccessTokenRevocations => Set<AccessTokenRevocation>();
    public DbSet<Marca> Marcas => Set<Marca>();
    public DbSet<CategoriaMaterial> CategoriasMateriales => Set<CategoriaMaterial>();
    public DbSet<Material> Materiales => Set<Material>();
    public DbSet<Proveedor> Proveedores => Set<Proveedor>();
    public DbSet<Contacto> Contactos => Set<Contacto>();
    public DbSet<ProveedorMarca> ProveedoresMarcas => Set<ProveedorMarca>();
    public DbSet<PrecioTarifa> PreciosTarifas => Set<PrecioTarifa>();
    public DbSet<BusinessInfo> BusinessInfo => Set<BusinessInfo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("dbo");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SalesystemDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
