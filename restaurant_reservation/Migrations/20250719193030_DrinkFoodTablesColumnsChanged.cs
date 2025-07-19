using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace restaurant_reservation.Migrations
{
    /// <inheritdoc />
    public partial class DrinkFoodTablesColumnsChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Food");

            migrationBuilder.DropColumn(
                name: "ContainsNuts",
                table: "Food");

            migrationBuilder.DropColumn(
                name: "IsVegetarian",
                table: "Food");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Drinks");

            migrationBuilder.DropColumn(
                name: "IsCarbonated",
                table: "Drinks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Food",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "ContainsNuts",
                table: "Food",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVegetarian",
                table: "Food",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Drinks",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsCarbonated",
                table: "Drinks",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
