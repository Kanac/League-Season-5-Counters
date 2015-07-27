using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Tables;
using LeagueSeason5CountersService.DataObjects;

namespace LeagueSeason5CountersService.Models
{
    public class LeagueSeason5CountersContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to alter your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
        //
        // To enable Entity Framework migrations in the cloud, please ensure that the 
        // service name, set by the 'MS_MobileServiceName' AppSettings in the local 
        // Web.config, is the same as the service name when hosted in Azure.
        private const string connectionStringName = "Name=MS_TableConnectionString";

        public LeagueSeason5CountersContext() : base(connectionStringName)
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

        public System.Data.Entity.DbSet<LeagueSeason5CountersService.DataObjects.Comment> Comments { get; set; }

        public System.Data.Entity.DbSet<LeagueSeason5CountersService.DataObjects.ChampionFeedback> ChampionFeedbacks { get; set; }

        public System.Data.Entity.DbSet<LeagueSeason5CountersService.DataObjects.UserRating> UserRatings { get; set; }

        public System.Data.Entity.DbSet<LeagueSeason5CountersService.DataObjects.Counter> Counters { get; set; }

        public System.Data.Entity.DbSet<LeagueSeason5CountersService.DataObjects.CounterRating> CounterRatings { get; set; }
    }

}
