namespace MagicTracker.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deckId : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Card", "Deck_DeckId", "dbo.Deck");
            DropIndex("dbo.Card", new[] { "Deck_DeckId" });
            RenameColumn(table: "dbo.Card", name: "Deck_DeckId", newName: "DeckId");
            AlterColumn("dbo.Card", "DeckId", c => c.Int(nullable: false));
            CreateIndex("dbo.Card", "DeckId");
            AddForeignKey("dbo.Card", "DeckId", "dbo.Deck", "DeckId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Card", "DeckId", "dbo.Deck");
            DropIndex("dbo.Card", new[] { "DeckId" });
            AlterColumn("dbo.Card", "DeckId", c => c.Int());
            RenameColumn(table: "dbo.Card", name: "DeckId", newName: "Deck_DeckId");
            CreateIndex("dbo.Card", "Deck_DeckId");
            AddForeignKey("dbo.Card", "Deck_DeckId", "dbo.Deck", "DeckId");
        }
    }
}
