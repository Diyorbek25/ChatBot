using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatBot.Core.Migrations
{
    /// <inheritdoc />
    public partial class Initial4 : Migration
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
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SelectedUserId",
                table: "SelectedUser",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SelectedUser_Users_UserId",
                table: "SelectedUser",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SelectedUser_Users_UserId",
                table: "SelectedUser");

            migrationBuilder.DropColumn(
                name: "SelectedUserId",
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
    }
}
