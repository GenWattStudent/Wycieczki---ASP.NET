using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book.App.Migrations
{
    public partial class imagesWaypoint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Waypoints");

            migrationBuilder.AlterColumn<float>(
                name: "Longitude",
                table: "Waypoints",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<float>(
                name: "Latitude",
                table: "Waypoints",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<int>(
                name: "WaypointModelId",
                table: "Images",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Images_WaypointModelId",
                table: "Images",
                column: "WaypointModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Waypoints_WaypointModelId",
                table: "Images",
                column: "WaypointModelId",
                principalTable: "Waypoints",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Waypoints_WaypointModelId",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_WaypointModelId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "WaypointModelId",
                table: "Images");

            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                table: "Waypoints",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "Waypoints",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Waypoints",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
