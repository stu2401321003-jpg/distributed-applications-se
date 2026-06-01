using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentACarAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddTariffPlanToReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TariffPlanId",
                table: "Reservations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_TariffPlanId",
                table: "Reservations",
                column: "TariffPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_TariffPlans_TariffPlanId",
                table: "Reservations",
                column: "TariffPlanId",
                principalTable: "TariffPlans",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_TariffPlans_TariffPlanId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_TariffPlanId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "TariffPlanId",
                table: "Reservations");
        }
    }
}
