using MovieProMVC.Enums;
using MovieProMVC.Models.TMDB;

namespace MovieProMVC.Services.Interfaces
{
    public interface IRemoteMovieService
    {
        Task<MovieDetail> MovieDetailAsync(int id);
        Task<ActorDetail> ActorDetailAsync(int id);
        Task<MovieSearch> SearchMoviesAsync(MovieCategory category, int count);
        Task<MovieSearch> SearchMoviesTrendingAsync(int count);
        Task<MovieSearch> SearchMoviesSimilarAsync(int id, int count);
        Task<ActorSearch> SearchActorsAsync(int count);
        Task<Genres> GetAllMovieGenresAsync();
        MovieGenre[] GetMovieGenresByIdAsync(int[] genresId);
        string GenerateSearchRequestUri();
    }
}
