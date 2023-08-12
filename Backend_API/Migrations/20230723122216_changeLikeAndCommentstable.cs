using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP_API.Migrations
{
    /// <inheritdoc />
    public partial class changeLikeAndCommentstable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_comments_media_comments_CommentId",
                table: "comments_media");

            migrationBuilder.AlterColumn<int>(
                name: "CommentId",
                table: "comments_media",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "comments_media",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CommentParentId",
                table: "comments",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_comments_CommentParentId",
                table: "comments",
                column: "CommentParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_comments_comments_CommentParentId",
                table: "comments",
                column: "CommentParentId",
                principalTable: "comments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_comments_media_comments_CommentId",
                table: "comments_media",
                column: "CommentId",
                principalTable: "comments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_comments_comments_CommentParentId",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "FK_comments_media_comments_CommentId",
                table: "comments_media");

            migrationBuilder.DropIndex(
                name: "IX_comments_CommentParentId",
                table: "comments");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "comments_media");

            migrationBuilder.DropColumn(
                name: "CommentParentId",
                table: "comments");

            migrationBuilder.AlterColumn<int>(
                name: "CommentId",
                table: "comments_media",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_comments_media_comments_CommentId",
                table: "comments_media",
                column: "CommentId",
                principalTable: "comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
