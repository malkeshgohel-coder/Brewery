using Brewery.DTOs;
using Brewery.Repositories.Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brewery.Repositories.Repositories
{
    public class InMemoryBreweryRepository : IBreweryRepository
    {
        private IEnumerable<BreweryDTO> _breweries = Enumerable.Empty<BreweryDTO>();
        private readonly object _lock = new();

        public Task<IEnumerable<BreweryDTO>> GetBreweriesAsync()
        {
            // return copy to avoid external mutation
            IEnumerable<BreweryDTO> copy;
            lock (_lock)
            {
                copy = _breweries.Select(b => new BreweryDTO
                {
                    Id = b.Id,
                    Name = b.Name,
                    City = b.City,
                    Phone = b.Phone,
                    Latitude = b.Latitude,
                    Longitude = b.Longitude,
                    DistanceInKm = b.DistanceInKm
                }).ToList();
            }
            return Task.FromResult(copy);
        }

        public Task SetBreweriesAsync(IEnumerable<BreweryDTO> breweries)
        {
            lock (_lock)
            {
                _breweries = breweries.ToList();
            }
            return Task.CompletedTask;
        }
    }
}
