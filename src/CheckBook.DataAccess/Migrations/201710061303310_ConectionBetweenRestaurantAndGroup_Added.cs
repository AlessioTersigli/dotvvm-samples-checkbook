namespace CheckBook.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ConectionBetweenRestaurantAndGroup_Added : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.VoteSessions", "UserGroup_Id", "dbo.UserGroups");
            DropIndex("dbo.VoteSessions", new[] { "UserGroup_Id" });
            AddColumn("dbo.Restaurants", "GroupId", c => c.Int(nullable: false));
            CreateIndex("dbo.Restaurants", "GroupId");
            CreateIndex("dbo.VoteSessions", "UserId");
            AddForeignKey("dbo.Restaurants", "GroupId", "dbo.Groups", "Id", cascadeDelete: true);
            AddForeignKey("dbo.VoteSessions", "UserId", "dbo.Users", "Id", cascadeDelete: true);
            DropColumn("dbo.VoteSessions", "UserGroup_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.VoteSessions", "UserGroup_Id", c => c.Int());
            DropForeignKey("dbo.VoteSessions", "UserId", "dbo.Users");
            DropForeignKey("dbo.Restaurants", "GroupId", "dbo.Groups");
            DropIndex("dbo.VoteSessions", new[] { "UserId" });
            DropIndex("dbo.Restaurants", new[] { "GroupId" });
            DropColumn("dbo.Restaurants", "GroupId");
            CreateIndex("dbo.VoteSessions", "UserGroup_Id");
            AddForeignKey("dbo.VoteSessions", "UserGroup_Id", "dbo.UserGroups", "Id");
        }
    }
}
