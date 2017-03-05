namespace Polfarer.Dto
{
    public class StockStatus
    {
        public Datum[] data { get; set; }
    }

    public class Datum
    {
        public string name { get; set; }
        public string displayName { get; set; }
        public string town { get; set; }
        public string line1 { get; set; }
        public string line2 { get; set; }
        public string country { get; set; }
        public string postalCode { get; set; }
        public string formattedDistance { get; set; }
        public string url { get; set; }
        public string stockPickup { get; set; }
        public string productcode { get; set; }
        public string storeLatitude { get; set; }
        public string storeLongitude { get; set; }
        public string stockLevel { get; set; }
    }
}