using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace server.Migrations
{
    /// <inheritdoc />
    public partial class MediaIdsProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ListId",
                table: "Polls");

            migrationBuilder.AddColumn<string>(
                name: "MediaIds",
                table: "Polls",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MediaIds",
                table: "Polls");

            migrationBuilder.AddColumn<string>(
                name: "ListId",
                table: "Polls",
                type: "TEXT",
                nullable: true);
        }
    }
}
