using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AnimeListSync.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DatabaseSeries",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatabaseSeries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Providers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Providers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProviderSeries",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Provider = table.Column<byte>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderSeries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Associations",
                columns: table => new
                {
                    DatabaseId = table.Column<long>(type: "INTEGER", nullable: false),
                    ProviderId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Associations", x => new { x.DatabaseId, x.ProviderId });
                    table.ForeignKey(
                        name: "FK_Associations_DatabaseSeries_DatabaseId",
                        column: x => x.DatabaseId,
                        principalTable: "DatabaseSeries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Associations_ProviderSeries_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "ProviderSeries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Providers",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 0, "MyAnimeList (https://myanimelist.net)", "MyAnimeList" },
                    { 1, "Shinden (https://shinden.pl)", "Shinden" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Associations_ProviderId",
                table: "Associations",
                column: "ProviderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Associations");

            migrationBuilder.DropTable(
                name: "Providers");

            migrationBuilder.DropTable(
                name: "DatabaseSeries");

            migrationBuilder.DropTable(
                name: "ProviderSeries");
        }
    }
}
