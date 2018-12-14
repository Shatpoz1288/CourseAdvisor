namespace Test1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ageFieldAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Students", "age", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Students", "age");
        }
    }
}
