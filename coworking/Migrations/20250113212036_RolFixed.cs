using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace coworking.Migrations
{
    /// <inheritdoc />
    public partial class RolFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRol");

            migrationBuilder.AddColumn<int>(
                name: "RolId",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_User_RolId",
                table: "User",
                column: "RolId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Rol_RolId",
                table: "User",
                column: "RolId",
                principalTable: "Rol",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Rol_RolId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_RolId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "RolId",
                table: "User");

            migrationBuilder.CreateTable(
                name: "UserRol",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RolId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRol", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRol_Rol_RolId",
                        column: x => x.RolId,
                        principalTable: "Rol",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRol_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserRol_RolId",
                table: "UserRol",
                column: "RolId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRol_UserId",
                table: "UserRol",
                column: "UserId");
        }
    }
}
