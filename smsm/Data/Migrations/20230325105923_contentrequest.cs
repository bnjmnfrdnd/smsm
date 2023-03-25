using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace smsm.Data.Migrations
{
    /// <inheritdoc />
    public partial class contentrequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Archived",
                table: "ContentRequests",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateTime",
                table: "ContentRequests",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Archived",
                table: "ContentRequests");

            migrationBuilder.DropColumn(
                name: "CreatedDateTime",
                table: "ContentRequests");
        }
    }
}
