using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ASP_API.Migrations
{
    /// <inheritdoc />
    public partial class replies2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "comments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TweetId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CommentParentId = table.Column<int>(type: "integer", nullable: true),
                    IsComment = table.Column<bool>(type: "boolean", nullable: false),
                    IsReply = table.Column<bool>(type: "boolean", nullable: false),
                    ReplyToId = table.Column<int>(type: "integer", nullable: true),
                    CommentText = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_comments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_comments_comments_ReplyToId",
                        column: x => x.ReplyToId,
                        principalTable: "comments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_comments_tweets_TweetId",
                        column: x => x.TweetId,
                        principalTable: "tweets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_comments_media_CommentId",
                table: "comments_media",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_comments_ReplyToId",
                table: "comments",
                column: "ReplyToId");

            migrationBuilder.CreateIndex(
                name: "IX_comments_TweetId",
                table: "comments",
                column: "TweetId");

            migrationBuilder.CreateIndex(
                name: "IX_comments_UserId",
                table: "comments",
                column: "UserId");

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
                name: "FK_comments_media_comments_CommentId",
                table: "comments_media");

            migrationBuilder.DropTable(
                name: "comments");

            migrationBuilder.DropIndex(
                name: "IX_comments_media_CommentId",
                table: "comments_media");
        }
    }
}
