using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using LeagueSeason5CountersService.Models;
using Microsoft.WindowsAzure.Mobile.Service.ScheduledJobs;
using System.Threading;
using System;
using System.Linq;
using LeagueSeason5CountersService.DataObjects;

namespace LeagueSeason5CountersService
{
    // A simple scheduled job which can be invoked manually by submitting an HTTP
    // POST request to the path "/jobs/Ad".

    public class AdJob : ScheduledJob
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
            Services.Log.Info("Change Ad unit");

            AdUnit currAdUnit = context.AdUnits.Where(x => x.InUse).FirstOrDefault();
            currAdUnit.LastUseddate = DateTime.Now;
            currAdUnit.InUse = false;
            AdUnit nextAdUnit = context.AdUnits.OrderBy(x => x.LastUseddate).FirstOrDefault();
            nextAdUnit.LastUseddate = DateTime.Now;
            nextAdUnit.InUse = true;

            context.SaveChanges();

            return Task.FromResult(true);
        }
    }
}