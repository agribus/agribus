using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agribus.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddAlertTableAndFixGreenhouseSensorFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "alert",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "text", nullable: false),
                    enabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    threshold_value = table.Column<double>(type: "double precision", nullable: false),
                    range_min_value = table.Column<double>(type: "double precision", nullable: false),
                    range_max_value = table.Column<double>(type: "double precision", nullable: false),
                    measure_type = table.Column<string>(type: "text", nullable: false),
                    rule_type = table.Column<string>(type: "text", nullable: false),
                    greenhouse_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    last_modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_alert", x => x.id);
                    table.CheckConstraint("CK_MeasureType_IsValid", "measure_type IN ('temperature', 'humidity', 'pressure', 'motion', 'rssi', 'neighbors', 'unknown')");
                    table.CheckConstraint("CK_RuleType_IsValid", "rule_type IN ('above', 'below', 'outside', 'inside')");
                    table.ForeignKey(
                        name: "fk_alert_greenhouse_greenhouse_id",
                        column: x => x.greenhouse_id,
                        principalTable: "greenhouse",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_alert_greenhouse_id",
                table: "alert",
                column: "greenhouse_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "alert");
        }
    }
}
