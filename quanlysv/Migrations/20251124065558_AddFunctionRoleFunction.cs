using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quanlysv.Migrations
{
    /// <inheritdoc />
    public partial class AddFunctionRoleFunction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClassName",
                table: "students");

            migrationBuilder.AlterColumn<int>(
                name: "HomeroomTeacher",
                table: "classes",
                type: "int",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "functions",
                columns: table => new
                {
                    FunctionCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FunctionName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_functions", x => x.FunctionCode);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "role_functions",
                columns: table => new
                {
                    RoleID = table.Column<int>(type: "int", nullable: false),
                    FunctionCode = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RoleID1 = table.Column<int>(type: "int", nullable: true),
                    FunctionCode1 = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_functions", x => new { x.RoleID, x.FunctionCode });
                    table.ForeignKey(
                        name: "FK_role_functions_functions_FunctionCode",
                        column: x => x.FunctionCode,
                        principalTable: "functions",
                        principalColumn: "FunctionCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_role_functions_functions_FunctionCode1",
                        column: x => x.FunctionCode1,
                        principalTable: "functions",
                        principalColumn: "FunctionCode");
                    table.ForeignKey(
                        name: "FK_role_functions_roles_RoleID",
                        column: x => x.RoleID,
                        principalTable: "roles",
                        principalColumn: "RoleID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_role_functions_roles_RoleID1",
                        column: x => x.RoleID1,
                        principalTable: "roles",
                        principalColumn: "RoleID");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_role_functions_FunctionCode",
                table: "role_functions",
                column: "FunctionCode");

            migrationBuilder.CreateIndex(
                name: "IX_role_functions_FunctionCode1",
                table: "role_functions",
                column: "FunctionCode1");

            migrationBuilder.CreateIndex(
                name: "IX_role_functions_RoleID1",
                table: "role_functions",
                column: "RoleID1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "role_functions");

            migrationBuilder.DropTable(
                name: "functions");

            migrationBuilder.AddColumn<string>(
                name: "ClassName",
                table: "students",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "HomeroomTeacher",
                table: "classes",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 100)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
