using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BulkCarnageIQ.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GroupFoodItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GroupFoodItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupFoodItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GroupFoodItemEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupFoodItemId = table.Column<int>(type: "int", nullable: false),
                    RecipeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PortionAmount = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupFoodItemEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupFoodItemEntries_GroupFoodItems_GroupFoodItemId",
                        column: x => x.GroupFoodItemId,
                        principalTable: "GroupFoodItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupFoodItemEntries_GroupFoodItemId",
                table: "GroupFoodItemEntries",
                column: "GroupFoodItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupFoodItemEntries");

            migrationBuilder.DropTable(
                name: "GroupFoodItems");
        }
    }
}
