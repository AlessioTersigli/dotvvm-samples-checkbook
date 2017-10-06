namespace CheckBook.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Restaurant_Added : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserGroups", "VoteSession_Id", "dbo.VoteSessions");
            DropIndex("dbo.UserGroups", new[] { "VoteSession_Id" });
            CreateTable(
                "dbo.Restaurants",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.VoteSessions", "RestaurantId", c => c.Int(nullable: false));
            AddColumn("dbo.VoteSessions", "UserId", c => c.Int(nullable: false));
            AddColumn("dbo.VoteSessions", "UserGroup_Id", c => c.Int());
            CreateIndex("dbo.VoteSessions", "RestaurantId");
            CreateIndex("dbo.VoteSessions", "UserGroup_Id");
            AddForeignKey("dbo.VoteSessions", "RestaurantId", "dbo.Restaurants", "Id", cascadeDelete: true);
            AddForeignKey("dbo.VoteSessions", "UserGroup_Id", "dbo.UserGroups", "Id");
            DropColumn("dbo.UserGroups", "VoteSession_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserGroups", "VoteSession_Id", c => c.Int());
            DropForeignKey("dbo.VoteSessions", "UserGroup_Id", "dbo.UserGroups");
            DropForeignKey("dbo.VoteSessions", "RestaurantId", "dbo.Restaurants");
            DropIndex("dbo.VoteSessions", new[] { "UserGroup_Id" });
            DropIndex("dbo.VoteSessions", new[] { "RestaurantId" });
            DropColumn("dbo.VoteSessions", "UserGroup_Id");
            DropColumn("dbo.VoteSessions", "UserId");
            DropColumn("dbo.VoteSessions", "RestaurantId");
            DropTable("dbo.Restaurants");
            CreateIndex("dbo.UserGroups", "VoteSession_Id");
            AddForeignKey("dbo.UserGroups", "VoteSession_Id", "dbo.VoteSessions", "Id");
        }
    }
}
