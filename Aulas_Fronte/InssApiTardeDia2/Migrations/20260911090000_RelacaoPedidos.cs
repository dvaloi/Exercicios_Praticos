using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InssApi.Migrations
{
    /// <inheritdoc />
    public partial class RelacaoPedidos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_ContribuinteId",
                table: "Pedidos",
                column: "ContribuinteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Contribuintes_ContribuinteId",
                table: "Pedidos",
                column: "ContribuinteId",
                principalTable: "Contribuintes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Contribuintes_ContribuinteId",
                table: "Pedidos");

            migrationBuilder.DropIndex(
                name: "IX_Pedidos_ContribuinteId",
                table: "Pedidos");
        }
    }
}
