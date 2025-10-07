using Brewery.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brewery.Repositories.Repositories.IRepositories
{
    public interface IBreweryRepository
    {
        Task<IEnumerable<BreweryDTO>> GetBreweriesAsync();
        Task SetBreweriesAsync(IEnumerable<BreweryDTO> breweries);
    }
}
