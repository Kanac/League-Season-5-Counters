namespace LeagueSeason5CountersService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CustomEntity : DbMigration
    {
        public override void Up()
        {
            DropIndex("LeagueSeason5Counters.ChampionFeedbacks", new[] { "CreatedAt" });
            DropIndex("LeagueSeason5Counters.Comments", new[] { "CreatedAt" });
            DropIndex("LeagueSeason5Counters.UserRatings", new[] { "CreatedAt" });
        }
        
        public override void Down()
        {
            CreateIndex("LeagueSeason5Counters.UserRatings", "CreatedAt", clustered: true);
            CreateIndex("LeagueSeason5Counters.Comments", "CreatedAt", clustered: true);
            CreateIndex("LeagueSeason5Counters.ChampionFeedbacks", "CreatedAt", clustered: true);
        }
    }
}
