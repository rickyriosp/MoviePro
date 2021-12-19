using MovieProMVC.Models.Database;
using MovieProMVC.Models.TMDB;

namespace MovieProMVC.Services.Interfaces
{
    public interface IDataMappingService
    {
        Task<Movie> MapMovieDetailAsync(MovieDetail movie);
        ActorDetail MapActorDetail(ActorDetail actor);
    }
}
