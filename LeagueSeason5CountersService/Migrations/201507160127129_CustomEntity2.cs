namespace LeagueSeason5CountersService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CustomEntity2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("LeagueSeason5Counters.ChampionFeedbacks", "CreatedAt", c => c.DateTimeOffset(precision: 7));
            AlterColumn("LeagueSeason5Counters.Comments", "CreatedAt", c => c.DateTimeOffset(precision: 7));
            AlterColumn("LeagueSeason5Counters.UserRatings", "CreatedAt", c => c.DateTimeOffset(precision: 7));
        }
        
        public override void Down()
        {
            AlterColumn("LeagueSeason5Counters.UserRatings", "CreatedAt", c => c.DateTimeOffset(nullable: false, precision: 7));
            AlterColumn("LeagueSeason5Counters.Comments", "CreatedAt", c => c.DateTimeOffset(nullable: false, precision: 7));
            AlterColumn("LeagueSeason5Counters.ChampionFeedbacks", "CreatedAt", c => c.DateTimeOffset(nullable: false, precision: 7));
        }
    }
}
