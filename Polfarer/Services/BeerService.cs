using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Polfarer.Dto;
using Polfarer.Models;

namespace Polfarer.Services
{
    public class BeerService : IBeerService
    {
        public async Task<int> SaveBeers(IEnumerable<PolProduct> products)
        {
            using (var db = new ApplicationDbContext())
            {
                var existingBeers = new HashSet<string>(await db.Beers.Select(x => x.MaterialNumber).ToListAsync());
                var insertProducts = products.Where(x => !existingBeers.Contains(x.Varenummer)).ToList();
                var beers = insertProducts.Select(CreateBeer).Where(x => x.BeerCategory != BeerCategory.Unknown).ToList();
                db.Beers.AddRange(beers);
                return await db.SaveChangesAsync();
            }
        }

        private static Beer CreateBeer(PolProduct x)
        {
            decimal alcohol;
            return new Beer
            {
                MaterialNumber = x.Varenummer,
                Alcohol = decimal.TryParse(x.Alkohol, NumberStyles.Any, CultureInfo.InvariantCulture, out alcohol) ? alcohol / 100 : 0,
                Bitterhet = x.Bitterhet,
                Brewery = x.Produsent,
                Country = x.Land,
                DateAdded = DateTime.UtcNow.Date,
                Friskhet = x.Friskhet,
                Fylde = x.Fylde,
                Name = x.Varenavn,
                Price = x.Pris,
                Smell = x.Lukt,
                Sweetness = x.Sodme,
                Taste = x.Smak,
                Volume = x.Volum,
                BeerCategory = Category(x.Varetype)
            };
        }

        private static BeerCategory Category(string vareType)
        {
            switch (vareType)
            {
                case "Porter & stout":
                    return BeerCategory.PorterStout;
                case "Surøl":
                    return BeerCategory.Sour;
                case "Barley wine":
                    return BeerCategory.BarleyWine;
                case "Lys ale":
                    return BeerCategory.LightAle;
                case "Lys lager":
                    return BeerCategory.LightLager;
                case "Mørk lager":
                    return BeerCategory.DarkLager;
                case "India pale ale":
                    return BeerCategory.Ipa;
                case "Saison farmhouse ale":
                    return BeerCategory.Saison;
                case "Spesial":
                    return BeerCategory.Special;
                case "Pale ale":
                    return BeerCategory.PaleAle;
                case "Hveteøl":
                    return BeerCategory.Wheat;
                case "Klosterstil":
                    return BeerCategory.Trapist;
                case "Scotch ale":
                    return BeerCategory.ScotchAle;
                case "Red/amber":
                    return BeerCategory.Amber;
                case "Brown ale":
                    return BeerCategory.BrownAle;
                default:
                    return BeerCategory.Unknown;
            }
        }
    }
}