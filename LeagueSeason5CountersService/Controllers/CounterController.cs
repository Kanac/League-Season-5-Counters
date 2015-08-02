using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using LeagueSeason5CountersService.DataObjects;
using LeagueSeason5CountersService.Models;
using LeagueSeason5CountersService.MappedDomainManager;
using AutoMapper;
using System.Net;
using System;
using System.Web.Http.Filters;

namespace LeagueSeason5CountersService.Controllers
{
    public class CounterController : TableController<CounterDto>
    {
        LeagueSeason5CountersContext context = new LeagueSeason5CountersContext();

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            DomainManager = new SimpleMappedEntityDomainManager<CounterDto, Counter>(context, Request, Services, counter => counter.Id);
        }

        // GET tables/Counter
        [EnableQuery(PageSize = 1000)]
        [ExpandProperty("CounterRatings")]
        public IQueryable<CounterDto> GetAllCounter()
        {
            return Query(); 
        }

        // GET tables/Counter/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public CounterDto GetCounter(string id)
        {
            return Mapper.Map<Counter, CounterDto>(context.Counters.Where(c => c.Id == id).FirstOrDefault());
        }

        // PATCH tables/Counter/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<CounterDto> PatchCounter(string id, Delta<CounterDto> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/Counter
        public async Task<IHttpActionResult> PostCounter(CounterDto item)
        {
            // Find the corresponding champion to this counter
            var champFeedback = this.context.ChampionFeedbacks.Where(c => c.Id == item.ChampionFeedbackId).FirstOrDefault();
            
            if (item.Name == champFeedback.Name)
                return Content(HttpStatusCode.MethodNotAllowed, "Attempting to add the champion itself to the counter list is not allowed.");

            // Check if the counter being submitted is allowed for counters
            if (item.Page == PageEnum.ChampionPage.Counter)
            {
                if (champFeedback.Counters.Where(c => c.Page == PageEnum.ChampionPage.Counter && c.Name == item.Name).Count() == 1)
                {
                    return Content(HttpStatusCode.MethodNotAllowed, "Attempting to add a duplicate counter is not allowed.");
                }
            }

            // Check if the counter being submitted is allowed for synergies 
            else 
            {
                if (champFeedback.Counters.Where(c => c.Page == PageEnum.ChampionPage.Synergy && c.Name == item.Name ).Count() == 1)
                {
                    return Content(HttpStatusCode.MethodNotAllowed, "Attempting to add a duplicate synergy is not allowed.");
                }
            }

            // Map the counter DTO to the server type
            var newCounter = Mapper.Map<CounterDto, Counter>(item);
            // Add it to the counter collection of the champion
            champFeedback.Counters.Add(newCounter);
            await this.context.SaveChangesAsync();

            // Map the new counter back to the item with new server published information
            Mapper.Map<Counter, CounterDto>(newCounter, item);
            return CreatedAtRoute("Tables", new { id = item.Id }, item);
        }

        // DELETE tables/Counter/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteCounter(string id)
        {
             return DeleteAsync(id);
        }


        [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
        class ExpandPropertyAttribute : ActionFilterAttribute
        {
            string propertyName;

            public ExpandPropertyAttribute(string propertyName)
            {
                this.propertyName = propertyName;
            }

            public override void OnActionExecuting(HttpActionContext actionContext)
            {
                base.OnActionExecuting(actionContext);
                var uriBuilder = new UriBuilder(actionContext.Request.RequestUri);
                var queryParams = uriBuilder.Query.TrimStart('?').Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                int expandIndex = -1;
                for (var i = 0; i < queryParams.Count; i++)
                {
                    if (queryParams[i].StartsWith("$expand", StringComparison.Ordinal))
                    {
                        expandIndex = i;
                        break;
                    }
                }

                if (expandIndex < 0)
                {
                    queryParams.Add("$expand=" + this.propertyName);
                }
                else
                {
                    queryParams[expandIndex] = queryParams[expandIndex] + "," + propertyName;
                }

                uriBuilder.Query = string.Join("&", queryParams);
                actionContext.Request.RequestUri = uriBuilder.Uri;
            }
        }
    }
}