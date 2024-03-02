using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book.App.Migrations
{
    public partial class Waypoint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tours_Users_UserModelId",
                table: "Tours");

            migrationBuilder.DropIndex(
                name: "IX_Tours_UserModelId",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "UserModelId",
                table: "Tours");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Tours",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "MaxUsers",
                table: "Tours",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Tours",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "TourModelUserModel",
                columns: table => new
                {
                    ToursId = table.Column<int>(type: "int", nullable: false),
                    UsersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourModelUserModel", x => new { x.ToursId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_TourModelUserModel_Tours_ToursId",
                        column: x => x.ToursId,
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TourModelUserModel_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TourModelUserModel_UsersId",
                table: "TourModelUserModel",
                column: "UsersId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TourModelUserModel");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "MaxUsers",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Tours");

            migrationBuilder.AddColumn<int>(
                name: "UserModelId",
                table: "Tours",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tours_UserModelId",
                table: "Tours",
                column: "UserModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tours_Users_UserModelId",
                table: "Tours",
                column: "UserModelId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
