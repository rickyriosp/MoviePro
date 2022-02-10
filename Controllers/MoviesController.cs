using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MovieProMVC.Data;
using MovieProMVC.Enums;
using MovieProMVC.Models.Database;
using MovieProMVC.Models.Settings;
using MovieProMVC.Services.Interfaces;
using Sentry;
using X.PagedList;

namespace MovieProMVC.Controllers
{
    public class MoviesController : Controller
    {
        private readonly AppSettings _appSettings;
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;
        private readonly IRemoteMovieService _tmdbMovieService;
        private readonly IDataMappingService _tmdbMappingService;

        private Dictionary<int, int> _libraryPages;

        public MoviesController(IOptions<AppSettings> appSettings, ApplicationDbContext context, IImageService imageService, IRemoteMovieService tmdbMovieService, IDataMappingService tmdbMappingService)
        {
            _appSettings = appSettings.Value;
            _context = context;
            _imageService = imageService;
            _tmdbMovieService = tmdbMovieService;
            _tmdbMappingService = tmdbMappingService;

            _libraryPages = new Dictionary<int, int>();
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
            return View(await _context.Movie.AsNoTracking().ToListAsync());
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id, bool local = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            Movie? movie = new();

            if (local is true)
            {
                try
                {
                    // Get the Movie data straight from the DB
                    movie = await _context.Movie
                        .Include(m => m.Cast)
                        .Include(m => m.Crew)
                        //.Include(m => m.Reviews)
                        .Include(m => m.MovieSimilar)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(m => m.Id == id);
                }
                catch (Exception err)
                {
                    SentrySdk.CaptureException(err);
                }
            }
            else
            {
                // Get the Movie data from the TMDB API
                var movieDetail = await _tmdbMovieService.MovieDetailAsync((int)id);
                movie = await _tmdbMappingService.MapMovieDetailAsync(movieDetail);
            }

            if (movie == null)
            {
                return NotFound();
            }

            ViewData["Local"] = local;

            return View(movie);
        }

        // GET: Movies/Create
        [Authorize(Roles = "Administrator, User")]
        public IActionResult Create()
        {
            ViewData["CollectionId"] = new SelectList(_context.Collection.AsNoTracking().ToList(), "Id", "Name");

            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Overview,RunTime,ReleaseDate,Rating,VoteAverage,Genres,Country,PosterFile,BackdropFile,TrailerUrl")] Movie movie, int collectionId)
        {
            if (ModelState.IsValid)
            {
                // Use the _imageService to store the incoming user specified image
                movie.PosterType = _imageService.ContentType(movie.PosterFile) ??
                                   Path.GetExtension(_appSettings.MovieProSettings.DefaultMoviePoster);
                movie.Poster = await _imageService.EncodeImageAsync(movie.PosterFile) ??
                               await _imageService.EncodeImageAsync(_appSettings.MovieProSettings.DefaultMoviePoster);

                movie.BackdropType = _imageService.ContentType(movie.BackdropFile) ??
                                   Path.GetExtension(_appSettings.MovieProSettings.DefaultMovieBackdrop);
                movie.Backdrop = await _imageService.EncodeImageAsync(movie.BackdropFile) ??
                               await _imageService.EncodeImageAsync(_appSettings.MovieProSettings.DefaultMovieBackdrop);

                movie.TrailerUrl = movie.TrailerUrl ?? _appSettings.MovieProSettings.DefaultTrailerUrl;

                // Set time Kind to UTC --> Npgsql throws error otherwise
                movie.ReleaseDate = movie.ReleaseDate.ToUniversalTime();

                _context.Add(movie);
                await _context.SaveChangesAsync();

                await AddToMovieCollection(movie.Id, collectionId);
                
                if (_context.Collection.First(c => c.Name.ToUpper() == _appSettings.MovieProSettings.DefaultCollection.Name.ToUpper()).Id != collectionId)
                {
                    await AddToMovieCollection(movie.Id, _appSettings.MovieProSettings.DefaultCollection.Name);
                }

                return RedirectToAction("Index", "MovieCollections");
            }
            return View(movie);
        }

        // GET: Movies/Edit/5
        [Authorize(Roles = "Administrator, User")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                var pageNumber = 1;
                var pageSize = 12;
                var movies = await _context.Movie
                    .AsNoTracking()
                    .ToPagedListAsync(pageNumber, pageSize);

                return View("EditIndex", movies);
            }

            ViewData["CollectionId"] = new SelectList(_context.Collection.AsNoTracking(), "Id", "Name");

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Overview,RunTime,ReleaseDate,Rating,VoteAverage,Genres,Country,TrailerUrl")] Movie movie)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // The original movie
                    var newMovie = await _context.Movie.FirstOrDefaultAsync(m => m.Id == movie.Id);

                    if (movie.PosterFile is not null)
                    {
                        newMovie.PosterType = movie.PosterFile?.ContentType;
                        newMovie.Poster = await _imageService.EncodeImageAsync(movie.PosterFile);
                    }

                    if (movie.BackdropFile is not null)
                    {
                        newMovie.BackdropType = movie.BackdropFile?.ContentType;
                        newMovie.Backdrop = await _imageService.EncodeImageAsync(movie.BackdropFile);
                    }

                    if (newMovie.Title != movie.Title)
                    {
                        newMovie.Title = movie.Title;
                    }
                    if (newMovie.Overview != movie.Overview)
                    {
                        newMovie.Overview = movie.Overview;
                    }
                    if (newMovie.RunTime != movie.RunTime)
                    {
                        newMovie.RunTime = movie.RunTime;
                    }
                    if (newMovie.ReleaseDate != movie.ReleaseDate)
                    {
                        // Set time Kind to UTC --> Npgsql throws error otherwise
                        newMovie.ReleaseDate = movie.ReleaseDate.ToUniversalTime();
                    }
                    if (newMovie.Rating != movie.Rating)
                    {
                        newMovie.Rating = movie.Rating;
                    }
                    if (newMovie.VoteAverage != movie.VoteAverage)
                    {
                        newMovie.VoteAverage = movie.VoteAverage;
                    }
                    if (newMovie.Genres != movie.Genres)
                    {
                        newMovie.Genres = movie.Genres;
                    }
                    if (newMovie.Country != movie.Country)
                    {
                        newMovie.Country = movie.Country;
                    }
                    if (newMovie.TrailerUrl != movie.TrailerUrl)
                    {
                        newMovie.TrailerUrl = movie.TrailerUrl;
                    }

                    _context.Update(newMovie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Movies", new { id = movie.Id, local = true });
            }

            ViewData["CollectionId"] = new SelectList(_context.Collection.AsNoTracking().ToList(), "Id", "Name");

            return View(movie);
        }

