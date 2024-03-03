using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book.App.Migrations
{
    public partial class Image2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImageModel_Tours_TourModelId",
                table: "ImageModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ImageModel",
                table: "ImageModel");

            migrationBuilder.RenameTable(
                name: "ImageModel",
                newName: "Images");

            migrationBuilder.RenameIndex(
                name: "IX_ImageModel_TourModelId",
                table: "Images",
                newName: "IX_Images_TourModelId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Images",
                table: "Images",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Tours_TourModelId",
                table: "Images",
                column: "TourModelId",
                principalTable: "Tours",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Tours_TourModelId",
                table: "Images");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Images",
                table: "Images");

            migrationBuilder.RenameTable(
                name: "Images",
                newName: "ImageModel");

            migrationBuilder.RenameIndex(
                name: "IX_Images_TourModelId",
                table: "ImageModel",
                newName: "IX_ImageModel_TourModelId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ImageModel",
                table: "ImageModel",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ImageModel_Tours_TourModelId",
                table: "ImageModel",
                column: "TourModelId",
                principalTable: "Tours",
                principalColumn: "Id");
        }
    }
}
