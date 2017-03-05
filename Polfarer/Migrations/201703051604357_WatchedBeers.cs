namespace Polfarer.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class WatchedBeers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WatchedBeers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        AlcoholPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Type = c.String(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BeerLocations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        WatchedBeerId = c.Int(nullable: false),
                        StockLevel = c.Int(nullable: false),
                        Name = c.String(),
                        Distance = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WatchedBeers", t => t.WatchedBeerId)
                .Index(t => t.WatchedBeerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BeerLocations", "WatchedBeerId", "dbo.WatchedBeers");
            DropIndex("dbo.BeerLocations", new[] { "WatchedBeerId" });
            DropTable("dbo.BeerLocations");
            DropTable("dbo.WatchedBeers");
        }
    }
}
