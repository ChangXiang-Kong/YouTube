using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BatchProcess3.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SkipNoActionFiles = table.Column<bool>(type: "INTEGER", nullable: false),
                    AllowDuplicateEntries = table.Column<bool>(type: "INTEGER", nullable: false),
                    LocationPaths = table.Column<string>(type: "TEXT", nullable: false),
                    SolidWorksHost = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PdmeVaultName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PdmeUserName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PdmePassword = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    UpdateTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    CreateTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Settings");
        }
    }
}
