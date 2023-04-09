using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatBot.Core.Migrations
{
    /// <inheritdoc />
    public partial class Initial3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SelectedUser_Users_UserId",
                table: "SelectedUser");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "SelectedUser",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_SelectedUser_Users_UserId",
                table: "SelectedUser",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SelectedUser_Users_UserId",
                table: "SelectedUser");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "SelectedUser",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SelectedUser_Users_UserId",
                table: "SelectedUser",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
