namespace MovieProMVC.Models.Database
{
    public class MovieReview
    {
        public string Id { get; set; }
        public int MovieId { get; set; }
        public string Author { get; set; }
        public string Avatar_path { get; set; }
        public int Rating { get; set; }
        public string Content { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }
        public Movie Movie { get; set; }
    }
}
