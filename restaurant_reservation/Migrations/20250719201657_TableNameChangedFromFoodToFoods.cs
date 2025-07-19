using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace restaurant_reservation.Migrations
{
    /// <inheritdoc />
    public partial class TableNameChangedFromFoodToFoods : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Food_Menus_MenuId",
                table: "Food");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Food",
                table: "Food");

            migrationBuilder.RenameTable(
                name: "Food",
                newName: "Foods");

            migrationBuilder.RenameIndex(
                name: "IX_Food_MenuId",
                table: "Foods",
                newName: "IX_Foods_MenuId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Foods",
                table: "Foods",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Foods_Menus_MenuId",
                table: "Foods",
                column: "MenuId",
                principalTable: "Menus",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Foods_Menus_MenuId",
                table: "Foods");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Foods",
                table: "Foods");

            migrationBuilder.RenameTable(
                name: "Foods",
                newName: "Food");

            migrationBuilder.RenameIndex(
                name: "IX_Foods_MenuId",
                table: "Food",
                newName: "IX_Food_MenuId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Food",
                table: "Food",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Food_Menus_MenuId",
                table: "Food",
                column: "MenuId",
                principalTable: "Menus",
                principalColumn: "Id");
        }
    }
}
