using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP_API.Migrations
{
    /// <inheritdoc />
    public partial class addcountrycode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "City",
                table: "AspNetUsers",
                newName: "CountryCode");

            migrationBuilder.AlterColumn<string>(
                name: "BackgroundImage",
                table: "AspNetUsers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CountryCode",
                table: "AspNetUsers",
                newName: "City");

            migrationBuilder.AlterColumn<string>(
                name: "BackgroundImage",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
