namespace SupplyStuff.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class orderinstructions : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "Instructions", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "Instructions");
        }
    }
}
