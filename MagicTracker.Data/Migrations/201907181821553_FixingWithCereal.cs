namespace MagicTracker.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixingWithCereal : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CardApi", "Colors", c => c.String());
            AddColumn("dbo.CardApi", "Subtypes", c => c.String());
            AddColumn("dbo.CardApi", "Printings", c => c.String());
            AddColumn("dbo.CardApi", "MultiSetDict", c => c.String());
            AddColumn("dbo.CardApi", "SetNameDict", c => c.String());
            AddColumn("dbo.Deck", "ListOfCards", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Deck", "ListOfCards");
            DropColumn("dbo.CardApi", "SetNameDict");
            DropColumn("dbo.CardApi", "MultiSetDict");
            DropColumn("dbo.CardApi", "Printings");
            DropColumn("dbo.CardApi", "Subtypes");
            DropColumn("dbo.CardApi", "Colors");
        }
    }
}
