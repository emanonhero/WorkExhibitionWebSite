using Microsoft.EntityFrameworkCore.Migrations;

namespace Exhibition.Migrations
{
    public partial class createWork : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "projects",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    pName = table.Column<string>(nullable: true),
                    Discribe = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "works",
                columns: table => new
                {
                    wId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    wName = table.Column<string>(nullable: true),
                    projId = table.Column<int>(nullable: false),
                    Discribe = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_works", x => x.wId);
                    table.ForeignKey(
                        name: "FK_works_projects_projId",
                        column: x => x.projId,
                        principalTable: "projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "imgs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Src = table.Column<string>(nullable: true),
                    Discribe = table.Column<string>(nullable: true),
                    wId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_imgs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_imgs_works_wId",
                        column: x => x.wId,
                        principalTable: "works",
                        principalColumn: "wId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_imgs_wId",
                table: "imgs",
                column: "wId");

            migrationBuilder.CreateIndex(
                name: "IX_works_projId",
                table: "works",
                column: "projId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "imgs");

            migrationBuilder.DropTable(
                name: "works");

            migrationBuilder.DropTable(
                name: "projects");
        }
    }
}
