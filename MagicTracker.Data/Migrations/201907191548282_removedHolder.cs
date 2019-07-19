namespace MagicTracker.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removedHolder : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Card", "Holder");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Card", "Holder", c => c.Int(nullable: false));
        }
    }
}
