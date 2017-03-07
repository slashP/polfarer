using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Polfarer.Models
{
    public class Beer
    {
        [Key]
        public int Id { get; set; }

        [Index("IDX_MaterialNumber", 1, IsUnique = true)]
        [MaxLength(32)]
        public string MaterialNumber { get; set; }

        public string Name { get; set; }

        public BeerCategory BeerCategory { get; set; }

        public decimal Alcohol { get; set; }

        public DateTime DateAdded { get; set; }

        public int Fylde { get; set; }

        public int Friskhet { get; set; }

        public int Bitterhet { get; set; }

        public int Sweetness { get; set; }

        public decimal Volume { get; set; }

        public decimal Price { get; set; }

        public string Smell { get; set; }

        public string Taste { get; set; }

        public string Country { get; set; }

        public string Brewery { get; set; }
    }
}