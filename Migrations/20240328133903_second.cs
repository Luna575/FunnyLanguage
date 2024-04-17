using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FunnyLanguage_WPF.Migrations
{
    /// <inheritdoc />
    public partial class second : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_WordLists_VideoId",
                table: "WordLists",
                column: "VideoId");

            migrationBuilder.AddForeignKey(
                name: "FK_WordLists_Videos_VideoId",
                table: "WordLists",
                column: "VideoId",
                principalTable: "Videos",
                principalColumn: "VideoId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WordLists_Videos_VideoId",
                table: "WordLists");

            migrationBuilder.DropIndex(
                name: "IX_WordLists_VideoId",
                table: "WordLists");
        }
    }
}
