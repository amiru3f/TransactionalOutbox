using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransactionalOutbox.Migrations
{
    /// <inheritdoc />
    public partial class indexfornotif : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Notif_CreatedAt",
                table: "Notif",
                column: "CreatedAt")
                .Annotation("SqlServer:Clustered", false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Notif_CreatedAt",
                table: "Notif");
        }
    }
}
