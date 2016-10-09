namespace LeagueSeason5CountersService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdUnitInuse : DbMigration
    {
        public override void Up()
        {
            AddColumn("LeagueSeason5Counters.AdUnits", "InUse", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("LeagueSeason5Counters.AdUnits", "InUse");
        }
    }
}
