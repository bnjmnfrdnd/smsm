using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace smsm.Data.Migrations
{
    /// <inheritdoc />
    public partial class contentrequestupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "ContentRequests",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "ContentRequests",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ContentRequests");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "ContentRequests");
        }
    }
}
