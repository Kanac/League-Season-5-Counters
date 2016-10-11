namespace LeagueSeason5CountersService.Migrations
{
    using LeagueSeason5CountersService.DataObjects;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Web.Configuration;
    internal sealed class Configuration : DbMigrationsConfiguration<LeagueSeason5CountersService.Models.LeagueSeason5CountersContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(LeagueSeason5CountersService.Models.LeagueSeason5CountersContext context)
        {
            if (context.AdUnits.Count() != 0)
            {
                base.Seed(context);
                return;
            }

            string appId = WebConfigurationManager.AppSettings["APP_ID_WIN10"];

            ISet<int> ids = new HashSet<int>
            {
                240296, 240305, 240306, 240307, 240308,
                240309, 240310, 240311, 240312, 240313,
                242902, 242903, 242912, 242913, 242914,
                242915, 242985, 242986, 242987, 242988,
                244643, 244652, 244653, 244655, 244656,
                246589, 246611, 246590, 246592, 246595,
                293532, 304062, 305133, 306229, 312172,
                297340, 298016, 298849, 299205, 299480,
                299713, 300198, 300957, 236223, 314537,
                242984, 280315, 239049, 239058, 239059,
                239060, 239061, 239062, 239063, 239064
            };

            foreach (int id in ids)
            {
                AdUnit adUnit = new AdUnit { Id = Guid.NewGuid().ToString(), Ad = id, App = appId, LastUseddate = DateTime.Now, InUse = false };
                context.AdUnits.AddOrUpdate(adUnit);
            }

            base.Seed(context);
        }
    }
}
