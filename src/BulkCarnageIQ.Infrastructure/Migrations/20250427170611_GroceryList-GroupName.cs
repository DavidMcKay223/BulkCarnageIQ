using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BulkCarnageIQ.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GroceryListGroupName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GroupName",
                table: "GroceryListItems",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupName",
                table: "GroceryListItems");
        }
    }
}
