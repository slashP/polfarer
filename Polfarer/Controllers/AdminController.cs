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

namespace Polfarer.Controllers
{
    public class AdminController : Controller
    {
        private const int MinimumAlcohol = 900;

        [Route("admin/fetch")]
        [HttpPost]
        public async Task<string> Beers()
        {
            var cookies = new CookieContainer();
            var handler = new HttpClientHandler { CookieContainer = cookies };

            var baseAddress = new Uri("https://www.vinmonopolet.no");
            cookies.Add(baseAddress, new Cookie("vmpSite-customerLocation", @""" | 59.913438, 10.742932"""));
            var client = new HttpClient(handler) { BaseAddress = baseAddress };
            var records = await PolProducts(client);
            decimal alc;
            var interestingStouts =
                records.Where(
                    x =>
                        decimal.TryParse(x.Alkohol, NumberStyles.Any, CultureInfo.InvariantCulture, out alc) &&
                        alc >= MinimumAlcohol && x.Varetype.ToLower().Contains("stout")).ToList();
            using (var db = new ApplicationDbContext())
            {
                foreach (var interestingStout in interestingStouts)
                {
                    try
                    {
                        var stockJson =
                            client.GetStringAsync(
                                $"vmpSite/store-pickup/{interestingStout.Varenummer}/pointOfServices?cartPage=false&entryNumber=0")
                                .Result;
                        var stockStatus = JsonConvert.DeserializeObject<StockStatus>(stockJson);
                        var watchedBeer = new WatchedBeer
                        {
                            Name = interestingStout.Varenavn,
                            AlcoholPercentage = decimal.Parse(interestingStout.Alkohol, CultureInfo.InvariantCulture) / 100,
                            Price = interestingStout.Pris,
                            Type = interestingStout.Varetype,
                            BeerLocations =
                                stockStatus.data.Where(x => int.Parse(x.stockLevel) > 0).Take(4).Select(x => new BeerLocation
                                {
                                    Name = x.displayName,
                                    Distance = decimal.Parse(x.formattedDistance.Split(' ').First(), CultureInfo.InvariantCulture),
                                    StockLevel = int.Parse(x.stockLevel)
                                }).ToList()
                        };
                        if (watchedBeer.BeerLocations.Any(x => x.Distance < 10 && x.StockLevel > 0))
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
                    await db.Database.ExecuteSqlCommandAsync("DELETE FROM [BeerLocations]");
                    await db.Database.ExecuteSqlCommandAsync("DELETE FROM [WatchedBeers]");
                    await db.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    Trace.TraceError($"Couldn't save beer info. {Environment.NewLine}{e}");
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