using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace server.Migrations
{
    /// <inheritdoc />
    public partial class DontStoreMovieData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vote_Polls_PollId",
                table: "Vote");

            migrationBuilder.DropTable(
                name: "Movie");

            migrationBuilder.AlterColumn<Guid>(
                name: "PollId",
                table: "Vote",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MovieIds",
                table: "Polls",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Vote_Polls_PollId",
                table: "Vote",
                column: "PollId",
                principalTable: "Polls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vote_Polls_PollId",
                table: "Vote");

            migrationBuilder.DropColumn(
                name: "MovieIds",
                table: "Polls");

            migrationBuilder.AlterColumn<Guid>(
                name: "PollId",
                table: "Vote",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.CreateTable(
                name: "Movie",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    PollId = table.Column<Guid>(type: "TEXT", nullable: true),
                    PosterPath = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
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

            migrationBuilder.AddForeignKey(
                name: "FK_Vote_Polls_PollId",
                table: "Vote",
                column: "PollId",
                principalTable: "Polls",
                principalColumn: "Id");
        }
    }
}
