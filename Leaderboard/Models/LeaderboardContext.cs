using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Tables;
using Leaderboard.DataObjects;

namespace Leaderboard.Models
{

    public class LeaderboardContext : DbContext
    {
        private const string connectionStringName = "Name=MS_TableConnectionString";

        public LeaderboardContext()
            : base(connectionStringName)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            string schema = ServiceSettingsDictionary.GetSchemaName();
            if (!string.IsNullOrEmpty(schema))
            {
                modelBuilder.HasDefaultSchema(schema);
            }

            modelBuilder.Conventions.Add(
                new AttributeToColumnAnnotationConvention<TableColumnAttribute, string>(
                    "ServiceTableColumn", (property, attributes) => attributes.Single().ColumnType.ToString()));
        }

        public System.Data.Entity.DbSet<Leaderboard.DataObjects.Player> Players { get; set; }

        public System.Data.Entity.DbSet<Leaderboard.DataObjects.PlayerRank> PlayerRanks { get; set; }
    }

}
