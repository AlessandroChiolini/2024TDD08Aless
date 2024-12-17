using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceCardToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Users",
                newName: "ServiceCard");

            migrationBuilder.UpdateData(
                table: "CopyTransactions",
                keyColumn: "Id",
                keyValue: 1,
                column: "Date",
                value: new DateTime(2024, 12, 15, 15, 26, 23, 989, DateTimeKind.Utc).AddTicks(4297));

            migrationBuilder.UpdateData(
                table: "CopyTransactions",
                keyColumn: "Id",
                keyValue: 2,
                column: "Date",
                value: new DateTime(2024, 12, 15, 15, 26, 23, 989, DateTimeKind.Utc).AddTicks(5533));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: "E1",
                column: "Date",
                value: new DateTime(2024, 12, 30, 15, 26, 23, 989, DateTimeKind.Utc).AddTicks(5946));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: "E2",
                column: "Date",
                value: new DateTime(2024, 12, 25, 15, 26, 23, 989, DateTimeKind.Utc).AddTicks(6404));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Balance", "Name", "ServiceCard" },
                values: new object[] { 1500m, "Alessandro Chiolini", "CARD123456" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Balance", "Name", "ServiceCard" },
                values: new object[] { 1000m, "Julien Blanch-Lanao", "CARD654321" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Balance", "Name", "ServiceCard" },
                values: new object[] { 1200m, "Gian-Luca Gloor", "CARD987654" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ServiceCard",
                table: "Users",
                newName: "Email");

            migrationBuilder.UpdateData(
                table: "CopyTransactions",
                keyColumn: "Id",
                keyValue: 1,
                column: "Date",
                value: new DateTime(2024, 12, 15, 14, 54, 24, 328, DateTimeKind.Utc).AddTicks(6489));

            migrationBuilder.UpdateData(
                table: "CopyTransactions",
                keyColumn: "Id",
                keyValue: 2,
                column: "Date",
                value: new DateTime(2024, 12, 15, 14, 54, 24, 328, DateTimeKind.Utc).AddTicks(8014));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: "E1",
                column: "Date",
                value: new DateTime(2024, 12, 30, 14, 54, 24, 328, DateTimeKind.Utc).AddTicks(8638));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: "E2",
                column: "Date",
                value: new DateTime(2024, 12, 25, 14, 54, 24, 328, DateTimeKind.Utc).AddTicks(9387));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Balance", "Email", "Name" },
                values: new object[] { 50m, "alexandre.salamin@mail.com", "Alexandre Salamin" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Balance", "Email", "Name" },
                values: new object[] { 100m, "jonathan.araujo@mail.com", "Jonathan Araujo" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Balance", "Email", "Name" },
                values: new object[] { 80m, "adrien.destefani@mail.com", "Adrien Destefani" });
        }
    }
}
