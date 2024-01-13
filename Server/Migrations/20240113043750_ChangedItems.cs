using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace server.Migrations
{
    /// <inheritdoc />
    public partial class ChangedItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemIds",
                table: "Polls");

            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "Vote",
                newName: "MovieId");

            migrationBuilder.CreateTable(
                name: "Movie",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    PosterPath = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    PollId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movie", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Movie_Polls_PollId",
                        column: x => x.PollId,
                        principalTable: "Polls",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Movie_PollId",
                table: "Movie",
                column: "PollId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Movie");

            migrationBuilder.RenameColumn(
                name: "MovieId",
                table: "Vote",
                newName: "ItemId");

            migrationBuilder.AddColumn<string>(
                name: "ItemIds",
                table: "Polls",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
