using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicVillaAPI.Migrations
{
    /// <inheritdoc />
    public partial class AgregarNumeroVillaTabla : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NumeroVillas",
                columns: table => new
                {
                    VillaNo = table.Column<int>(type: "int", nullable: false),
                    VillaId = table.Column<int>(type: "int", nullable: false),
                    DetalleEspecial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NumeroVillas", x => x.VillaNo);
                    table.ForeignKey(
                        name: "FK_NumeroVillas_Villas_VillaId",
                        column: x => x.VillaId,
                        principalTable: "Villas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FechaActualizacion", "FechaCracion" },
                values: new object[] { new DateTime(2024, 7, 5, 19, 10, 27, 781, DateTimeKind.Local).AddTicks(3712), new DateTime(2024, 7, 5, 19, 10, 27, 781, DateTimeKind.Local).AddTicks(3696) });

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaActualizacion", "FechaCracion" },
                values: new object[] { new DateTime(2024, 7, 5, 19, 10, 27, 781, DateTimeKind.Local).AddTicks(3717), new DateTime(2024, 7, 5, 19, 10, 27, 781, DateTimeKind.Local).AddTicks(3717) });

            migrationBuilder.CreateIndex(
                name: "IX_NumeroVillas_VillaId",
                table: "NumeroVillas",
                column: "VillaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NumeroVillas");

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FechaActualizacion", "FechaCracion" },
                values: new object[] { new DateTime(2024, 6, 25, 22, 49, 50, 250, DateTimeKind.Local).AddTicks(1715), new DateTime(2024, 6, 25, 22, 49, 50, 250, DateTimeKind.Local).AddTicks(1701) });

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaActualizacion", "FechaCracion" },
                values: new object[] { new DateTime(2024, 6, 25, 22, 49, 50, 250, DateTimeKind.Local).AddTicks(1718), new DateTime(2024, 6, 25, 22, 49, 50, 250, DateTimeKind.Local).AddTicks(1718) });
        }
    }
}
