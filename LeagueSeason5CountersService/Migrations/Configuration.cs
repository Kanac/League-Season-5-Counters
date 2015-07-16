namespace LeagueSeason5CountersService.Migrations
{
    using LeagueSeason5CountersService.DataObjects;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<LeagueSeason5CountersService.Models.LeagueSeason5CountersContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(LeagueSeason5CountersService.Models.LeagueSeason5CountersContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            List<ChampionFeedback> championFeedbackCollection = new List<ChampionFeedback>
            {
                new ChampionFeedback { Id = Guid.NewGuid().ToString(), Name = "Ezreal"},
                new ChampionFeedback { Id = Guid.NewGuid().ToString(), Name = "Ahri"},

            };
            foreach (ChampionFeedback championFeedback in championFeedbackCollection)
            {
                context.ChampionFeedbacks.Add(championFeedback);
            }

            base.Seed(context);
        }
    }
}
