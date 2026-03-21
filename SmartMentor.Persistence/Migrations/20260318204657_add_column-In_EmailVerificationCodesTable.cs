using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMentor.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class add_columnIn_EmailVerificationCodesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "VerficationToken",
                table: "EmailVerificationCodes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VerficationToken",
                table: "EmailVerificationCodes");
        }
    }
}
