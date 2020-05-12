namespace SupplyStuff.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Name", c => c.String());
            AddColumn("dbo.AspNetUsers", "Reference", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Reference");
            DropColumn("dbo.AspNetUsers", "Name");
        }
    }
}
