using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KittyAPI.Migrations
{
    /// <inheritdoc />
    public partial class test3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Streams_StreamId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_StreamId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "StreamId",
                table: "Users");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StreamUserModel");

            migrationBuilder.AddColumn<int>(
                name: "StreamId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_StreamId",
                table: "Users",
                column: "StreamId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Streams_StreamId",
                table: "Users",
                column: "StreamId",
                principalTable: "Streams",
                principalColumn: "StreamId");
        }
    }
}
