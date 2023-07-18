using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP_API.Migrations
{
    /// <inheritdoc />
    public partial class addviewscolumntweet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Views",
                table: "tweets",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Views",
                table: "tweets");
        }
    }
}
