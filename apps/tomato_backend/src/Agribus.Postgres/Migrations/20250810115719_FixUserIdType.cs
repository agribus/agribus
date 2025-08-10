using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agribus.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class FixUserIdType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "user_id",
                table: "greenhouse",
                type: "varchar(32)",
                nullable: false,
                comment: "user table is currently stored in an external database (Clerk)",
                oldClrType: typeof(string),
                oldType: "text",
                oldComment: "user table is currently stored in an external database (Clerk)"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "user_id",
                table: "greenhouse",
                type: "text",
                nullable: false,
                comment: "user table is currently stored in an external database (Clerk)",
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldComment: "user table is currently stored in an external database (Clerk)"
            );
        }
    }
}
