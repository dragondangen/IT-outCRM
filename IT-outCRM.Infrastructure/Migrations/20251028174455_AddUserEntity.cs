using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IT_outCRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_AccountsStatus_AccountStatusId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_orderStatuses_OrderStatusId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_orderSupportTeams_SupportTeamId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_orderSupportTeams_Admins_AdminId",
                table: "orderSupportTeams");

            migrationBuilder.DropPrimaryKey(
                name: "PK_orderSupportTeams",
                table: "orderSupportTeams");

            migrationBuilder.DropPrimaryKey(
                name: "PK_orderStatuses",
                table: "orderStatuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountsStatus",
                table: "AccountsStatus");

            migrationBuilder.RenameTable(
                name: "orderSupportTeams",
                newName: "OrderSupportTeams");

            migrationBuilder.RenameTable(
                name: "orderStatuses",
                newName: "OrderStatuses");

            migrationBuilder.RenameTable(
                name: "AccountsStatus",
                newName: "AccountStatuses");

            migrationBuilder.RenameIndex(
                name: "IX_orderSupportTeams_AdminId",
                table: "OrderSupportTeams",
                newName: "IX_OrderSupportTeams_AdminId");

            migrationBuilder.RenameIndex(
                name: "IX_orderStatuses_Name",
                table: "OrderStatuses",
                newName: "IX_OrderStatuses_Name");

            migrationBuilder.RenameIndex(
                name: "IX_AccountsStatus_Name",
                table: "AccountStatuses",
                newName: "IX_AccountStatuses_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderSupportTeams",
                table: "OrderSupportTeams",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderStatuses",
                table: "OrderStatuses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountStatuses",
                table: "AccountStatuses",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "User"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_AccountStatuses_AccountStatusId",
                table: "Accounts",
                column: "AccountStatusId",
                principalTable: "AccountStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_OrderStatuses_OrderStatusId",
                table: "Orders",
                column: "OrderStatusId",
                principalTable: "OrderStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_OrderSupportTeams_SupportTeamId",
                table: "Orders",
                column: "SupportTeamId",
                principalTable: "OrderSupportTeams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderSupportTeams_Admins_AdminId",
                table: "OrderSupportTeams",
                column: "AdminId",
                principalTable: "Admins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_AccountStatuses_AccountStatusId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_OrderStatuses_OrderStatusId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_OrderSupportTeams_SupportTeamId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderSupportTeams_Admins_AdminId",
                table: "OrderSupportTeams");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderSupportTeams",
                table: "OrderSupportTeams");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderStatuses",
                table: "OrderStatuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountStatuses",
                table: "AccountStatuses");

            migrationBuilder.RenameTable(
                name: "OrderSupportTeams",
                newName: "orderSupportTeams");

            migrationBuilder.RenameTable(
                name: "OrderStatuses",
                newName: "orderStatuses");

            migrationBuilder.RenameTable(
                name: "AccountStatuses",
                newName: "AccountsStatus");

            migrationBuilder.RenameIndex(
                name: "IX_OrderSupportTeams_AdminId",
                table: "orderSupportTeams",
                newName: "IX_orderSupportTeams_AdminId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderStatuses_Name",
                table: "orderStatuses",
                newName: "IX_orderStatuses_Name");

            migrationBuilder.RenameIndex(
                name: "IX_AccountStatuses_Name",
                table: "AccountsStatus",
                newName: "IX_AccountsStatus_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_orderSupportTeams",
                table: "orderSupportTeams",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_orderStatuses",
                table: "orderStatuses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountsStatus",
                table: "AccountsStatus",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_AccountsStatus_AccountStatusId",
                table: "Accounts",
                column: "AccountStatusId",
                principalTable: "AccountsStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_orderStatuses_OrderStatusId",
                table: "Orders",
                column: "OrderStatusId",
                principalTable: "orderStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_orderSupportTeams_SupportTeamId",
                table: "Orders",
                column: "SupportTeamId",
                principalTable: "orderSupportTeams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_orderSupportTeams_Admins_AdminId",
                table: "orderSupportTeams",
                column: "AdminId",
                principalTable: "Admins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
