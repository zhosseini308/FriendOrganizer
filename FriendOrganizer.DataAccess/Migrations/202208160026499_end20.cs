namespace FriendOrganizer.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class end20 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ProgrammingLanguage", "Name", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ProgrammingLanguage", "Name", c => c.String(nullable: false, maxLength: 50));
        }
    }
}
