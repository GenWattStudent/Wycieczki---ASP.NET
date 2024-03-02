using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book.App.Migrations
{
    public partial class AddWaypointToDbContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WaypointModel_Tours_TourId",
                table: "WaypointModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WaypointModel",
                table: "WaypointModel");

            migrationBuilder.RenameTable(
                name: "WaypointModel",
                newName: "Waypoints");

            migrationBuilder.RenameIndex(
                name: "IX_WaypointModel_TourId",
                table: "Waypoints",
                newName: "IX_Waypoints_TourId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Waypoints",
                table: "Waypoints",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Waypoints_Tours_TourId",
                table: "Waypoints",
                column: "TourId",
                principalTable: "Tours",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Waypoints_Tours_TourId",
                table: "Waypoints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Waypoints",
                table: "Waypoints");

            migrationBuilder.RenameTable(
                name: "Waypoints",
                newName: "WaypointModel");

            migrationBuilder.RenameIndex(
                name: "IX_Waypoints_TourId",
                table: "WaypointModel",
                newName: "IX_WaypointModel_TourId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WaypointModel",
                table: "WaypointModel",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WaypointModel_Tours_TourId",
                table: "WaypointModel",
                column: "TourId",
                principalTable: "Tours",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
