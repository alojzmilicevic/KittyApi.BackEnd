using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KittyAPI.Migrations
{
    /// <inheritdoc />
    public partial class test4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StreamUserModel");

            migrationBuilder.CreateTable(
                name: "StreamUser",
                columns: table => new
                {
                    StreamsStreamId = table.Column<int>(type: "int", nullable: false),
                    UsersUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreamUser", x => new { x.StreamsStreamId, x.UsersUserId });
                    table.ForeignKey(
                        name: "FK_StreamUser_Streams_StreamsStreamId",
                        column: x => x.StreamsStreamId,
                        principalTable: "Streams",
                        principalColumn: "StreamId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StreamUser_Users_UsersUserId",
                        column: x => x.UsersUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StreamUser_UsersUserId",
                table: "StreamUser",
                column: "UsersUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StreamUser");

            migrationBuilder.CreateTable(
                name: "StreamUserModel",
                columns: table => new
                {
                    StreamsStreamId = table.Column<int>(type: "int", nullable: false),
                    UsersUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreamUserModel", x => new { x.StreamsStreamId, x.UsersUserId });
                    table.ForeignKey(
                        name: "FK_StreamUserModel_Streams_StreamsStreamId",
                        column: x => x.StreamsStreamId,
                        principalTable: "Streams",
                        principalColumn: "StreamId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StreamUserModel_Users_UsersUserId",
                        column: x => x.UsersUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StreamUserModel_UsersUserId",
                table: "StreamUserModel",
                column: "UsersUserId");
        }
    }
}
