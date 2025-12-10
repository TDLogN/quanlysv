using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quanlysv.Migrations
{
    /// <inheritdoc />
    public partial class AddFunctionRoleTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_role_functions_functions_FunctionCode1",
                table: "role_functions");

            migrationBuilder.DropForeignKey(
                name: "FK_role_functions_roles_RoleID1",
                table: "role_functions");

            migrationBuilder.DropIndex(
                name: "IX_role_functions_FunctionCode1",
                table: "role_functions");

            migrationBuilder.DropIndex(
                name: "IX_role_functions_RoleID1",
                table: "role_functions");

            migrationBuilder.DropColumn(
                name: "FunctionCode1",
                table: "role_functions");

            migrationBuilder.DropColumn(
                name: "RoleID1",
                table: "role_functions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FunctionCode1",
                table: "role_functions",
                type: "varchar(50)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "RoleID1",
                table: "role_functions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_role_functions_FunctionCode1",
                table: "role_functions",
                column: "FunctionCode1");

            migrationBuilder.CreateIndex(
                name: "IX_role_functions_RoleID1",
                table: "role_functions",
                column: "RoleID1");

            migrationBuilder.AddForeignKey(
                name: "FK_role_functions_functions_FunctionCode1",
                table: "role_functions",
                column: "FunctionCode1",
                principalTable: "functions",
                principalColumn: "FunctionCode");

            migrationBuilder.AddForeignKey(
                name: "FK_role_functions_roles_RoleID1",
                table: "role_functions",
                column: "RoleID1",
                principalTable: "roles",
                principalColumn: "RoleID");
        }
    }
}
