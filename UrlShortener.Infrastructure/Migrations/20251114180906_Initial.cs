using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlShortener.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "USER",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FULL_NAME = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BIRTH_DATE = table.Column<DateOnly>(type: "date", nullable: true),
                    PHONE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EMAIL = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PASSWORD_HASH = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IS_EMAIL_CONFIRMED = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SHORTENED_URL",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CODE = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    ORIGINAL_URL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CREATED_AT_UTC = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    USER_ID = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SHORTENED_URL", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SHORTENED_URL_USER_USER_ID",
                        column: x => x.USER_ID,
                        principalTable: "USER",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VISITED_URL",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CODE = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    VISITED_AT_UTC = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    USER_AGENT = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    REFERER = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IP_ADDRESS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SHORTENED_URL_ID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VISITED_URL", x => x.ID);
                    table.ForeignKey(
                        name: "FK_VISITED_URL_SHORTENED_URL_SHORTENED_URL_ID",
                        column: x => x.SHORTENED_URL_ID,
                        principalTable: "SHORTENED_URL",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SHORTENED_URL_CODE",
                table: "SHORTENED_URL",
                column: "CODE",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SHORTENED_URL_USER_ID",
                table: "SHORTENED_URL",
                column: "USER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_USER_EMAIL_UNIQUE",
                table: "USER",
                column: "EMAIL",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VISITED_URL_CODE",
                table: "VISITED_URL",
                column: "CODE");

            migrationBuilder.CreateIndex(
                name: "IX_VISITED_URL_SHORTENED_URL_ID",
                table: "VISITED_URL",
                column: "SHORTENED_URL_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VISITED_URL");

            migrationBuilder.DropTable(
                name: "SHORTENED_URL");

            migrationBuilder.DropTable(
                name: "USER");
        }
    }
}
