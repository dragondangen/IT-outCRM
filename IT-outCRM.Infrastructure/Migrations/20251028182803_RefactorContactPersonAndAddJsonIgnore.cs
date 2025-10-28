using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IT_outCRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorContactPersonAndAddJsonIgnore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "ContactPerson");

            migrationBuilder.RenameColumn(
                name: "SurName",
                table: "ContactPerson",
                newName: "MiddleName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MiddleName",
                table: "ContactPerson",
                newName: "SurName");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ContactPerson",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
