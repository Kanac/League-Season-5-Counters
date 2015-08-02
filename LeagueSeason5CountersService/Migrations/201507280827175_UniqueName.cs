namespace LeagueSeason5CountersService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UniqueName : DbMigration
    {
        public override void Up()
        {
            AlterColumn("LeagueSeason5Counters.ChampionFeedbacks", "Name", c => c.String(maxLength: 200));
            CreateIndex("LeagueSeason5Counters.ChampionFeedbacks", "Name", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("LeagueSeason5Counters.ChampionFeedbacks", new[] { "Name" });
            AlterColumn("LeagueSeason5Counters.ChampionFeedbacks", "Name", c => c.String());
        }
    }
}
