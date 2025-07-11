using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agribus.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddGreenHouseForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_sensor_greenhouse_id",
                table: "sensor");

            migrationBuilder.AddColumn<Guid>(
                name: "geenhouse_id",
                table: "sensor",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_sensor_geenhouse_id",
                table: "sensor",
                column: "geenhouse_id");

            migrationBuilder.AddForeignKey(
                name: "fk_sensor_greenhouse_geenhouse_id",
                table: "sensor",
                column: "geenhouse_id",
                principalTable: "greenhouse",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_sensor_greenhouse_geenhouse_id",
                table: "sensor");

            migrationBuilder.DropIndex(
                name: "ix_sensor_geenhouse_id",
                table: "sensor");

            migrationBuilder.DropColumn(
                name: "geenhouse_id",
                table: "sensor");

            migrationBuilder.AddForeignKey(
                name: "fk_sensor_greenhouse_id",
                table: "sensor",
                column: "id",
                principalTable: "greenhouse",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
