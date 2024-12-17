using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEventSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CopyTransactions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CopyTransactions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: "E1",
                column: "Date",
                value: new DateTime(2024, 12, 30, 15, 49, 56, 286, DateTimeKind.Utc).AddTicks(4447));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: "E2",
                column: "Date",
                value: new DateTime(2024, 12, 25, 15, 49, 56, 286, DateTimeKind.Utc).AddTicks(5023));

            migrationBuilder.InsertData(
                table: "Events",
                columns: new[] { "Id", "AvailableTickets", "Date", "Name", "TicketPrice" },
                values: new object[,]
                {
                    { "E10", 55, new DateTime(2025, 1, 6, 15, 49, 56, 286, DateTimeKind.Utc).AddTicks(5087), "Charity Auction", 35.0m },
                    { "E3", 75, new DateTime(2024, 12, 20, 15, 49, 56, 286, DateTimeKind.Utc).AddTicks(5028), "Art Exhibition", 20.0m },
                    { "E4", 60, new DateTime(2025, 1, 4, 15, 49, 56, 286, DateTimeKind.Utc).AddTicks(5030), "Tech Workshop", 30.0m },
                    { "E5", 90, new DateTime(2024, 12, 22, 15, 49, 56, 286, DateTimeKind.Utc).AddTicks(5079), "Film Screening", 15.0m },
                    { "E6", 120, new DateTime(2025, 1, 9, 15, 49, 56, 286, DateTimeKind.Utc).AddTicks(5081), "Sports Gala", 40.0m },
                    { "E7", 150, new DateTime(2025, 1, 14, 15, 49, 56, 286, DateTimeKind.Utc).AddTicks(5082), "Music Festival", 70.0m },
                    { "E8", 40, new DateTime(2024, 12, 27, 15, 49, 56, 286, DateTimeKind.Utc).AddTicks(5084), "Literature Meetup", 10.0m },
                    { "E9", 80, new DateTime(2025, 1, 2, 15, 49, 56, 286, DateTimeKind.Utc).AddTicks(5085), "Business Conference", 60.0m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: "E10");

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: "E3");

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: "E4");

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: "E5");

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: "E6");

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: "E7");

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: "E8");

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: "E9");

            migrationBuilder.InsertData(
                table: "CopyTransactions",
                columns: new[] { "Id", "Amount", "Date", "NumberOfCopies", "UserId" },
                values: new object[,]
                {
                    { 1, 1.00m, new DateTime(2024, 12, 15, 15, 26, 23, 989, DateTimeKind.Utc).AddTicks(4297), 10, 1 },
                    { 2, 0.50m, new DateTime(2024, 12, 15, 15, 26, 23, 989, DateTimeKind.Utc).AddTicks(5533), 5, 1 }
                });

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
        }
    }
}
