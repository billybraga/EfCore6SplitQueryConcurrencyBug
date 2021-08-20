using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EfCore6SplitQueryConcurrencyBug.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Parents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Children",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Children", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Children_Parents_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Parents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Children_ParentId",
                table: "Children",
                column: "ParentId");
            
            migrationBuilder
                .InsertData(
                    "Parents",
                    new[] { "Id", "Name" },
                    new[,]
                    {
                        { "13319478-bec7-4a9f-bddc-dde28c1b5787", "Entity1" },
                        { "249cad7c-7706-4a1e-84a6-b7f37485c71b", "Entity2" },
                    }
                );
                
            migrationBuilder
                .InsertData(
                    "Children",
                    new[] { "Id", "ParentId", "Name" },
                    new[,]
                    {
                        { "70c7ddba-7e09-4b28-8c27-609afe777d57", "13319478-bec7-4a9f-bddc-dde28c1b5787", "Children1.1" },
                        { "bd3b9d1b-f6fd-40d9-9a34-a8658e81f2c6", "249cad7c-7706-4a1e-84a6-b7f37485c71b", "Children2.1" },
                    }
                );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Children");

            migrationBuilder.DropTable(
                name: "Parents");
        }
    }
}
