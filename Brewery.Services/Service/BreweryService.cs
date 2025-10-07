using Brewery.DTOs;
using Brewery.Models;
using Brewery.Repositories.Repositories;
using Brewery.Repositories.Repositories.IRepositories;
using Brewery.Services.Service.IService;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brewery.Services.Service
{
    public class BreweryService : IBreweryService
    {
        private const string CacheKey = "OpenBrewery_All";
        private readonly IBreweryRepository _breweryRepository;
        private readonly IOpenBreweryRepository _openBreweryRepository;
        private readonly IMemoryCache _cache;
        private readonly ILogger<BreweryService> _logger;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(10);

        public BreweryService(IBreweryRepository breweryRepository,IOpenBreweryRepository openBreweryRepository, IMemoryCache cache, ILogger<BreweryService> logger)
        {
            _breweryRepository = breweryRepository;
            _openBreweryRepository = openBreweryRepository;
            _cache = cache;
            _logger = logger;
        }

        // public entry point
        public async Task<IEnumerable<BreweryDTO>> GetBreweriesAsync(string? search = null, string? sortBy = null, double? lat = null, double? lon = null, CancellationToken cancellationToken = default)
        {
            var cached = await GetOrRefreshCacheAsync(cancellationToken);

            // search (case-insensitive contains)
            IEnumerable<BreweryDTO> working = cached;
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLowerInvariant();
                working = working.Where(b => (b.Name ?? "").ToLowerInvariant().Contains(s)
                                          || (b.City ?? "").ToLowerInvariant().Contains(s));
            }

            // compute distances if lat/lon provided
            if (lat.HasValue && lon.HasValue)
            {
                foreach (var b in working)
                {
                    if (b.Latitude.HasValue && b.Longitude.HasValue)
                        b.DistanceInKm = HaversineDistance(lat.Value, lon.Value, b.Latitude.Value, b.Longitude.Value);
                    else
                        b.DistanceInKm = null;
                }
            }

            // sorting: name, city, distance
            working = (sortBy ?? "").ToLowerInvariant() switch
            {
                "name" => working.OrderBy(b => b.Name),
                "city" => working.OrderBy(b => b.City),
                "distance" => working.OrderBy(b => b.DistanceInKm ?? double.MaxValue),
                _ => working.OrderBy(b => b.Name)
            };

            return working;
        }

        public async Task<IEnumerable<string>> AutocompleteAsync(string query, int limit = 10)
        {
            if (string.IsNullOrWhiteSpace(query)) return Enumerable.Empty<string>();
            var cached = await GetOrRefreshCacheAsync();
            var low = query.Trim().ToLowerInvariant();
            return cached
                .Select(b => b.Name ?? "")
                .Where(n => n.ToLowerInvariant().StartsWith(low))
                .Distinct()
                .Take(limit);
        }


        private async Task<IEnumerable<BreweryDTO>> GetOrRefreshCacheAsync(CancellationToken cancellationToken = default)
        {
            if (_cache.TryGetValue<IEnumerable<BreweryDTO>>(CacheKey, out var cached))
            {
                _logger.LogDebug("Returning breweries from cache");
                return cached;
            }

            _logger.LogInformation("Cache miss - fetching from external API");
            // fetch external source
            var external = await _openBreweryRepository.GetAllBreweriesAsync(cancellationToken);
            // map
            var mapped = external.Select(MapToDto).Where(d => !string.IsNullOrEmpty(d.Name)).ToList();

            // store in repository (in-memory)
            await _breweryRepository.SetBreweriesAsync(mapped);

            // set cache
            _cache.Set(CacheKey, mapped, _cacheDuration);
            return mapped;
        }

        private BreweryDTO MapToDto(OpenBreweryModel s)
        {
            return new BreweryDTO
            {
                Id = s.id,
                Name = s.name,
                City = s.city,
                Phone = s.phone,
                Latitude = s.latitude,
                Longitude = s.longitude
            };
        }

        // Haversine formula: returns kilometers
        private static double HaversineDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double R = 6371; // km
            var dLat = ToRad(lat2 - lat1);
            var dLon = ToRad(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private static double ToRad(double deg) => deg * Math.PI / 180.0;
    }
}
