using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieProMVC.Data.Migrations
{
    public partial class MovieGenre : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string[]>(
                name: "Genres",
                table: "Movie",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Genres",
                table: "Movie");
        }
    }
}
