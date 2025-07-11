using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agribus.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddGreenhouseTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "greenhouse",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "varchar(100)", nullable: false),
                    country = table.Column<string>(type: "text", nullable: false),
                    city = table.Column<string>(type: "text", nullable: false),
                    crops = table.Column<string>(type: "jsonb", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    last_modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_greenhouse", x => x.id);
                },
                comment: "Greenhouses table stores information about greenhouse facilities.");

            migrationBuilder.CreateTable(
                name: "sensor",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "text", nullable: false),
                    source_address = table.Column<string>(type: "varchar(50)", nullable: false),
                    sensor_model = table.Column<string>(type: "varchar(50)", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    last_modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sensor", x => x.id);
                    table.CheckConstraint("CK_Sensor_Model_IsValid", "sensor_model IN ('RuuviTag', 'RuuviTagPro', 'Unknown')");
                    table.ForeignKey(
                        name: "fk_sensor_greenhouse_id",
                        column: x => x.id,
                        principalTable: "greenhouse",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Sensors table stores information about sensors used in the system.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sensor");

            migrationBuilder.DropTable(
                name: "greenhouse");
        }
    }
}
