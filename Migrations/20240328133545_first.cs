using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FunnyLanguage_WPF.Migrations
{
    /// <inheritdoc />
    public partial class first : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Videos_WordLists_WordListId",
                table: "Videos");

            migrationBuilder.DropIndex(
                name: "IX_Videos_WordListId",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "WordListId",
                table: "Videos");

            migrationBuilder.AddColumn<int>(
                name: "SuccessRate",
                table: "Words",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FolderPath",
                table: "WordLists",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "VideoId",
                table: "WordLists",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CaptionFolderPath",
                table: "Videos",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FolderPath",
                table: "Videos",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Videos",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VideoCode",
                table: "Videos",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    LanguageId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    VideoId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.LanguageId);
                    table.ForeignKey(
                        name: "FK_Languages_Videos_VideoId",
                        column: x => x.VideoId,
                        principalTable: "Videos",
                        principalColumn: "VideoId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Languages_VideoId",
                table: "Languages",
                column: "VideoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropColumn(
                name: "SuccessRate",
                table: "Words");

            migrationBuilder.DropColumn(
                name: "FolderPath",
                table: "WordLists");

            migrationBuilder.DropColumn(
                name: "VideoId",
                table: "WordLists");

            migrationBuilder.DropColumn(
                name: "CaptionFolderPath",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "FolderPath",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "VideoCode",
                table: "Videos");

            migrationBuilder.AddColumn<int>(
                name: "WordListId",
                table: "Videos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Videos_WordListId",
                table: "Videos",
                column: "WordListId");

            migrationBuilder.AddForeignKey(
                name: "FK_Videos_WordLists_WordListId",
                table: "Videos",
                column: "WordListId",
                principalTable: "WordLists",
                principalColumn: "WordListId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
