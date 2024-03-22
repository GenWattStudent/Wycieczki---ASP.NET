using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book.App.Migrations
{
    public partial class ReservationModelAdd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReservationModel_Tours_TourId",
                table: "ReservationModel");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservationModel_Users_UserId",
                table: "ReservationModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReservationModel",
                table: "ReservationModel");

            migrationBuilder.RenameTable(
                name: "ReservationModel",
                newName: "Reservations");

            migrationBuilder.RenameIndex(
                name: "IX_ReservationModel_UserId",
                table: "Reservations",
                newName: "IX_Reservations_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ReservationModel_TourId",
                table: "Reservations",
                newName: "IX_Reservations_TourId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reservations",
                table: "Reservations",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Tours_TourId",
                table: "Reservations",
                column: "TourId",
                principalTable: "Tours",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Users_UserId",
                table: "Reservations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Tours_TourId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Users_UserId",
                table: "Reservations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reservations",
                table: "Reservations");

            migrationBuilder.RenameTable(
                name: "Reservations",
                newName: "ReservationModel");

            migrationBuilder.RenameIndex(
                name: "IX_Reservations_UserId",
                table: "ReservationModel",
                newName: "IX_ReservationModel_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Reservations_TourId",
                table: "ReservationModel",
                newName: "IX_ReservationModel_TourId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReservationModel",
                table: "ReservationModel",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationModel_Tours_TourId",
                table: "ReservationModel",
                column: "TourId",
                principalTable: "Tours",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationModel_Users_UserId",
                table: "ReservationModel",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
