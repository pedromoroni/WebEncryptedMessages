using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebMessages.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNewFieldsToMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Received",
                table: "Messages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Received",
                table: "Messages");
        }
    }
}
