using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieProMVC.Data.Migrations
{
    public partial class AddNewDbSets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovieCast_Movie_MovieId",
                table: "MovieCast");

            migrationBuilder.DropForeignKey(
                name: "FK_MovieCrew_Movie_MovieId",
                table: "MovieCrew");

            migrationBuilder.DropForeignKey(
                name: "FK_MovieReview_Movie_MovieId",
                table: "MovieReview");

            migrationBuilder.DropForeignKey(
                name: "FK_MovieSimilar_Movie_MovieId",
                table: "MovieSimilar");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieSimilar",
                table: "MovieSimilar");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieReview",
                table: "MovieReview");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieCrew",
                table: "MovieCrew");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieCast",
                table: "MovieCast");

            migrationBuilder.RenameTable(
                name: "MovieSimilar",
                newName: "Similar");

            migrationBuilder.RenameTable(
                name: "MovieReview",
                newName: "Review");

            migrationBuilder.RenameTable(
                name: "MovieCrew",
                newName: "Crew");

            migrationBuilder.RenameTable(
                name: "MovieCast",
                newName: "Cast");

            migrationBuilder.RenameIndex(
                name: "IX_MovieSimilar_MovieId",
                table: "Similar",
                newName: "IX_Similar_MovieId");

            migrationBuilder.RenameIndex(
                name: "IX_MovieReview_MovieId",
                table: "Review",
                newName: "IX_Review_MovieId");

            migrationBuilder.RenameIndex(
                name: "IX_MovieCrew_MovieId",
                table: "Crew",
                newName: "IX_Crew_MovieId");

            migrationBuilder.RenameIndex(
                name: "IX_MovieCast_MovieId",
                table: "Cast",
                newName: "IX_Cast_MovieId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Similar",
                table: "Similar",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Review",
                table: "Review",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Crew",
                table: "Crew",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cast",
                table: "Cast",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cast_Movie_MovieId",
                table: "Cast",
                column: "MovieId",
                principalTable: "Movie",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Crew_Movie_MovieId",
                table: "Crew",
                column: "MovieId",
                principalTable: "Movie",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Movie_MovieId",
                table: "Review",
                column: "MovieId",
                principalTable: "Movie",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Similar_Movie_MovieId",
                table: "Similar",
                column: "MovieId",
                principalTable: "Movie",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cast_Movie_MovieId",
                table: "Cast");

            migrationBuilder.DropForeignKey(
                name: "FK_Crew_Movie_MovieId",
                table: "Crew");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_Movie_MovieId",
                table: "Review");

            migrationBuilder.DropForeignKey(
                name: "FK_Similar_Movie_MovieId",
                table: "Similar");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Similar",
                table: "Similar");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Review",
                table: "Review");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Crew",
                table: "Crew");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cast",
                table: "Cast");

            migrationBuilder.RenameTable(
                name: "Similar",
                newName: "MovieSimilar");

            migrationBuilder.RenameTable(
                name: "Review",
                newName: "MovieReview");

            migrationBuilder.RenameTable(
                name: "Crew",
                newName: "MovieCrew");

            migrationBuilder.RenameTable(
                name: "Cast",
                newName: "MovieCast");

            migrationBuilder.RenameIndex(
                name: "IX_Similar_MovieId",
                table: "MovieSimilar",
                newName: "IX_MovieSimilar_MovieId");

            migrationBuilder.RenameIndex(
                name: "IX_Review_MovieId",
                table: "MovieReview",
                newName: "IX_MovieReview_MovieId");

            migrationBuilder.RenameIndex(
                name: "IX_Crew_MovieId",
                table: "MovieCrew",
                newName: "IX_MovieCrew_MovieId");

            migrationBuilder.RenameIndex(
                name: "IX_Cast_MovieId",
                table: "MovieCast",
                newName: "IX_MovieCast_MovieId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieSimilar",
                table: "MovieSimilar",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieReview",
                table: "MovieReview",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieCrew",
                table: "MovieCrew",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieCast",
                table: "MovieCast",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MovieCast_Movie_MovieId",
                table: "MovieCast",
                column: "MovieId",
                principalTable: "Movie",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MovieCrew_Movie_MovieId",
                table: "MovieCrew",
                column: "MovieId",
                principalTable: "Movie",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MovieReview_Movie_MovieId",
                table: "MovieReview",
                column: "MovieId",
                principalTable: "Movie",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MovieSimilar_Movie_MovieId",
                table: "MovieSimilar",
                column: "MovieId",
                principalTable: "Movie",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
