namespace LeagueSeason5CountersService.Migrations
{
    using LeagueSeason5CountersService.DataObjects;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
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

            //List<UserRating> userRatings = new List<UserRating>{
            //    new UserRating {Id = Guid.NewGuid().ToString(), Score=1, UniqueUser="123"},
            //    new UserRating {Id = Guid.NewGuid().ToString(), Score=-1, UniqueUser="456"},
            //    new UserRating {Id = Guid.NewGuid().ToString(), Score=-1, UniqueUser="789"},

            //};

            //List<Comment> comments = new List<Comment>
            //{
            //    new Comment {Id = Guid.NewGuid().ToString(), Score = 0, Text = "Test1", User="Ant", UserRatings = new Collection<UserRating>{userRatings[0]}},
            //    new Comment {Id = Guid.NewGuid().ToString(), Score = 0, Text = "Test2", User="Bob", UserRatings = new Collection<UserRating>{userRatings[1]}},
            //    new Comment {Id = Guid.NewGuid().ToString(), Score = 0, Text = "Test3", User="Charlie", UserRatings = new Collection<UserRating>{userRatings[2]}}


            //};

            //List<ChampionFeedback> championFeedbackCollection = new List<ChampionFeedback>
            //{
            //    new ChampionFeedback { Id = Guid.NewGuid().ToString(), Name = "Taric", Comments = new Collection<Comment>{comments[0]}},
            //    new ChampionFeedback { Id = Guid.NewGuid().ToString(), Name = "Alistar", Comments = new Collection<Comment>{comments[1]}},
            //    new ChampionFeedback { Id = Guid.NewGuid().ToString(), Name = "Warwick", Comments = new Collection<Comment>{comments[2]}},

            //};

            //foreach (ChampionFeedback championFeedback in championFeedbackCollection)
            //{
            //    context.ChampionFeedbacks.AddOrUpdate(championFeedback);
            //}

            //base.Seed(context);
        }
    }
}
