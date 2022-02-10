using System.ComponentModel.DataAnnotations;

namespace MovieProMVC.Models.Database
{
    public class MovieReview
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public string ReviewId { get; set; }
        public string Author { get; set; }
        [Display(Name = "Avatar")]
        public string AvatarPath { get; set; }
        public float Rating { get; set; }
        public string Content { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }
        public Movie Movie { get; set; }
    }
}
