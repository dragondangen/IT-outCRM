using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IT_outCRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SupportTeamId",
                table: "Orders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "Executors",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "Customers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ContactPersonID",
                table: "Companies",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Orders_SupportTeamId",
                table: "Orders",
                column: "SupportTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Executors_CompanyId",
                table: "Executors",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_CompanyId",
                table: "Customers",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_ContactPersonID",
                table: "Companies",
                column: "ContactPersonID");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_ContactPerson_ContactPersonID",
                table: "Companies",
                column: "ContactPersonID",
                principalTable: "ContactPerson",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Companies_CompanyId",
                table: "Customers",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Executors_Companies_CompanyId",
                table: "Executors",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_orderSupportTeams_SupportTeamId",
                table: "Orders",
                column: "SupportTeamId",
                principalTable: "orderSupportTeams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_ContactPerson_ContactPersonID",
                table: "Companies");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Companies_CompanyId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_Executors_Companies_CompanyId",
                table: "Executors");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_orderSupportTeams_SupportTeamId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_SupportTeamId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Executors_CompanyId",
                table: "Executors");

            migrationBuilder.DropIndex(
                name: "IX_Customers_CompanyId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Companies_ContactPersonID",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "SupportTeamId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Executors");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ContactPersonID",
                table: "Companies");
        }
    }
}
