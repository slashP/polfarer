using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using CsvHelper;
using Newtonsoft.Json;
using Polfarer.Dto;
using Polfarer.Models;
using Polfarer.Services;

namespace Polfarer.Controllers
{
    public class AdminController : Controller
    {
        private readonly IBeerService _beerService;
        private static readonly Uri BaseAddress = new Uri("https://www.vinmonopolet.no");

        public AdminController()
        {
            _beerService = new BeerService();
        }

        [Route("admin/allBeersFromCsv")]
        [HttpPost]
        public async Task<string> Update()
        {
            var httpClient = new HttpClient
            {
                BaseAddress = BaseAddress
            };
            var polProducts = await PolProducts(httpClient);
            var numberOfInserts = await _beerService.SaveBeers(polProducts);
            return $"Inserted {numberOfInserts} beers.";
        }

        [Route("admin/fetch")]
        [HttpPost]
        public async Task<string> Beers(string searchTerm, decimal alcoholLevel)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return $"Provide a {nameof(searchTerm)}";
            }

            if (alcoholLevel < 2)
            {
                return $"{nameof(alcoholLevel)} missing or too low.";
            }

            var minimumAlcohol = alcoholLevel < 100 ? alcoholLevel * 100m : alcoholLevel;
            var cookies = new CookieContainer();
            var handler = new HttpClientHandler { CookieContainer = cookies };

            cookies.Add(BaseAddress, new Cookie("vmpSite-customerLocation", @""" | 59.913438, 10.742932"""));
            var client = new HttpClient(handler) { BaseAddress = BaseAddress };
            var records = await PolProducts(client);
            decimal alc;
            var interestingBeers =
                records.Where(
                    x =>
                        decimal.TryParse(x.Alkohol, NumberStyles.Any, CultureInfo.InvariantCulture, out alc) &&
                        alc >= minimumAlcohol && x.Varetype.ToLower().Contains(searchTerm.ToLower())).ToList();
            using (var db = new ApplicationDbContext())
            {
                foreach (var interestingBeer in interestingBeers)
                {
                    try
                    {
                        var stockJson =
                            await client.GetStringAsync(
                                $"vmpSite/store-pickup/{interestingBeer.Varenummer}/pointOfServices?cartPage=false&entryNumber=0");
                        var stockStatus = JsonConvert.DeserializeObject<StockStatus>(stockJson);
                        var watchedBeer = new WatchedBeer
                        {
                            Name = interestingBeer.Varenavn,
                            AlcoholPercentage = decimal.Parse(interestingBeer.Alkohol, CultureInfo.InvariantCulture) / 100,
                            Price = interestingBeer.Pris,
                            Type = interestingBeer.Varetype,
                            BeerLocations =
                                stockStatus.data.Select(x => new BeerLocation
                                {
                                    Name = x.displayName,
                                    Distance = decimal.Parse(x.formattedDistance.Split(' ').First(), CultureInfo.InvariantCulture),
                                    StockLevel = int.Parse(x.stockLevel)
                                }).Where(x => x.Distance < 7 && x.StockLevel > 0).ToList()
                        };
                        if (watchedBeer.BeerLocations.Any())
                        {
                            db.WatchedBeers.Add(watchedBeer);
                        }
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError($"Couldn't fetch beer info from vinmonopolet.no. {Environment.NewLine}{e}");
                    }
                }
                try
                {
                    await db.Database.ExecuteSqlCommandAsync(
                        $"DELETE FROM [BeerLocations] WHERE [{nameof(BeerLocation.WatchedBeerId)}] IN (SELECT [Id] FROM [WatchedBeers] WHERE [Type] LIKE '%' + @p0 + '%')", searchTerm);
                    await db.Database.ExecuteSqlCommandAsync($"DELETE FROM [WatchedBeers] WHERE [{nameof(WatchedBeer.Type)}] LIKE '%' + @p0 + '%'", searchTerm);
                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    Trace.TraceError($"Couldn't save beer info. {Environment.NewLine}{e}");
                    return "Failed saving to db. " + e;
                }
            }

            return "All good.";
        }

        private static async Task<List<PolProduct>> PolProducts(HttpClient client)
        {
            try
            {
                var productsString =
                    await
                        client.GetAsync(
                            "medias/sys_master/products/products/hbc/hb0/8834253127710/produkter.csv");
                var csv =
                    new CsvReader(new StringReader(Encoding.Default.GetString(await productsString.Content.ReadAsByteArrayAsync())));
                csv.Configuration.Delimiter = ";";
                csv.Configuration.Encoding = Encoding.Default;
                csv.Configuration.CultureInfo = new CultureInfo("nb-NO");
                var records = csv.GetRecords<PolProduct>().ToList();
                return records;
            }
            catch (Exception e)
            {
                Trace.TraceError($"Couldn't fetch CSV from vinmonopolet.no. {Environment.NewLine}{e}");
                throw;
            }
        }
    }
}