using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using MovieProMVC.Enums;
using MovieProMVC.Models.Settings;
using MovieProMVC.Models.TMDB;
using MovieProMVC.Services.Interfaces;
using System.Runtime.Serialization.Json;

namespace MovieProMVC.Services
{
    public class TMDBMovieService : IRemoteMovieService
    {
        private readonly AppSettings _appSettings;
        private readonly IHttpClientFactory _httpClient;

        public TMDBMovieService(IOptions<AppSettings> appSettings, IHttpClientFactory httpClient)
        {
            _appSettings = appSettings.Value;
            _httpClient = httpClient;
        }

        public async Task<ActorDetail> ActorDetailAsync(int id)
        {
            // Step 1: Setup a default instance of ActorDetail
            ActorDetail actorDetail = new();

            // Step 2: Assemble the full request uri string
            var query = $"{_appSettings.TMDBSettings.BaseUrl}/person/{id}";
            var queryParams = new Dictionary<string, string>()
            {
                { "api_key", _appSettings.MovieProSettings.TmdbApiKey },
                { "language", _appSettings.TMDBSettings.QueryOptions.Language}
            };

            var requestUri = QueryHelpers.AddQueryString(query, queryParams);

            // Step 3: Create a client and execute the request
            var client = _httpClient.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var response = await client.SendAsync(request);

            // Step 4: Deserialize and return the ActorDetail object
            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                var dcjs = new DataContractJsonSerializer(typeof(ActorDetail));
                actorDetail = dcjs.ReadObject(responseStream) as ActorDetail;
            }

            return actorDetail;
        }

        public async Task<MovieDetail> MovieDetailAsync(int id)
        {
            // Step 1: Setup a default instance of MovieDetail
            MovieDetail movieDetail = new();

            // Step 2: Assemble the full request uri string
            var query = $"{_appSettings.TMDBSettings.BaseUrl}/movie/{id}";
            var queryParams = new Dictionary<string, string>()
            {
                { "api_key", _appSettings.MovieProSettings.TmdbApiKey },
                { "language", _appSettings.TMDBSettings.QueryOptions.Language},
                { "append_to_response", _appSettings.TMDBSettings.QueryOptions.AppendToResponse }
            };

            var requestUri = QueryHelpers.AddQueryString(query, queryParams);

            // Step 3: Create a client and execute the request
            var client = _httpClient.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var response = await client.SendAsync(request);

            // Step 4: Deserialize and return the MovieDetail object
            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                var dcjs = new DataContractJsonSerializer(typeof(MovieDetail));
                movieDetail = dcjs.ReadObject(responseStream) as MovieDetail;
            }

            return movieDetail;
        }

        public async Task<MovieSearch> SearchMoviesAsync(MovieCategory category, int count)
        {
            // Step 1: Setup a default instance of MovieSearch
            MovieSearch movieSearch = new();

            // Step 2: Assemble the full request uri string
            var query = $"{_appSettings.TMDBSettings.BaseUrl}/movie/{category}";
            var queryParams = new Dictionary<string, string>()
            {
                { "api_key", _appSettings.MovieProSettings.TmdbApiKey },
                { "language", _appSettings.TMDBSettings.QueryOptions.Language},
                { "page", _appSettings.TMDBSettings.QueryOptions.Page }
            };

            var requestUri = QueryHelpers.AddQueryString(query, queryParams);

            // Step 3: Create a client and execute the request
            var client = _httpClient.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var response = await client.SendAsync(request);

            // Step 4: Deserialize and return the MovieSearch object
            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                var dcjs = new DataContractJsonSerializer(typeof(MovieSearch));
                movieSearch = (MovieSearch)dcjs.ReadObject(responseStream);
                movieSearch.results = movieSearch.results.Take(count).ToArray();
                movieSearch.results.ToList().ForEach(result => result.poster_path = $"{_appSettings.TMDBSettings.BaseImagePath}/{_appSettings.MovieProSettings.DefaultPosterSize}/{result.poster_path}");
            }

            return movieSearch;
        }

        public async Task<MovieSearch> SearchMoviesTrendingAsync(int count)
        {
            // Step 1: Setup a default instance of MovieSearch
            MovieSearch movieSearch = new();

            // Step 2: Assemble the full request uri string
            var query = $"{_appSettings.TMDBSettings.BaseUrl}/trending/movie/day";
            var queryParams = new Dictionary<string, string>()
            {
                { "api_key", _appSettings.MovieProSettings.TmdbApiKey },
            };

            var requestUri = QueryHelpers.AddQueryString(query, queryParams);

            // Step 3: Create a client and execute the request
            var client = _httpClient.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var response = await client.SendAsync(request);

            // Step 4: Deserialize and return the MovieSearch object
            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                var dcjs = new DataContractJsonSerializer(typeof(MovieSearch));
                movieSearch = (MovieSearch)dcjs.ReadObject(responseStream);
                movieSearch.results = movieSearch.results.Take(count).ToArray();
                movieSearch.results.ToList().ForEach(result => result.poster_path = $"{_appSettings.TMDBSettings.BaseImagePath}/{_appSettings.MovieProSettings.DefaultPosterSize}/{result.poster_path}");
            }

            return movieSearch;
        }

        public async Task<Genres> GetAllMovieGenresAsync()
        {
            // Step 1: Setup a default instance of Genres
            Genres movieGenres = new();

            // Step 2: Assemble the full request uri string
            var query = $"{_appSettings.TMDBSettings.BaseUrl}/genre/movie/list";
            var queryParams = new Dictionary<string, string>()
            {
                { "api_key", _appSettings.MovieProSettings.TmdbApiKey },
                { "language", _appSettings.TMDBSettings.QueryOptions.Language},
            };

            var requestUri = QueryHelpers.AddQueryString(query, queryParams);

            // Step 3: Create a client and execute the request
            var client = _httpClient.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var response = await client.SendAsync(request);

            // Step 4: Deserialize and return the MovieSearch object
            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                var dcjs = new DataContractJsonSerializer(typeof(Genres));
                movieGenres = (Genres)dcjs.ReadObject(responseStream);
            }

            return movieGenres;
        }

        public async Task<List<string>> GetMovieGenresByIdAsync(Genres allGenres, int[] genresId)
        {
            var movieGenres = new List<string>();

            foreach (var genreId in genresId)
            {
                movieGenres.Add(allGenres.genres.ToList().FirstOrDefault(g => g.id == genreId).name);
            }

            return movieGenres;
        }
    }
}
