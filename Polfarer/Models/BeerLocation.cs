namespace Polfarer.Models
{
    public class BeerLocation
    {
        public int Id { get; set; }

        public int WatchedBeerId { get; set; }

        public virtual WatchedBeer WatchedBeer { get; set; }

        public int StockLevel { get; set; }

        public string Name { get; set; }

        public decimal Distance { get; set; }
    }
}