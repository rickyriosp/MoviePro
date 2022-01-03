using MovieProMVC.Enums;
using MovieProMVC.Models.TMDB;

namespace MovieProMVC.Services.Interfaces
{
    public interface IRemoteMovieService
    {
        Task<MovieDetail> MovieDetailAsync(int id);
        Task<ActorDetail> ActorDetailAsync(int id);
        Task<MovieSearch> SearchMoviesAsync(MovieCategory category, int count);
        Task<Genres> GetMovieGenresAsync();
        Task<List<string>> GetMovieGenresByIdAsync(Genres allGenres, int[] genresId);
    }
}
