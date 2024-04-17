using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FunnyLanguage_WPF.Migrations
{
    /// <inheritdoc />
    public partial class MigrationName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WordLists",
                columns: table => new
                {
                    WordListId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WordLists", x => x.WordListId);
                });

            migrationBuilder.CreateTable(
                name: "Videos",
                columns: table => new
                {
                    VideoId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Url = table.Column<string>(type: "TEXT", nullable: false),
                    WordListId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Videos", x => x.VideoId);
                    table.ForeignKey(
                        name: "FK_Videos_WordLists_WordListId",
                        column: x => x.WordListId,
                        principalTable: "WordLists",
                        principalColumn: "WordListId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Words",
                columns: table => new
                {
                    WordId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WordlistId = table.Column<int>(type: "INTEGER", nullable: false),
                    FirstLanguage = table.Column<string>(type: "TEXT", nullable: false),
                    SecondLanguage = table.Column<string>(type: "TEXT", nullable: false),
                    OriginalText = table.Column<string>(type: "TEXT", nullable: false),
                    TranslatedText = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Words", x => x.WordId);
                    table.ForeignKey(
                        name: "FK_Words_WordLists_WordlistId",
                        column: x => x.WordlistId,
                        principalTable: "WordLists",
                        principalColumn: "WordListId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Videos_WordListId",
                table: "Videos",
                column: "WordListId");

            migrationBuilder.CreateIndex(
                name: "IX_Words_WordlistId",
                table: "Words",
                column: "WordlistId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Videos");

            migrationBuilder.DropTable(
                name: "Words");

            migrationBuilder.DropTable(
                name: "WordLists");
        }
    }
}
