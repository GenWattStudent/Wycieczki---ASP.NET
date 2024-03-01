using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book.App.Migrations
{
    public partial class UserModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserModelId",
                table: "Tours",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tours_Users_UserModelId",
                table: "Tours");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Tours_UserModelId",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "UserModelId",
                table: "Tours");
        }
    }
}
