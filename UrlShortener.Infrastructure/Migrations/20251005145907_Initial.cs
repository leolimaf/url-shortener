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
                name: "SHORTENED_URL",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LONG_URL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SHORT_URL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CODE = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    CREATED_AT_UTC = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SHORTENED_URL", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SHORTENED_URL_CODE",
                table: "SHORTENED_URL",
                column: "CODE",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SHORTENED_URL");
        }
    }
}
