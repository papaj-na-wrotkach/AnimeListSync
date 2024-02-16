using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AnimeListSync.DB.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InternalSeriesSet",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InternalSeriesSet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProviderSet",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderSet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProviderSeriesSet",
                columns: table => new
                {
                    ProviderId = table.Column<byte>(type: "INTEGER", nullable: false),
                    IdFromProvider = table.Column<string>(type: "TEXT", nullable: false),
                    Id = table.Column<long>(type: "INTEGER", nullable: false),
                    InternalSeriesId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderSeriesSet", x => new { x.ProviderId, x.IdFromProvider });
                    table.ForeignKey(
                        name: "FK_ProviderSeriesSet_InternalSeriesSet_InternalSeriesId",
                        column: x => x.InternalSeriesId,
                        principalTable: "InternalSeriesSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProviderSeriesSet_ProviderSet_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "ProviderSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ProviderSet",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { (byte)0, "MyAnimeList (https://myanimelist.net)", "MyAnimeList" },
                    { (byte)1, "Shinden (https://shinden.pl)", "Shinden" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProviderSeriesSet_InternalSeriesId",
                table: "ProviderSeriesSet",
                column: "InternalSeriesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProviderSeriesSet");

            migrationBuilder.DropTable(
                name: "InternalSeriesSet");

            migrationBuilder.DropTable(
                name: "ProviderSet");
        }
    }
}
