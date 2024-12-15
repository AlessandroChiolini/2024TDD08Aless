using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class newservicecardnumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CopyTransactions");

            migrationBuilder.AddColumn<string>(
                name: "ServiceCard",
                table: "EventTickets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: "E1",
                column: "Date",
                value: new DateTime(2024, 12, 30, 18, 35, 22, 41, DateTimeKind.Utc).AddTicks(8308));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: "E10",
                column: "Date",
                value: new DateTime(2025, 1, 6, 18, 35, 22, 41, DateTimeKind.Utc).AddTicks(8875));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: "E2",
                column: "Date",
                value: new DateTime(2024, 12, 25, 18, 35, 22, 41, DateTimeKind.Utc).AddTicks(8860));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: "E3",
                column: "Date",
                value: new DateTime(2024, 12, 20, 18, 35, 22, 41, DateTimeKind.Utc).AddTicks(8865));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: "E4",
                column: "Date",
                value: new DateTime(2025, 1, 4, 18, 35, 22, 41, DateTimeKind.Utc).AddTicks(8867));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: "E5",
                column: "Date",
                value: new DateTime(2024, 12, 22, 18, 35, 22, 41, DateTimeKind.Utc).AddTicks(8868));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: "E6",
                column: "Date",
                value: new DateTime(2025, 1, 9, 18, 35, 22, 41, DateTimeKind.Utc).AddTicks(8870));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: "E7",
                column: "Date",
                value: new DateTime(2025, 1, 14, 18, 35, 22, 41, DateTimeKind.Utc).AddTicks(8871));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: "E8",
                column: "Date",
                value: new DateTime(2024, 12, 27, 18, 35, 22, 41, DateTimeKind.Utc).AddTicks(8873));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: "E9",
                column: "Date",
                value: new DateTime(2025, 1, 2, 18, 35, 22, 41, DateTimeKind.Utc).AddTicks(8874));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "ServiceCard",
                value: "1");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "ServiceCard",
                value: "2");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "ServiceCard",
                value: "3");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceCard",
                table: "EventTickets");

            migrationBuilder.CreateTable(
                name: "CopyTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NumberOfCopies = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CopyTransactions", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: "E1",
                column: "Date",
                value: new DateTime(2024, 12, 30, 15, 49, 56, 286, DateTimeKind.Utc).AddTicks(4447));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: "E10",
                column: "Date",
                value: new DateTime(2025, 1, 6, 15, 49, 56, 286, DateTimeKind.Utc).AddTicks(5087));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: "E2",
                column: "Date",
                value: new DateTime(2024, 12, 25, 15, 49, 56, 286, DateTimeKind.Utc).AddTicks(5023));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: "E3",
                column: "Date",
                value: new DateTime(2024, 12, 20, 15, 49, 56, 286, DateTimeKind.Utc).AddTicks(5028));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: "E4",
                column: "Date",
                value: new DateTime(2025, 1, 4, 15, 49, 56, 286, DateTimeKind.Utc).AddTicks(5030));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: "E5",
                column: "Date",
                value: new DateTime(2024, 12, 22, 15, 49, 56, 286, DateTimeKind.Utc).AddTicks(5079));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: "E6",
                column: "Date",
                value: new DateTime(2025, 1, 9, 15, 49, 56, 286, DateTimeKind.Utc).AddTicks(5081));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: "E7",
                column: "Date",
                value: new DateTime(2025, 1, 14, 15, 49, 56, 286, DateTimeKind.Utc).AddTicks(5082));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: "E8",
                column: "Date",
                value: new DateTime(2024, 12, 27, 15, 49, 56, 286, DateTimeKind.Utc).AddTicks(5084));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: "E9",
                column: "Date",
                value: new DateTime(2025, 1, 2, 15, 49, 56, 286, DateTimeKind.Utc).AddTicks(5085));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "ServiceCard",
                value: "CARD123456");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "ServiceCard",
                value: "CARD654321");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "ServiceCard",
                value: "CARD987654");
        }
    }
}
