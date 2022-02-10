using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieProMVC.Data.Migrations
{
    public partial class AddRemoteIdToMovieSimilar : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RemoteId",
                table: "MovieSimilar",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemoteId",
                table: "MovieSimilar");
        }
    }
}
