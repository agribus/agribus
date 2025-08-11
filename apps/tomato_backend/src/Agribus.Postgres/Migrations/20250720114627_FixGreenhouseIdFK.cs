using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agribus.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class FixGreenhouseIdFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_sensor_greenhouse_geenhouse_id",
                table: "sensor"
            );

            migrationBuilder.RenameColumn(
                name: "geenhouse_id",
                table: "sensor",
                newName: "greenhouse_id"
            );

            migrationBuilder.RenameIndex(
                name: "ix_sensor_geenhouse_id",
                table: "sensor",
                newName: "ix_sensor_greenhouse_id"
            );

            migrationBuilder.AddForeignKey(
                name: "fk_sensor_greenhouse_greenhouse_id",
                table: "sensor",
                column: "greenhouse_id",
                principalTable: "greenhouse",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_sensor_greenhouse_greenhouse_id",
                table: "sensor"
            );

            migrationBuilder.RenameColumn(
                name: "greenhouse_id",
                table: "sensor",
                newName: "geenhouse_id"
            );

            migrationBuilder.RenameIndex(
                name: "ix_sensor_greenhouse_id",
                table: "sensor",
                newName: "ix_sensor_geenhouse_id"
            );

            migrationBuilder.AddForeignKey(
                name: "fk_sensor_greenhouse_geenhouse_id",
                table: "sensor",
                column: "geenhouse_id",
                principalTable: "greenhouse",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );
        }
    }
}
