using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApis.Migrations
{
    public partial class UserAuth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "RoleId",
                table: "USER",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "ROLE",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ROLE", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_USER_RoleId",
                table: "USER",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_USER_ROLE_RoleId",
                table: "USER",
                column: "RoleId",
                principalTable: "ROLE",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_USER_ROLE_RoleId",
                table: "USER");

            migrationBuilder.DropTable(
                name: "ROLE");

            migrationBuilder.DropIndex(
                name: "IX_USER_RoleId",
                table: "USER");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "USER");
        }
    }
}
