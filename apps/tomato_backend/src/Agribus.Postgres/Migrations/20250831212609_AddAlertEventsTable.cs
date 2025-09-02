using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agribus.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddAlertEventsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "alert_events",
                columns: table => new
                {
                    id = table.Column<Guid>(
                        type: "uuid",
                        nullable: false,
                        defaultValueSql: "gen_random_uuid()"
                    ),
                    measure_value = table.Column<double>(type: "double precision", nullable: false),
                    occured_at = table.Column<DateTimeOffset>(
                        type: "timestamptz",
                        nullable: false,
                        defaultValueSql: "NOW()"
                    ),
                    alert_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sensor_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false,
                        defaultValueSql: "NOW()"
                    ),
                    last_modified = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false,
                        defaultValueSql: "NOW()"
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_alert_events", x => x.id);
                    table.ForeignKey(
                        name: "fk_alert_events_alert_alert_id",
                        column: x => x.alert_id,
                        principalTable: "alert",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "fk_alert_events_sensor_sensor_id",
                        column: x => x.sensor_id,
                        principalTable: "sensor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "ix_alert_events_alert_id",
                table: "alert_events",
                column: "alert_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_alert_events_sensor_id",
                table: "alert_events",
                column: "sensor_id"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "alert_events");
        }
    }
}