        // GET: Movies/Delete/5
        [Authorize(Roles = "Administrator, User")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movie.FindAsync(id);
            _context.Movie.Remove(movie);
            await _context.SaveChangesAsync();
            return RedirectToAction("Library", "Movies");
        }

        // GET: Movies/Import
        [Authorize(Roles = "Administrator, User")]
        public async Task<IActionResult> Import()
        {
            var movies = await _context.Movie
                .AsNoTracking()
                .ToListAsync();
            return View(movies);
        }

        // GET: Movies/Import/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(int id)
        {
            // If we already have this movie we can just warn the user instead of importing it again
            if (_context.Movie.Any(m => m.MovieId == id))
            {
                var localMovie = await _context.Movie
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.MovieId == id);
                return RedirectToAction("Details", "Movies", new { id = localMovie.Id, local = true });
            }

            // Step 1: Get the raw data form the API
            var movieDetail = await _tmdbMovieService.MovieDetailAsync(id);

            // Step 2: Run the data through a mapping procedure
            var movie = await _tmdbMappingService.MapMovieDetailAsync(movieDetail);

            // Step 3: Add the new movie
            _context.Add(movie);
            await _context.SaveChangesAsync();

            // Step 4: Assign it to the default All collection
            await AddToMovieCollection(movie.Id, _appSettings.MovieProSettings.DefaultCollection.Name);

            return RedirectToAction("Import");
        }

        // GET: Movies/Library
        [Authorize(Roles = "Administrator, User")]
        public async Task<IActionResult> Library(int? collectionId, int? page)
        {
            ViewData["CollectionsId"] = _context.Collection
                .Where(c => c.Name.ToUpper() != "ALL")
                .OrderBy(c => c.Id)
                .AsNoTracking()
                .ToList();
            
            var output = new List<IPagedList<MovieCollection>>();

            foreach (Collection collection in ViewBag.CollectionsId)
            {
                var pageNumber = 1;
                var pageSize = 6;

                if (collection.Id == collectionId)
                {
                    _libraryPages[collection.Id] = (int)page;
                    // Set navigation property for html script
                    ViewData["CollectionSelected"] = $"collection-{collection.Id}";
                }

                if (_libraryPages.TryGetValue(collection.Id, out pageNumber) == false)
                {
                    pageNumber = 1;
                }

                var movieCollection = _context.MovieCollection
                    .Include(m => m.Movie)
                    .Include(c => c.Collection)
                    .Where(c => c.CollectionId == collection.Id)
                    .OrderBy(c => c.Order)
                    .AsNoTracking()
                    .ToPagedList(pageNumber, pageSize);

                output.Add(movieCollection);
            }

            return View(output);
        }

        private bool MovieExists(int id)
        {
            return _context.Movie.Any(e => e.Id == id);
        }

        private async Task AddToMovieCollection(int movieId, string collectionName)
        {
            var collection = await _context.Collection
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Name == collectionName);
            
            _context.Add(
                new MovieCollection()
                {
                    MovieId = movieId,
                    CollectionId = collection.Id
                });
            await _context.SaveChangesAsync();
        }

        private async Task AddToMovieCollection(int movieId, int collectionId)
        {
            _context.Add(
                new MovieCollection()
                {
                    MovieId = movieId,
                    CollectionId = collectionId
                });
            await _context.SaveChangesAsync();
        }
    }
}
