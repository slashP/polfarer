using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;
using Polfarer.Models;

namespace Polfarer.Controllers
{
    public class BeerController : Controller
    {
        [Route("beers")]
        public async Task<ActionResult> Beers()
        {
            using (var db = new ApplicationDbContext())
            {
                var beers =
                    await
                        db.WatchedBeers.Include(x => x.BeerLocations).ToListAsync();
                return View(beers);
            }
        }
    }
}