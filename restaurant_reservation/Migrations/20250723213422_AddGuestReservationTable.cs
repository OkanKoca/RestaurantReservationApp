using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace restaurant_reservation.Migrations
{
    /// <inheritdoc />
    public partial class AddGuestReservationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GuestReservation_Tables_TableId",
                table: "GuestReservation");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_AspNetUsers_CustomerId",
                table: "Reservation");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_Tables_TableId",
                table: "Reservation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reservation",
                table: "Reservation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GuestReservation",
                table: "GuestReservation");

            migrationBuilder.RenameTable(
                name: "Reservation",
                newName: "Reservations");

            migrationBuilder.RenameTable(
                name: "GuestReservation",
                newName: "GuestReservations");

            migrationBuilder.RenameIndex(
                name: "IX_Reservation_TableId",
                table: "Reservations",
                newName: "IX_Reservations_TableId");

            migrationBuilder.RenameIndex(
                name: "IX_Reservation_CustomerId",
                table: "Reservations",
                newName: "IX_Reservations_CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_GuestReservation_TableId",
                table: "GuestReservations",
                newName: "IX_GuestReservations_TableId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reservations",
                table: "Reservations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GuestReservations",
                table: "GuestReservations",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GuestReservations_Tables_TableId",
                table: "GuestReservations",
                column: "TableId",
                principalTable: "Tables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_AspNetUsers_CustomerId",
                table: "Reservations",
                column: "CustomerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Tables_TableId",
                table: "Reservations",
                column: "TableId",
                principalTable: "Tables",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GuestReservations_Tables_TableId",
                table: "GuestReservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_AspNetUsers_CustomerId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Tables_TableId",
                table: "Reservations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reservations",
                table: "Reservations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GuestReservations",
                table: "GuestReservations");

            migrationBuilder.RenameTable(
                name: "Reservations",
                newName: "Reservation");

            migrationBuilder.RenameTable(
                name: "GuestReservations",
                newName: "GuestReservation");

            migrationBuilder.RenameIndex(
                name: "IX_Reservations_TableId",
                table: "Reservation",
                newName: "IX_Reservation_TableId");

            migrationBuilder.RenameIndex(
                name: "IX_Reservations_CustomerId",
                table: "Reservation",
                newName: "IX_Reservation_CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_GuestReservations_TableId",
                table: "GuestReservation",
                newName: "IX_GuestReservation_TableId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reservation",
                table: "Reservation",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GuestReservation",
                table: "GuestReservation",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GuestReservation_Tables_TableId",
                table: "GuestReservation",
                column: "TableId",
                principalTable: "Tables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_AspNetUsers_CustomerId",
                table: "Reservation",
                column: "CustomerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_Tables_TableId",
                table: "Reservation",
                column: "TableId",
                principalTable: "Tables",
                principalColumn: "Id");
        }
    }
}
