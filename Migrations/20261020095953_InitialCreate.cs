using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OliveOilApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OliveOilProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Region = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PricePerLiter = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    QuantityInStock = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OliveOilProducts", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "OliveOilProducts",
                columns: new[] { "Id", "Name", "PricePerLiter", "QuantityInStock", "Region" },
                values: new object[,]
                {
                    { 1, "Nabali Extra Virgin", 45.00m, 120, "Nablus" },
                    { 2, "Souri Premium", 50.00m, 80, "Jenin" },
                    { 3, "Rumi Classic", 38.50m, 200, "Tulkarm" },
                    { 4, "Nabali Cold Pressed", 55.00m, 60, "Salfit" },
                    { 5, "Souri Organic", 60.00m, 40, "Ramallah" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OliveOilProducts");
        }
    }
}
