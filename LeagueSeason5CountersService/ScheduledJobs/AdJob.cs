using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using LeagueSeason5CountersService.Models;
using Microsoft.WindowsAzure.Mobile.Service.ScheduledJobs;
using System.Threading;
using System;
using System.Linq;
using LeagueSeason5CountersService.DataObjects;
using System.Web.Configuration;

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

            string appIdPhone = WebConfigurationManager.AppSettings["APP_ID_PHONE"];
            string appIdWin10 = WebConfigurationManager.AppSettings["APP_ID_WIN10"];

            UseNextAdUnit(appIdPhone);
            UseNextAdUnit(appIdWin10);

            context.SaveChanges();

            return Task.FromResult(true);
        }

        private void UseNextAdUnit(string appId)
        {
            var adUnits = context.AdUnits.Where(x => x.App == appId);

            AdUnit currAdUnitPhone = adUnits.Where(x => x.InUse).FirstOrDefault();
            currAdUnitPhone.LastUseddate = DateTime.Now;
            currAdUnitPhone.InUse = false;
            AdUnit nextAdUnitPhone = adUnits.OrderBy(x => x.LastUseddate).FirstOrDefault();
            nextAdUnitPhone.LastUseddate = DateTime.Now;
            nextAdUnitPhone.InUse = true;

            context.SaveChanges();
        }
    }
}