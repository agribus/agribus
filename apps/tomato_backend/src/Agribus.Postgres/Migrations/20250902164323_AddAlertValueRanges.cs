using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agribus.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddAlertValueRanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
