using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DDStudy2022.Api.Migrations
{
    /// <inheritdoc />
    public partial class addedIpToSessionModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IPAddress",
                table: "UserSessions",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IPAddress",
                table: "UserSessions");
        }
    }
}
