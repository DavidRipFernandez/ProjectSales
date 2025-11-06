using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projectSales.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "BusinessInfo",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    empresaNombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    nit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    contactoNombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    contactoTelefono = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    contactoEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    direccion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ciudad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    pais = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    web = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    facebook = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    instagram = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    tiktok = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    isPrimary = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    creadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    fechaCreacion = table.Column<DateTime>(type: "datetime2(3)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    modificadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    fechaModificacion = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    rowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessInfo", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "CategoriasMateriales",
                schema: "dbo",
                columns: table => new
                {
                    CategoriaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    CreadoPor = table.Column<int>(type: "int", nullable: true),
                    ModificadoPor = table.Column<int>(type: "int", nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriasMateriales", x => x.CategoriaId);
                });

            migrationBuilder.CreateTable(
                name: "Marcas",
                schema: "dbo",
                columns: table => new
                {
                    MarcaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    CreadoPor = table.Column<int>(type: "int", nullable: true),
                    ModificadoPor = table.Column<int>(type: "int", nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marcas", x => x.MarcaId);
                });

            migrationBuilder.CreateTable(
                name: "Modules",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    creadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    fechaCreacion = table.Column<DateTime>(type: "datetime2(3)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    modificadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    fechaModificacion = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    rowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modules", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Proveedores",
                schema: "dbo",
                columns: table => new
                {
                    ProveedorCifId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DomicilioSocial = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    CreadoPor = table.Column<int>(type: "int", nullable: true),
                    ModificadoPor = table.Column<int>(type: "int", nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proveedores", x => x.ProveedorCifId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    creadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    fechaCreacion = table.Column<DateTime>(type: "datetime2(3)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    modificadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    fechaModificacion = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    rowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    passwordHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ultimoLoginUtc = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    creadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    fechaCreacion = table.Column<DateTime>(type: "datetime2(3)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    modificadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    fechaModificacion = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    rowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Materiales",
                schema: "dbo",
                columns: table => new
                {
                    MaterialId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    UnidadMedida = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SKU = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CategoriaId = table.Column<int>(type: "int", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    CreadoPor = table.Column<int>(type: "int", nullable: true),
                    ModificadoPor = table.Column<int>(type: "int", nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materiales", x => x.MaterialId);
                    table.ForeignKey(
                        name: "FK_Materiales_Categoria",
                        column: x => x.CategoriaId,
                        principalSchema: "dbo",
                        principalTable: "CategoriasMateriales",
                        principalColumn: "CategoriaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Actions",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    moduleId = table.Column<int>(type: "int", nullable: false),
                    key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    sortOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    creadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    fechaCreacion = table.Column<DateTime>(type: "datetime2(3)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    modificadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    fechaModificacion = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    rowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actions", x => x.id);
                    table.ForeignKey(
                        name: "FK_Actions_Modules",
                        column: x => x.moduleId,
                        principalSchema: "dbo",
                        principalTable: "Modules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Contactos",
                schema: "dbo",
                columns: table => new
                {
                    ContactoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Telefono = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProveedorCifId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    CreadoPor = table.Column<int>(type: "int", nullable: true),
                    ModificadoPor = table.Column<int>(type: "int", nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contactos", x => x.ContactoId);
                    table.ForeignKey(
                        name: "FK_Contactos_Proveedor",
                        column: x => x.ProveedorCifId,
                        principalSchema: "dbo",
                        principalTable: "Proveedores",
                        principalColumn: "ProveedorCifId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProveedoresMarcas",
                schema: "dbo",
                columns: table => new
                {
                    ProveedorCifId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MarcaId = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    CreadoPor = table.Column<int>(type: "int", nullable: true),
                    ModificadoPor = table.Column<int>(type: "int", nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProveedoresMarcas", x => new { x.ProveedorCifId, x.MarcaId });
                    table.ForeignKey(
                        name: "FK_ProvMarcas_Marca",
                        column: x => x.MarcaId,
                        principalSchema: "dbo",
                        principalTable: "Marcas",
                        principalColumn: "MarcaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProvMarcas_Prov",
                        column: x => x.ProveedorCifId,
                        principalSchema: "dbo",
                        principalTable: "Proveedores",
                        principalColumn: "ProveedorCifId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                schema: "dbo",
                columns: table => new
                {
                    userId = table.Column<int>(type: "int", nullable: false),
                    roleId = table.Column<int>(type: "int", nullable: false),
                    creadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    fechaCreacion = table.Column<DateTime>(type: "datetime2(3)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    modificadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    fechaModificacion = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    rowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.userId, x.roleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles",
                        column: x => x.roleId,
                        principalSchema: "dbo",
                        principalTable: "Roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users",
                        column: x => x.userId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserSessions",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<int>(type: "int", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2(3)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    lastSeenAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    ipAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    userAgent = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    deviceName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    location = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    expiresAt = table.Column<DateTime>(type: "datetime2(3)", nullable: false),
                    revokedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    revokeReason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessions", x => x.id);
                    table.ForeignKey(
                        name: "FK_UserSessions_Users",
                        column: x => x.userId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PreciosTarifas",
                schema: "dbo",
                columns: table => new
                {
                    MaterialId = table.Column<int>(type: "int", nullable: false),
                    ProveedorCifId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MarcaId = table.Column<int>(type: "int", nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(7)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    CreadoPor = table.Column<int>(type: "int", nullable: true),
                    ModificadoPor = table.Column<int>(type: "int", nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreciosTarifas", x => new { x.MaterialId, x.ProveedorCifId, x.MarcaId });
                    table.CheckConstraint("CK_PreciosTarifas_Precio", "[Precio] >= 0");
                    table.ForeignKey(
                        name: "FK_PreciosTarifas_Marca",
                        column: x => x.MarcaId,
                        principalSchema: "dbo",
                        principalTable: "Marcas",
                        principalColumn: "MarcaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PreciosTarifas_Material",
                        column: x => x.MaterialId,
                        principalSchema: "dbo",
                        principalTable: "Materiales",
                        principalColumn: "MaterialId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PreciosTarifas_Proveedor",
                        column: x => x.ProveedorCifId,
                        principalSchema: "dbo",
                        principalTable: "Proveedores",
                        principalColumn: "ProveedorCifId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    roleId = table.Column<int>(type: "int", nullable: false),
                    moduleId = table.Column<int>(type: "int", nullable: false),
                    actionId = table.Column<int>(type: "int", nullable: false),
                    allowed = table.Column<bool>(type: "bit", nullable: false),
                    creadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    fechaCreacion = table.Column<DateTime>(type: "datetime2(3)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    modificadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    fechaModificacion = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    rowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => x.id);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Actions",
                        column: x => x.actionId,
                        principalSchema: "dbo",
                        principalTable: "Actions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Modules",
                        column: x => x.moduleId,
                        principalSchema: "dbo",
                        principalTable: "Modules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles",
                        column: x => x.roleId,
                        principalSchema: "dbo",
                        principalTable: "Roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccessTokenRevocations",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    jti = table.Column<string>(type: "char(36)", fixedLength: true, maxLength: 36, nullable: false),
                    sessionId = table.Column<int>(type: "int", nullable: true),
                    revokedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    expiresAt = table.Column<DateTime>(type: "datetime2(3)", nullable: false),
                    reason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessTokenRevocations", x => x.id);
                    table.ForeignKey(
                        name: "FK_AccessRevocations_Session",
                        column: x => x.sessionId,
                        principalSchema: "dbo",
                        principalTable: "UserSessions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                schema: "dbo",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<int>(type: "int", nullable: false),
                    sessionId = table.Column<int>(type: "int", nullable: false),
                    tokenHash = table.Column<byte[]>(type: "varbinary(32)", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2(3)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    expiresAt = table.Column<DateTime>(type: "datetime2(3)", nullable: false),
                    revokedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    rotatedFromTokenId = table.Column<int>(type: "int", nullable: true),
                    replacedByTokenId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_ReplacedBy",
                        column: x => x.replacedByTokenId,
                        principalSchema: "dbo",
                        principalTable: "RefreshTokens",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_RotatedFrom",
                        column: x => x.rotatedFromTokenId,
                        principalSchema: "dbo",
                        principalTable: "RefreshTokens",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Session",
                        column: x => x.sessionId,
                        principalSchema: "dbo",
                        principalTable: "UserSessions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_User",
                        column: x => x.userId,
                        principalSchema: "dbo",
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessTokenRevocations_sessionId",
                schema: "dbo",
                table: "AccessTokenRevocations",
                column: "sessionId");

            migrationBuilder.CreateIndex(
                name: "UX_AccessRevocations_Jti",
                schema: "dbo",
                table: "AccessTokenRevocations",
                column: "jti",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Actions_Module_Key",
                schema: "dbo",
                table: "Actions",
                columns: new[] { "moduleId", "key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Actions_Module_Sort",
                schema: "dbo",
                table: "Actions",
                columns: new[] { "moduleId", "sortOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_BusinessInfo_Primary",
                schema: "dbo",
                table: "BusinessInfo",
                column: "isPrimary",
                unique: true,
                filter: "[isPrimary] = 1");

            migrationBuilder.CreateIndex(
                name: "UX_CategoriasMateriales_Nombre",
                schema: "dbo",
                table: "CategoriasMateriales",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contactos_Proveedor",
                schema: "dbo",
                table: "Contactos",
                column: "ProveedorCifId");

            migrationBuilder.CreateIndex(
                name: "UX_Marcas_Nombre",
                schema: "dbo",
                table: "Marcas",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Materiales_CategoriaId",
                schema: "dbo",
                table: "Materiales",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "UX_Materiales_Codigo",
                schema: "dbo",
                table: "Materiales",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Materiales_SKU",
                schema: "dbo",
                table: "Materiales",
                column: "SKU",
                unique: true,
                filter: "[SKU] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UX_Modules_key",
                schema: "dbo",
                table: "Modules",
                column: "key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PreciosTarifas_Marca",
                schema: "dbo",
                table: "PreciosTarifas",
                column: "MarcaId");

            migrationBuilder.CreateIndex(
                name: "IX_PreciosTarifas_Prov",
                schema: "dbo",
                table: "PreciosTarifas",
                column: "ProveedorCifId");

            migrationBuilder.CreateIndex(
                name: "UX_Proveedores_Nombre",
                schema: "dbo",
                table: "Proveedores",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProveedoresMarcas_MarcaId",
                schema: "dbo",
                table: "ProveedoresMarcas",
                column: "MarcaId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_replacedByTokenId",
                schema: "dbo",
                table: "RefreshTokens",
                column: "replacedByTokenId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_rotatedFromTokenId",
                schema: "dbo",
                table: "RefreshTokens",
                column: "rotatedFromTokenId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Session",
                schema: "dbo",
                table: "RefreshTokens",
                column: "sessionId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_User",
                schema: "dbo",
                table: "RefreshTokens",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "UX_RefreshTokens_Hash",
                schema: "dbo",
                table: "RefreshTokens",
                column: "tokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_actionId",
                schema: "dbo",
                table: "RolePermissions",
                column: "actionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_moduleId",
                schema: "dbo",
                table: "RolePermissions",
                column: "moduleId");

            migrationBuilder.CreateIndex(
                name: "UX_RolePerms_UQ",
                schema: "dbo",
                table: "RolePermissions",
                columns: new[] { "roleId", "moduleId", "actionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Roles_nombre",
                schema: "dbo",
                table: "Roles",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_roleId",
                schema: "dbo",
                table: "UserRoles",
                column: "roleId");

            migrationBuilder.CreateIndex(
                name: "UX_Users_email",
                schema: "dbo",
                table: "Users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Users_username",
                schema: "dbo",
                table: "Users",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_Active",
                schema: "dbo",
                table: "UserSessions",
                column: "userId",
                filter: "[revokedAt] IS NULL")
                .Annotation("SqlServer:Include", new[] { "createdAt", "lastSeenAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessTokenRevocations",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "BusinessInfo",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Contactos",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PreciosTarifas",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ProveedoresMarcas",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "RefreshTokens",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "RolePermissions",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UserRoles",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Materiales",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Marcas",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Proveedores",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UserSessions",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Actions",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "CategoriasMateriales",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Modules",
                schema: "dbo");
        }
    }
}
