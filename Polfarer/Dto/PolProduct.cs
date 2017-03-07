namespace Polfarer.Dto
{
    public class PolProduct
    {
        public string Varenummer { get; set; }

        public string Varenavn { get; set; }

        public decimal Pris { get; set; }

        public decimal Volum { get; set; }

        public string Varetype { get; set; }

        public string Alkohol { get; set; }

        public string Produsent { get; set; }

        public string Lukt { get; set; }

        public string Smak { get; set; }

        public string Land { get; set; }

        public int Fylde { get; set; }

        public int Friskhet { get; set; }

        public int Bitterhet { get; set; }

        public int Sodme { get; set; }
    }
}