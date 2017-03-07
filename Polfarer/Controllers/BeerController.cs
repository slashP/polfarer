using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Polfarer.Models;

namespace Polfarer.Controllers
{
    public class BeerController : Controller
    {
        [Route("beers")]
        public async Task<ActionResult> Beers(string query = null)
        {
            using (var db = new ApplicationDbContext())
            {
                var beers =
                    await
                        db.WatchedBeers.Where(x => x.Type.Contains(query)).Include(x => x.BeerLocations).ToListAsync();
                return View(beers);
            }
        }

        [Route("allBeers")]
        public async Task<ActionResult> AllBeers()
        {
            using (var db = new ApplicationDbContext())
            {
                var beers = await db.Beers.OrderByDescending(x => x.Alcohol).ToListAsync();
                return View(beers);
            }
        }

        [Route("")]
        public async Task<ActionResult> Pol(string query = "Stout")
        {
            using (var db = new ApplicationDbContext())
            {
                var groupedBeers =
                    (await db.BeerLocations.Include(x => x.WatchedBeer)
                        .Where(x => x.WatchedBeer.Name.Contains(query) || x.WatchedBeer.Type.Contains(query))
                        .ToListAsync())
                    .GroupBy(x => x.Name)
                    .OrderByDescending(x => x.Count())
                    .ToList();
                var types = await db.WatchedBeers.Select(x => x.Type).Distinct().ToListAsync();
                return View(new PolViewModel
                {
                    GroupedBeers = groupedBeers,
                    Types = types,
                    SearchTerm = query
                });
            }
        }
    }
}