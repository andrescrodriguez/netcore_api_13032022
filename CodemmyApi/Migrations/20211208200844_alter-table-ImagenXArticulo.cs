using Microsoft.EntityFrameworkCore.Migrations;

namespace CodemmyApi.Migrations
{
    public partial class altertableImagenXArticulo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "ImagenXArticulo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ImagenXArticulo",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
