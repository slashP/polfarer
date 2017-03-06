using System.Collections.Generic;
using System.Linq;
using Polfarer.Models;

namespace Polfarer.Controllers
{
    public class PolViewModel
    {
        public IEnumerable<string> Types { get; set; }

        public IEnumerable<IGrouping<string, BeerLocation>> GroupedBeers { get; set; }

        public string SearchTerm { get; set; }
    }
}