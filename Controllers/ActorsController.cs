using Microsoft.AspNetCore.Mvc;
using MovieProMVC.Services.Interfaces;

namespace MovieProMVC.Controllers
{
    public class ActorsController : Controller
    {
        public readonly IRemoteMovieService _tmdbMovieService;
        public readonly IDataMappingService _mappingService;

        public ActorsController(IRemoteMovieService tmdbMovieService, IDataMappingService mappingService)
        {
            _tmdbMovieService = tmdbMovieService;
            _mappingService = mappingService;
        }

        // GET: Actors/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var actor = await _tmdbMovieService.ActorDetailAsync(id);
            actor = _mappingService.MapActorDetail(actor);

            return View(actor);
        }
    }
}
