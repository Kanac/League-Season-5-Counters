using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using LeagueSeason5CountersService.DataObjects;
using LeagueSeason5CountersService.Models;
using LeagueSeason5CountersService.MappedDomainManager;
using System.Web.Http.Description;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System;
using System.Web.Http.Filters;
using AutoMapper;

namespace LeagueSeason5CountersService.Controllers
{
    public class ChampionFeedbackController : TableController<ChampionFeedbackDto>
    {
        private LeagueSeason5CountersContext context = new LeagueSeason5CountersContext();

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            DomainManager = new SimpleMappedEntityDomainManager<ChampionFeedbackDto, ChampionFeedback>
                                (context, Request, Services, championFeedback => championFeedback.Id);
        }

        // GET tables/ChampionFeedback
        [EnableQuery(PageSize = 1000)]
        [ExpandProperty("Comments/UserRatings, Counters/CounterRatings")]
        public IQueryable<ChampionFeedbackDto> GetAllChampionFeedback()
        {
            return base.Query();
        }



        // GET tables/ChampionFeedback/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public ChampionFeedbackDto GetChampionFeedback(string id)
        {
            return Mapper.Map<ChampionFeedback, ChampionFeedbackDto>(context.ChampionFeedbacks.Where(c => c.Id == id).FirstOrDefault());
        }

        // PATCH tables/ChampionFeedback/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public async Task<ChampionFeedbackDto> PatchChampionFeedback(string id, Delta<ChampionFeedbackDto> patch)
        {

            ChampionFeedback currentChampionFeedback = this.context.ChampionFeedbacks.Include("Comments.UserRatings")
                                    .First(j => (j.Id == id));
            ChampionFeedbackDto updatedpatchEntity = patch.GetEntity();
            Mapper.Map<ChampionFeedbackDto, ChampionFeedback>(updatedpatchEntity, currentChampionFeedback);
            await this.context.SaveChangesAsync();
            // Convert to client type before returning the result
            var result = Mapper.Map<ChampionFeedback, ChampionFeedbackDto>(currentChampionFeedback);
            return result;
        }

        // POST tables/ChampionFeedback
        public async Task<IHttpActionResult> PostChampionFeedback(ChampionFeedbackDto item)
        {
            ChampionFeedbackDto current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/ChampionFeedback/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteChampionFeedback(string id)
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