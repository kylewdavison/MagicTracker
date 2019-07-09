namespace MagicTracker.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changs : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Card");
            AlterColumn("dbo.Card", "CardId", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Card", "CardId");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.Card");
            AlterColumn("dbo.Card", "CardId", c => c.Guid(nullable: false));
            AddPrimaryKey("dbo.Card", "CardId");
        }
    }
}
