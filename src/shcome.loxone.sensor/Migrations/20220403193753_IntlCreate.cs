using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace shcome.loxone.sensor.Migrations
{
    public partial class IntlCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "sensor",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying", nullable: true),
                    description = table.Column<string>(type: "character varying", nullable: true),
                    type = table.Column<string>(type: "character varying", nullable: true),
                    category = table.Column<string>(type: "character varying", nullable: true),
                    room = table.Column<string>(type: "character varying", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sensor", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sensor_value",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sensorId = table.Column<int>(type: "integer", nullable: false),
                    descriminator = table.Column<string>(type: "character varying", nullable: false),
                    timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    reading = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sensor_value", x => x.id);
                    table.ForeignKey(
                        name: "FK_sensor_value_sensor_sensorId",
                        column: x => x.sensorId,
                        principalSchema: "public",
                        principalTable: "sensor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sensor_value_sensorId",
                schema: "public",
                table: "sensor_value",
                column: "sensorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sensor_value",
                schema: "public");

            migrationBuilder.DropTable(
                name: "sensor",
                schema: "public");
        }
    }
}
