using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameLibrary.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddGameOwnership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Games",
                newName: "CreatedByUserId");

            migrationBuilder.CreateTable(
                name: "GameOwnerships",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    PricePaid = table.Column<decimal>(type: "TEXT", nullable: true),
                    AddedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameOwnerships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameOwnerships_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameOwnerships_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameOwnerships_GameId_UserId",
                table: "GameOwnerships",
                columns: new[] { "GameId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameOwnerships_UserId",
                table: "GameOwnerships",
                column: "UserId");

            migrationBuilder.Sql(
                @"INSERT INTO GameOwnerships (GameId, UserId, PricePaid, AddedAt)
                  SELECT Id, CreatedByUserId, Price, CURRENT_TIMESTAMP FROM Games;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameOwnerships");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "Games",
                newName: "OwnerId");
        }
    }
}
