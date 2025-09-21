using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agribus.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class FixAlertsConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(name: "CK_MeasureType_IsValid", table: "alert");

            migrationBuilder.DropCheckConstraint(name: "CK_RuleType_IsValid", table: "alert");

            migrationBuilder.AlterColumn<double>(
                name: "threshold_value",
                table: "alert",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision"
            );

            migrationBuilder.AlterColumn<double>(
                name: "range_min_value",
                table: "alert",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision"
            );

            migrationBuilder.AlterColumn<double>(
                name: "range_max_value",
                table: "alert",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision"
            );

            migrationBuilder.AddCheckConstraint(
                name: "CK_MeasureType_IsValid",
                table: "alert",
                sql: "LOWER(measure_type) IN ('temperature', 'humidity', 'pressure', 'motion', 'rssi', 'neighbors', 'unknown')"
            );

            migrationBuilder.AddCheckConstraint(
                name: "CK_RuleType_IsValid",
                table: "alert",
                sql: "LOWER(rule_type) IN ('above', 'below', 'outside', 'inside')"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(name: "CK_MeasureType_IsValid", table: "alert");

            migrationBuilder.DropCheckConstraint(name: "CK_RuleType_IsValid", table: "alert");

            migrationBuilder.AlterColumn<double>(
                name: "threshold_value",
                table: "alert",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<double>(
                name: "range_min_value",
                table: "alert",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<double>(
                name: "range_max_value",
                table: "alert",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true
            );

            migrationBuilder.AddCheckConstraint(
                name: "CK_MeasureType_IsValid",
                table: "alert",
                sql: "measure_type IN ('temperature', 'humidity', 'pressure', 'motion', 'rssi', 'neighbors', 'unknown')"
            );

            migrationBuilder.AddCheckConstraint(
                name: "CK_RuleType_IsValid",
                table: "alert",
                sql: "rule_type IN ('above', 'below', 'outside', 'inside')"
            );
        }
    }
}
