namespace MagicTracker.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ApiAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CardApi",
                c => new
                    {
                        CardApiId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ManaCost = c.String(),
                        Type = c.String(),
                        Text = c.String(),
                    })
                .PrimaryKey(t => t.CardApiId);
            
            AddColumn("dbo.Card", "CardApiId", c => c.Int());
            CreateIndex("dbo.Card", "CardApiId");
            AddForeignKey("dbo.Card", "CardApiId", "dbo.CardApi", "CardApiId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Card", "CardApiId", "dbo.CardApi");
            DropIndex("dbo.Card", new[] { "CardApiId" });
            DropColumn("dbo.Card", "CardApiId");
            DropTable("dbo.CardApi");
        }
    }
}
