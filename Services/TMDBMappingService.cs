using Microsoft.Extensions.Options;
using MovieProMVC.Enums;
using MovieProMVC.Models.Database;
using MovieProMVC.Models.Settings;
using MovieProMVC.Models.TMDB;
using MovieProMVC.Services.Interfaces;

namespace MovieProMVC.Services
{
    public class TMDBMappingService : IDataMappingService
    {
        private readonly AppSettings _appSettings;
        private readonly IImageService _imageService;
        private readonly IRemoteMovieService _tmdbMovieService;
        public TMDBMappingService(IOptions<AppSettings> appSettings, IImageService imageService, IRemoteMovieService tmdbMovieService)
        {
            _appSettings = appSettings.Value;
            _imageService = imageService;
            _tmdbMovieService = tmdbMovieService;
        }

        public ActorDetail MapActorDetail(ActorDetail actor)
        {
            // 1. Image
            actor.profile_path = BuildCastImage(actor.profile_path);

            // 2. Bio
            if (string.IsNullOrEmpty(actor.biography))
                actor.biography = "Not Available";

            // 3. Place of birth
            if (string.IsNullOrEmpty(actor.place_of_birth))
                actor.place_of_birth = "Not Available";

            // 4. Birthday
            if (string.IsNullOrEmpty(actor.birthday))
                actor.birthday = "Not Available";
            else
                actor.birthday = DateTime.Parse(actor.birthday).ToString("MMM dd, yyyy");
            
            return actor;
        }

