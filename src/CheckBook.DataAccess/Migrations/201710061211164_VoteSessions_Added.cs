namespace CheckBook.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoteSessions_Added : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.VoteSessions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.UserGroups", "VoteSession_Id", c => c.Int());
            CreateIndex("dbo.UserGroups", "VoteSession_Id");
            AddForeignKey("dbo.UserGroups", "VoteSession_Id", "dbo.VoteSessions", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserGroups", "VoteSession_Id", "dbo.VoteSessions");
            DropIndex("dbo.UserGroups", new[] { "VoteSession_Id" });
            DropColumn("dbo.UserGroups", "VoteSession_Id");
            DropTable("dbo.VoteSessions");
        }
    }
}
