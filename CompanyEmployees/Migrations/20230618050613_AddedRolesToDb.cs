using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CompanyEmployees.Migrations
{
    /// <inheritdoc />
    public partial class AddedRolesToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "a154fac2-32a0-4ca5-a686-4dfd236a4a9a", "872e64d6-efee-4957-9ec1-85a345a82b47", "Manager", "MANAGER" },
                    { "e19c7478-d16b-461d-aa68-61970b2b4fc2", "312b1ff8-cf14-4580-9cbb-a1dd07170605", "Administrator", "ADMINISTRATOR" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a154fac2-32a0-4ca5-a686-4dfd236a4a9a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e19c7478-d16b-461d-aa68-61970b2b4fc2");
        }
    }
}
