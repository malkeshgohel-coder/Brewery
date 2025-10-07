using Brewery.Services.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Brewery.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    //[Route("api/[controller]")]
    public class BreweryController : ControllerBase
    {
        private readonly IBreweryService _breweryService;
        private readonly ILogger<BreweryController> _logger;
        public BreweryController(IBreweryService breweryService, ILogger<BreweryController> logger)
        {
            _breweryService = breweryService;
            _logger = logger;
        }

        /// <summary>
        /// Get breweries. Supports search, sorting and distance calculation.
        /// Query examples:
        /// GET /api/v1/breweries?search=dog&sortBy=name
        /// GET /api/v1/breweries?lat=37.77&lon=-122.42&sortBy=distance
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? search, [FromQuery] string? sortBy, [FromQuery] double? lat, [FromQuery] double? lon, CancellationToken cancellationToken = default)
        {
            var list = await _breweryService.GetBreweriesAsync(search, sortBy, lat, lon, cancellationToken);
            return Ok(list);
        }

        /// <summary>
        /// Autocomplete brewery names
        /// GET /api/v1/breweries/autocomplete?q=dog
        /// </summary>
        [HttpGet("autocomplete")]
        public async Task<IActionResult> Autocomplete([FromQuery] string query, [FromQuery] int limit = 10)
        {
            if (string.IsNullOrWhiteSpace(query)) 
                return BadRequest("query is required");

            var results = await _breweryService.AutocompleteAsync(query, limit);
            return Ok(results);
        }

    }
}
