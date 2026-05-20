using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookPortal.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLoanAndPurchase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Loans_UserDocument",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "BookTitle",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "UserDocument",
                table: "Loans");

            migrationBuilder.AlterColumn<string>(
                name: "Cpf",
                table: "Users",
                type: "TEXT",
                maxLength: 11,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldMaxLength: 11);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAT",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Purchases",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Purchases",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Loans",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Loans_UserId",
                table: "Loans",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Loans_UserId",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "CreatedAT",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Loans");

            migrationBuilder.AlterColumn<int>(
                name: "Cpf",
                table: "Users",
                type: "INTEGER",
                maxLength: 11,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 11);

            migrationBuilder.AddColumn<string>(
                name: "BookTitle",
                table: "Loans",
                type: "TEXT",
                maxLength: 240,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserDocument",
                table: "Loans",
                type: "TEXT",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_UserDocument",
                table: "Loans",
                column: "UserDocument");
        }
    }
}
