namespace Polfarer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AllBeersTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Beers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MaterialNumber = c.String(maxLength: 32),
                        Name = c.String(),
                        BeerCategory = c.Int(nullable: false),
                        Alcohol = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateAdded = c.DateTime(nullable: false),
                        Fylde = c.Int(nullable: false),
                        Friskhet = c.Int(nullable: false),
                        Bitterhet = c.Int(nullable: false),
                        Sweetness = c.Int(nullable: false),
                        Volume = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Smell = c.String(),
                        Taste = c.String(),
                        Country = c.String(),
                        Brewery = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.MaterialNumber, unique: true, name: "IDX_MaterialNumber");
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Beers", "IDX_MaterialNumber");
            DropTable("dbo.Beers");
        }
    }
}
