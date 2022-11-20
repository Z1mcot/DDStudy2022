using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DDStudy2022.Api.Migrations
{
    /// <inheritdoc />
    public partial class fixSubscriptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsConfirmed",
                table: "Subscriptions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsModified",
                table: "PostComments",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsConfirmed",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "IsModified",
                table: "PostComments");
        }
    }
}
