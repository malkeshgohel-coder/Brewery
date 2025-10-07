using Brewery.Models;
using Brewery.Repositories.Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Brewery.Repositories.Repositories
{
    public class OpenBreweryRepository : IOpenBreweryRepository
    {
        private readonly HttpClient _http;
        public OpenBreweryRepository(HttpClient http)
        {
            _http = http;
        }

        // fetch multiple pages (openbrewerydb paginates)
        public async Task<IEnumerable<OpenBreweryModel>> GetAllBreweriesAsync(CancellationToken cancellationToken = default)
        {
            var results = new List<OpenBreweryModel>();
            int page = 1;
            const int perPage = 50;

            try
            {
                while (true)
                {
                    var url = $"https://api.openbrewerydb.org/v1/breweries?page={page}&per_page={perPage}";

                     var res = await _http.GetFromJsonAsync<List<OpenBreweryModel>>($"breweries?page={page}&per_page={perPage}", cancellationToken);

                    if (res == null || res.Count == 0)
                        break;

                    results.AddRange(res);

                    // defensive: don't loop forever
                    if (res.Count < perPage)
                        break;

                    page++;
                }

                return results;
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<IEnumerable<OpenBreweryModel>> SearchBreweriesAsync(string query, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Enumerable.Empty<OpenBreweryModel>();

            // openbrewery has /breweries/search?query=
            var encoded = Uri.EscapeDataString(query);

            var res = await _http.GetFromJsonAsync<List<OpenBreweryModel>>($"/breweries/search?query={encoded}", cancellationToken);

            return res ?? Enumerable.Empty<OpenBreweryModel>();
        }
    }
}
