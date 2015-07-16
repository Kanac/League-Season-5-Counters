using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Web.Http;
using Leaderboard.DataObjects;
using Leaderboard.Models;
using Microsoft.WindowsAzure.Mobile.Service;

namespace Leaderboard
{
    public static class WebApiConfig
    {
        public static void Register()
        {
            // Use this class to set configuration options for your mobile service
            ConfigOptions options = new ConfigOptions();

            // Use this class to set WebAPI configuration options
            HttpConfiguration config = ServiceConfig.Initialize(new ConfigBuilder(options));

            // To display errors in the browser during development, uncomment the following
            // line. Comment it out again when you deploy your service for production use.
            // config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            Database.SetInitializer(new LeaderboardInitializer());
        }
    }

    public class LeaderboardInitializer : DropCreateDatabaseIfModelChanges<LeaderboardContext>
    {
        protected override void Seed(LeaderboardContext context)
        {
            List<Player> players = new List<Player>
            {
                new Player { Id = Guid.NewGuid().ToString(), Name = "Alice" },
                new Player { Id = Guid.NewGuid().ToString(), Name = "Bro" },
                new Player { Id = Guid.NewGuid().ToString(), Name = "Charles" }
            };

            foreach (var player in players)
            {
                context.Players.Add(player);
            }

            
            base.Seed(context);
        }
    }
}

