using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DDStudy2022.Api.Migrations
{
    /// <inheritdoc />
    public partial class uniqueNameTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Users_NameTag",
                table: "Users",
                column: "NameTag",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_NameTag",
                table: "Users");
        }
    }
}
