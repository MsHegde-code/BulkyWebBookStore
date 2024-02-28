using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BulkyWeb.Migrations
{
    /// <inheritdoc />
    public partial class NewContactUsTableData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Contact",
                columns: new[] { "Id", "EmailAddress", "UserQuery" },
                values: new object[] { 1, "hello@gmail.com", "help!!" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Contact",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
