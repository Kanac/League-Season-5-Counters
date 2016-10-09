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
            if (context.AdUnits.Count() != 0)
            {
                base.Seed(context);
                return;
            }

            string appId = "bf747944-c75c-4f2a-a027-7c159b32261d";
            DateTime date = DateTime.Now;

            List<int> idList = new List<int>
            {
                240175, 242815, 300199, 242100, 239130, 242804, 240199,
                240176, 305132, 249556, 242820, 240202, 242801, 244459,
                302450, 314536, 240174, 299204, 244417, 244435,
                244415, 244429, 244443, 244423, 244451, 244431, 244437,
                244421, 244413, 244427, 236727, 244425, 242101, 240170,
                304061, 240177, 244457, 244411, 244441,
                244433, 244419, 239138, 244461, 244449, 242841,
                240171, 312169, 244439, 244445
            };

            foreach (int id in idList)
            {
                AdUnit adUnit = new AdUnit { Id = Guid.NewGuid().ToString(), Ad = id, App = appId, LastUseddate = date, InUse = false };
                context.AdUnits.AddOrUpdate(adUnit);
            }

            base.Seed(context);
        }
    }
}
