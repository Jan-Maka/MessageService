using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Server.Migrations
{
    /// <inheritdoc />
    public partial class fixedTypoInConversationAndGroupchat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastMessageRecieved",
                table: "GroupChats",
                newName: "LastMessageReceived");

            migrationBuilder.RenameColumn(
                name: "LastMessageRecieved",
                table: "Conversations",
                newName: "LastMessageReceived");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastMessageReceived",
                table: "GroupChats",
                newName: "LastMessageRecieved");

            migrationBuilder.RenameColumn(
                name: "LastMessageReceived",
                table: "Conversations",
                newName: "LastMessageRecieved");
        }
    }
}
