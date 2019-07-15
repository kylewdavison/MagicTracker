namespace MagicTracker.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _3rdMigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Card", "Deck_DeckId", c => c.Int());
            CreateIndex("dbo.Card", "Deck_DeckId");
            AddForeignKey("dbo.Card", "Deck_DeckId", "dbo.Deck", "DeckId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Card", "Deck_DeckId", "dbo.Deck");
            DropIndex("dbo.Card", new[] { "Deck_DeckId" });
            DropColumn("dbo.Card", "Deck_DeckId");
        }
    }
}
