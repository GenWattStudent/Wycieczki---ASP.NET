using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book.App.Migrations
{
    public partial class TravelAgency2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_Users_UserId",
                table: "Contacts");

            migrationBuilder.DropIndex(
                name: "IX_Contacts_UserId",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Contacts");

            migrationBuilder.AddColumn<int>(
                name: "ContactId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TravelAgencyId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TravelAgencyId",
                table: "Tours",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TravelAgencyId",
                table: "Reservations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TravelAgencyModelId",
                table: "Images",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TravelAgencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VideoPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogoPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TravelAgencies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TravelAgencies_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TourId = table.Column<int>(type: "int", nullable: true),
                    TravelAgencyId = table.Column<int>(type: "int", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Tours_TourId",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comments_TravelAgencies_TravelAgencyId",
                        column: x => x.TravelAgencyId,
                        principalTable: "TravelAgencies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_ContactId",
                table: "Users",
                column: "ContactId",
                unique: true,
                filter: "[ContactId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_TravelAgencyId",
                table: "Users",
                column: "TravelAgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Tours_TravelAgencyId",
                table: "Tours",
                column: "TravelAgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_TravelAgencyId",
                table: "Reservations",
                column: "TravelAgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_TravelAgencyModelId",
                table: "Images",
                column: "TravelAgencyModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_TourId",
                table: "Comments",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_TravelAgencyId",
                table: "Comments",
                column: "TravelAgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UserId",
                table: "Comments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TravelAgencies_AddressId",
                table: "TravelAgencies",
                column: "AddressId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_TravelAgencies_TravelAgencyModelId",
                table: "Images",
                column: "TravelAgencyModelId",
                principalTable: "TravelAgencies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_TravelAgencies_TravelAgencyId",
                table: "Reservations",
                column: "TravelAgencyId",
                principalTable: "TravelAgencies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tours_TravelAgencies_TravelAgencyId",
                table: "Tours",
                column: "TravelAgencyId",
                principalTable: "TravelAgencies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Contacts_ContactId",
                table: "Users",
                column: "ContactId",
                principalTable: "Contacts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_TravelAgencies_TravelAgencyId",
                table: "Users",
                column: "TravelAgencyId",
                principalTable: "TravelAgencies",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_TravelAgencies_TravelAgencyModelId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_TravelAgencies_TravelAgencyId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Tours_TravelAgencies_TravelAgencyId",
                table: "Tours");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Contacts_ContactId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_TravelAgencies_TravelAgencyId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "TravelAgencies");

            migrationBuilder.DropIndex(
                name: "IX_Users_ContactId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_TravelAgencyId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Tours_TravelAgencyId",
                table: "Tours");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_TravelAgencyId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Images_TravelAgencyModelId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "ContactId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TravelAgencyId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TravelAgencyId",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "TravelAgencyId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "TravelAgencyModelId",
                table: "Images");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Contacts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_UserId",
                table: "Contacts",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_Users_UserId",
                table: "Contacts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
