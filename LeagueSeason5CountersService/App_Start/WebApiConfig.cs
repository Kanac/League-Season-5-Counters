using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Web.Http;
using AutoMapper;
using Microsoft.WindowsAzure.Mobile.Service;
using LeagueSeason5CountersService.DataObjects;
using LeagueSeason5CountersService.Models;
using System.Data.Entity.Migrations;
using LeagueSeason5CountersService.Migrations;

namespace LeagueSeason5CountersService
{
    public static class WebApiConfig
    {
        public static void Register()
        {
            // Use this class to set configuration options for your mobile service
            ConfigOptions options = new ConfigOptions();

            // Use this class to set WebAPI configuration options
            HttpConfiguration config = ServiceConfig.Initialize(new ConfigBuilder(options));

            Mapper.Initialize(cfg =>
            {
                // Define a map from the database type TodoItem to 
                // client type TodoItemDto. Used when getting data.
                cfg.CreateMap<ChampionFeedback, ChampionFeedbackDto>();
                cfg.CreateMap<Comment, CommentDto>();
                cfg.CreateMap<UserRating, UserRatingDto>();
                cfg.CreateMap<Counter, CounterDto>();
                cfg.CreateMap<CounterRating, CounterRatingDto>();

                // Define a map from the client type to the database
                // type. Used when inserting and updating data.
                cfg.CreateMap<ChampionFeedbackDto, ChampionFeedback>();
                cfg.CreateMap<CommentDto, Comment>();
                cfg.CreateMap<UserRatingDto, UserRating>();
                cfg.CreateMap<CounterDto, Counter>();
                cfg.CreateMap<CounterRatingDto, CounterRating>();

            });

            
            // To display errors in the browser during development, uncomment the following
            // line. Comment it out again when you deploy your service for production use.
            //config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;



            Database.SetInitializer(new MigrateDatabaseToLatestVersion<LeagueSeason5CountersContext, Configuration>());
            //var migrator = new DbMigrator(new Configuration());
            //migrator.Update();
        }
    }

    public class LeagueSeason5CountersInitializer : CreateDatabaseIfNotExists<LeagueSeason5CountersContext>
    {
        protected override void Seed(LeagueSeason5CountersContext context)
        {
            //List<ChampionFeedback> championFeedbackCollection = new List<ChampionFeedback>
            //{
            //    new ChampionFeedback { Id = Guid.NewGuid().ToString(), Name = "Ezreal" },
            //    new ChampionFeedback { Id = Guid.NewGuid().ToString(), Name = "Ahri" }

            //};
            //foreach (ChampionFeedback championFeedback in championFeedbackCollection)
            //{
            //    context.ChampionFeedbacks.Add(championFeedback);
            //}

            //base.Seed(context);
        }
    }
}

