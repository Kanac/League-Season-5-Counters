using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using LeagueSeason5CountersService.Models;
using Microsoft.WindowsAzure.Mobile.Service.ScheduledJobs;
using System.Threading;
using System;
using System.Linq;

namespace LeagueSeason5CountersService
{
    // A simple scheduled job which can be invoked manually by submitting an HTTP
    // POST request to the path "/jobs/sample".

    public class SampleJob : ScheduledJob
    {
        private LeagueSeason5CountersContext context;

        protected override void Initialize(ScheduledJobDescriptor scheduledJobDescriptor,
            CancellationToken cancellationToken)
        {
            base.Initialize(scheduledJobDescriptor, cancellationToken);
            context = new LeagueSeason5CountersContext();
        }
        public override Task ExecuteAsync()
        {
            Services.Log.Info("Purging old records");
            var monthAgo = DateTimeOffset.UtcNow.AddDays(-3);

            var toDelete1 = context.ChampionFeedbacks.Where(x => x.Deleted == true && x.UpdatedAt <= monthAgo).ToArray();
            var toDelete2 = context.Comments.Where(x => x.Deleted == true && x.UpdatedAt <= monthAgo).ToArray();
            var toDelete3 = context.UserRatings.Where(x => x.Deleted == true && x.UpdatedAt <= monthAgo).ToArray();
            context.ChampionFeedbacks.RemoveRange(toDelete1);
            context.Comments.RemoveRange(toDelete2);
            context.UserRatings.RemoveRange(toDelete3);
            context.SaveChanges();

            return Task.FromResult(true);
        }
    }
}