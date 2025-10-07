using Brewery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brewery.Repositories.Repositories.IRepositories
{
    public interface IOpenBreweryRepository
    {
        Task<IEnumerable<OpenBreweryModel>> GetAllBreweriesAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<OpenBreweryModel>> SearchBreweriesAsync(string query, CancellationToken cancellationToken = default);
    }
}
