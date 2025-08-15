using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agribus.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdColumnToGreenhouse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "country",
                table: "greenhouse",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text"
            );

            migrationBuilder.AlterColumn<string>(
                name: "city",
                table: "greenhouse",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text"
            );

            migrationBuilder.AddColumn<string>(
                name: "user_id",
                table: "greenhouse",
                type: "text",
                nullable: false,
                defaultValue: "",
                comment: "user table is currently stored in an external database (Clerk)"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "user_id", table: "greenhouse");

            migrationBuilder.AlterColumn<string>(
                name: "country",
                table: "greenhouse",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)"
            );

            migrationBuilder.AlterColumn<string>(
                name: "city",
                table: "greenhouse",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)"
            );
        }
    }
}