        public async Task<Movie> MapMovieDetailAsync(MovieDetail movie)
        {
            Movie newMovie = null;

            try
            {
                newMovie = new Movie()
                {
                    MovieId = movie.id,
                    Title = movie.title,
                    TagLine = movie.tagline,
                    Overview = movie.overview,
                    RunTime = movie.runtime,
                    VoteAverage = movie.vote_average,
                    ReleaseDate = DateTime.Parse(movie.release_date),
                    TrailerUrl = BuildTrailerPath(movie.videos),
                    Backdrop = await EncodeBackdropImageAsync(movie.backdrop_path),
                    BackdropType = BuildImageType(movie.backdrop_path),
                    Poster = await EncodePosterImageAsync(movie.poster_path),
                    PosterType = BuildImageType(movie.poster_path),
                    Rating = GetRating(movie.release_dates),
                    Genres = GetGenres(movie.genres),
                    Country = GetCountry(movie.production_countries)
                };

                var castMembers = movie.credits.cast.OrderByDescending(c => c.popularity)
                    .GroupBy(c => c.cast_id)
                    .Select(g => g.FirstOrDefault())
                    .Take(20)
                    .ToList();

                castMembers.ForEach(member =>
                {
                    newMovie.Cast.Add(new MovieCast()
                    {
                        CastId = member.id,
                        Department = member.known_for_department,
                        Name = member.name,
                        Character = member.character,
                        ImageUrl = BuildCastImage(member.profile_path),
                    });
                });

                var crewMembers = movie.credits.crew.OrderByDescending(c => c.popularity)
                    .GroupBy(c => c.id)
                    .Select(g => g.First())
                    .Take(20)
                    .ToList();

                crewMembers.ForEach(member =>
                {
                    newMovie.Crew.Add(new MovieCrew()
                    {
                        CrewId = member.id,
                        Department = member.department,
                        Name = member.name,
                        Job = member.job,
                        ImageUrl = BuildCastImage(member.profile_path),
                    });
                });

                var similarMovies = movie.similar.results.OrderByDescending(m => m.popularity)
                    .GroupBy(m => m.id)
                    .Select(g => g.First())
                    .Take(10)
                    .ToList();

                similarMovies.ForEach(similar =>
                {
                    newMovie.MovieSimilar.Add(new MovieSimilar()
                    {
                        MovieId = similar.id,
                        Title = similar.title,
                        VoteAverage = similar.vote_average,
                        Genres = _tmdbMovieService.GetMovieGenresByIdAsync(similar.genre_ids),
                        PosterPath = similar.poster_path,
                    });
                });

                var reviews = movie.reviews.results.OrderByDescending(r => r.author_details.rating)
                    .GroupBy(r => r.id)
                    .Select(g => g.First())
                    .Take(3)
                    .ToList();

                if (reviews is not null)
                {
                    reviews.ForEach(review =>
                    {
                        newMovie.Reviews.Add(new MovieReview()
                        {
                            ReviewId = review.id,
                            Author = review.author,
                            AvatarPath = BuildAvatarImage(review.author_details.avatar_path),
                            Rating = review.author_details.rating ?? (int)MovieRating.G,
                            Content = review.content,
                            Created_at = DateTime.Parse(review.created_at),
                            Updated_at = DateTime.Parse(review.updated_at),
                        });
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in MapMovieDetailAsync: {ex.Message}");
            }

            return newMovie;
        }

        private string BuildTrailerPath(Videos videos)
        {
            var videoKey = videos.results.FirstOrDefault(r => r.type.ToLower().Trim() == "trailer" && r.key != "" )?.key;
            return string.IsNullOrEmpty(videoKey) ? videoKey : $"{_appSettings.TMDBSettings.BaseYouTubePath}{videoKey}";
        }

        private async Task<byte[]> EncodeBackdropImageAsync(string path)
        {
            var backdropPath = $"{_appSettings.TMDBSettings.BaseImagePath}/{_appSettings.MovieProSettings.DefaultBackdropSize}/{path}";
            return await _imageService.EncodeImageUrlAsync(backdropPath);
        }

        private string BuildImageType(string path)
        {
            if (string.IsNullOrEmpty(path)) return path;

            return $"image/{Path.GetExtension(path).TrimStart('.')}";
        }

        private async Task<byte[]> EncodePosterImageAsync(string path)
        {
            var posterPath = $"{_appSettings.TMDBSettings.BaseImagePath}/{_appSettings.MovieProSettings.DefaultPosterSize}/{path}";
            return await _imageService.EncodeImageUrlAsync(posterPath);
        }

        private MovieRating GetRating(Release_Dates dates)
        {
            var movieRating = MovieRating.NR;
            var certification = dates.results.FirstOrDefault(r => r.iso_3166_1 == "US");
            if (certification is not null)
            {
                var apiRating = certification.release_dates.FirstOrDefault(c => c.certification != "")?.certification.Replace("-", "");
                if (!string.IsNullOrEmpty(apiRating))
                {
                    movieRating = (MovieRating)Enum.Parse(typeof(MovieRating), apiRating, true);
                }
            }
            return movieRating;
        }
        
        private MovieGenre[] GetGenres(Genre[] genres)
        {
            List<MovieGenre> movieGenres = new List<MovieGenre>();

            if (genres is not null)
            {
                foreach (var genre in genres)
                {
                    var decodedGenre = (MovieGenre)genre.id;
                    movieGenres.Add(decodedGenre);
                }
            }

            return movieGenres.ToArray();
        }

        private string GetCountry(Production_Countries[] production_countries)
        {
            var country = "N/A";

            if (production_countries != null && production_countries.Length > 0)
            {
                country = production_countries.First().name;
            }

            return country;
        }

        private string BuildCastImage(string path)
        {
            if (string.IsNullOrEmpty(path)) return _appSettings.MovieProSettings.DefaultCastImage;

            return $"{_appSettings.TMDBSettings.BaseImagePath}/{_appSettings.MovieProSettings.DefaultPosterSize}/{path}";
        }

        private string BuildAvatarImage(string path)
        {
            if (string.IsNullOrEmpty(path)) return _appSettings.MovieProSettings.DefaultAvatarImage;

            if (path.Contains("https")) return path.Substring(1);

            return $"{_appSettings.TMDBSettings.BaseImagePath}/{_appSettings.MovieProSettings.DefaultPosterSize}/{path}";
        }

    }
}
