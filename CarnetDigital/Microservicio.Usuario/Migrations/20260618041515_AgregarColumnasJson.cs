using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microservicio.Usuario.Migrations
{
    /// <inheritdoc />
    public partial class AgregarColumnasJson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EstadoUsuario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadoUsuario", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Identificacion = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NombreCompleto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContrasenaEncriptada = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FotografiaBase64 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TokenConfirmacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaExpiracionToken = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EstadoId = table.Column<int>(type: "int", nullable: false),
                    TipoIdentificacionId = table.Column<int>(type: "int", nullable: false),
                    TipoUsuarioId = table.Column<int>(type: "int", nullable: false),
                    RolId = table.Column<int>(type: "int", nullable: false),
                    InstitucionesIds = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CarrerasIds = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AreasIds = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Identificacion);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EstadoUsuario");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
