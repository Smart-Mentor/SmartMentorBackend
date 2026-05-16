using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMentor.Persistence.Migrations
{
    public partial class AddUserLifecycleAnalyticsColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "EmailVerifiedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoginAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProfileCompletedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE AspNetUsers
                SET EmailVerifiedAt = CreatedAt
                WHERE EmailConfirmed = 1 AND EmailVerifiedAt IS NULL
                """);

            migrationBuilder.Sql("""
                UPDATE AspNetUsers
                SET ProfileCompletedAt = CreatedAt
                WHERE IsProfileCompleted = 1 AND ProfileCompletedAt IS NULL
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EmailVerifiedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastLoginAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProfileCompletedAt",
                table: "AspNetUsers");
        }
    }
}
