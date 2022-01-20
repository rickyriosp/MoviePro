using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieProMVC.Data;
using MovieProMVC.Models;
using MovieProMVC.Models.ViewModels;
using MovieProMVC.Services.Interfaces;
using System.Diagnostics;

namespace MovieProMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IRemoteMovieService _tmdbMovieService;
        private readonly IMovieEmailSender _emailSender;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IRemoteMovieService tmdbMovieService, IMovieEmailSender emailSender)
        {
            _logger = logger;
            _context = context;
            _tmdbMovieService = tmdbMovieService;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> Index()
        {
            const int count = 16;
            var data = new LandingPageVM()
            {
                CustomCollections = await _context.Collection
                    .Include(c => c.MovieCollections)
                    .ThenInclude(mc => mc.Movie)
                    .ToListAsync(),
                NowPlaying = await _tmdbMovieService.SearchMoviesAsync(Enums.MovieCategory.now_playing, count),
                Popular = await _tmdbMovieService.SearchMoviesAsync(Enums.MovieCategory.popular, count),
                TopRated = await _tmdbMovieService.SearchMoviesAsync(Enums.MovieCategory.top_rated, count),
                Upcoming = await _tmdbMovieService.SearchMoviesAsync(Enums.MovieCategory.upcoming, count),
                Trending = await _tmdbMovieService.SearchMoviesTrendingAsync(count),
                Genres = await _tmdbMovieService.GetAllMovieGenresAsync(),
                ActorsPopular = await _tmdbMovieService.SearchActorsAsync(count),
            };

            return View(data);
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactMe model)
        {
            if (ModelState.IsValid)
            {
                // This is where we send the email
                await _emailSender.SendContactEmailAsync(model.Name, model.Email, model.Subject, model.Message);

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("/Home/HandleError/{code}")]
        public IActionResult HandleError(int code)
        {
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            ViewData["ErrorMessage"] = $"Error occurred. The ErrorCode is: {code}";
            ViewData["Path"] = statusCodeResult.OriginalPath;
            ViewData["Query"] = statusCodeResult.OriginalQueryString;

            if (code == 404)
            {
                return View("404");
            }

            return StatusCode(code);
        }
    }
}