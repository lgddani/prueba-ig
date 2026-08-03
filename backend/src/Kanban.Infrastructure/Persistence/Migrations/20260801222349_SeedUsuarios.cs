using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kanban.Infrastructure.Persistence.Migrations
{
    /// <summary>
    /// Las contraseñas están
    /// almacenadas como PBKDF2-HMACSHA256 (100k iteraciones) con salt fijo embebido en el
    /// hash y pepper de servidor tomado de la variable de entorno PASSWORD_PEPPER
    /// </summary>
    public partial class SeedUsuarios : Migration
    {
        private static readonly Guid AnaId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        private static readonly Guid LuisId = Guid.Parse("22222222-2222-2222-2222-222222222222");

        private const string AnaHash = "100000.obLD1OX2BxgpOktcbX6PkA==.LVAp0GvhG2Md7/SpS34LlDh7YqEZd/ec7YS1wSiEb2c=";
        private const string LuisHash = "100000.ECAwQFBgcICQoLDA0ODxAg==.m3OPja3rjQQPzkettIFkObyfgvjzKMBg9ozkyCBy/ws=";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "usuarios",
                columns: new[] { "Id", "Nombre", "Correo", "PasswordHash", "FechaCreacion" },
                values: new object[,]
                {
                    { AnaId, "Ana Torres", "ana.torres@kanban.dev", AnaHash, new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc) },
                    { LuisId, "Luis Peña", "luis.pena@kanban.dev", LuisHash, new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(table: "usuarios", keyColumn: "Id", keyValue: AnaId);
            migrationBuilder.DeleteData(table: "usuarios", keyColumn: "Id", keyValue: LuisId);
        }
    }
}
