namespace LeagueSeason5CountersService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdUnitRemoveIdName : DbMigration
    {
        public override void Up()
        {
            AddColumn("LeagueSeason5Counters.AdUnits", "Ad", c => c.Int(nullable: false));
            AddColumn("LeagueSeason5Counters.AdUnits", "App", c => c.String());
            DropColumn("LeagueSeason5Counters.AdUnits", "AdId");
            DropColumn("LeagueSeason5Counters.AdUnits", "AppId");
        }
        
        public override void Down()
        {
            AddColumn("LeagueSeason5Counters.AdUnits", "AppId", c => c.String());
            AddColumn("LeagueSeason5Counters.AdUnits", "AdId", c => c.Int(nullable: false));
            DropColumn("LeagueSeason5Counters.AdUnits", "App");
            DropColumn("LeagueSeason5Counters.AdUnits", "Ad");
        }
    }
}
