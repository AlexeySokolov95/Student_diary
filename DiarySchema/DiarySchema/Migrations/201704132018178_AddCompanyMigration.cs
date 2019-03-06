namespace DiarySchema.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCompanyMigration : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Groups", name: "MonitorId", newName: "StudentId");
            RenameIndex(table: "dbo.Groups", name: "IX_MonitorId", newName: "IX_StudentId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Groups", name: "IX_StudentId", newName: "IX_MonitorId");
            RenameColumn(table: "dbo.Groups", name: "StudentId", newName: "MonitorId");
        }
    }
}
