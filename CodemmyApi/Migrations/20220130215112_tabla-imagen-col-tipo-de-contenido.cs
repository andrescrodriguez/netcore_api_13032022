using Microsoft.EntityFrameworkCore.Migrations;

namespace CodemmyApi.Migrations
{
    public partial class tablaimagencoltipodecontenido : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TipoDeContenido",
                table: "Imagen",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoDeContenido",
                table: "Imagen");
        }
    }
}
