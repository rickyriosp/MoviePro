using MovieProMVC.Enums;
using System.ComponentModel.DataAnnotations;

namespace MovieProMVC.Models.Database
{
    public class MovieSimilar
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public string Title { get; set; }
        public float VoteAverage { get; set; }
        public MovieGenre[] Genres { get; set; }
        [Display(Name = "Poster")]
        public string PosterPath { get; set; }
        public Movie Movie { get; set; }
    }
}
