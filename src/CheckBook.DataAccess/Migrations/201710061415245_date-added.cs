namespace CheckBook.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dateadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VoteSessions", "Date", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.VoteSessions", "Date");
        }
    }
}
