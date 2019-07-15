namespace MagicTracker.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Test : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Deck",
                c => new
                    {
                        DeckId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        OwnerId = c.Guid(nullable: false),
                        CardListString = c.String(),
                    })
                .PrimaryKey(t => t.DeckId);
            
            AddColumn("dbo.Card", "Deck_DeckId", c => c.Int());
            CreateIndex("dbo.Card", "Deck_DeckId");
            AddForeignKey("dbo.Card", "Deck_DeckId", "dbo.Deck", "DeckId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Card", "Deck_DeckId", "dbo.Deck");
            DropIndex("dbo.Card", new[] { "Deck_DeckId" });
            DropColumn("dbo.Card", "Deck_DeckId");
            DropTable("dbo.Deck");
        }
    }
}
