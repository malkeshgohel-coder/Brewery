using Brewery.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brewery.Services.Service.IService
{
    public interface IBreweryService
    {
        Task<IEnumerable<BreweryDTO>> GetBreweriesAsync(string? search = null, string? sortBy = null, double? lat = null, double? lon = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<string>> AutocompleteAsync(string query, int limit = 10);
    }
}
