using System.Collections.Generic;
using System.Threading.Tasks;
using Polfarer.Dto;

namespace Polfarer.Services
{
    public interface IBeerService
    {
        Task<int> SaveBeers(IEnumerable<PolProduct> products);
    }
}