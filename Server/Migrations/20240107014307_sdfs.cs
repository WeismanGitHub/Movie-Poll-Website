using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace server.Migrations
{
    /// <inheritdoc />
    public partial class sdfs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ChoiceId",
                table: "Vote",
                newName: "ItemId");

            migrationBuilder.RenameColumn(
                name: "MediaIds",
                table: "Polls",
                newName: "ItemIds");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "Vote",
                newName: "ChoiceId");

            migrationBuilder.RenameColumn(
                name: "ItemIds",
                table: "Polls",
                newName: "MediaIds");
        }
    }
}
