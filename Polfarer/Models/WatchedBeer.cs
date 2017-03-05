using System.Collections.Generic;

namespace Polfarer.Models
{
    public class WatchedBeer
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal AlcoholPercentage { get; set; }

        public string Type { get; set; }

        public decimal Price { get; set; }

        public virtual ICollection<BeerLocation> BeerLocations { get; set; }
    }
}