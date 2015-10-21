using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using LeagueSeason5CountersService.DataObjects;
using LeagueSeason5CountersService.Models;
using LeagueSeason5CountersService.MappedDomainManager;
using System;
using System.Web.Http.Filters;
using AutoMapper;

namespace LeagueSeason5CountersService.Controllers
{
    public class CounterCommentController : TableController<CounterCommentDto>
    {
        private LeagueSeason5CountersContext context = new LeagueSeason5CountersContext();
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            DomainManager = new SimpleMappedEntityDomainManager<CounterCommentDto, CounterComment>(context, Request, Services, comment => comment.Id);
        }

        // GET tables/CounterComment
        [EnableQuery(PageSize = 1000)]
        [ExpandProperty("CounterCommentRatings")]
        public IQueryable<CounterCommentDto> GetAllCounterComment()
        {
            return Query(); 
        }

        // GET tables/CounterComment/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public CounterCommentDto GetCounterComment(string id)
        {
            return Mapper.Map<CounterComment, CounterCommentDto>(context.CounterComments.Where(c => c.Id == id).FirstOrDefault());
        }

        // PATCH tables/CounterComment/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<CounterCommentDto> PatchCounterComment(string id, Delta<CounterCommentDto> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/CounterComment
        public async Task<IHttpActionResult> PostCounterComment(CounterCommentDto item)
        {
            // Find corresponding counter 
            var counter = this.context.Counters.Where(c => c.Id == item.CounterId).FirstOrDefault();
            var newComment = Mapper.Map<CounterCommentDto, CounterComment>(item);
            counter.CounterComments.Add(newComment);
            await this.context.SaveChangesAsync();

            Mapper.Map<CounterComment, CounterCommentDto>(newComment, item);
            return CreatedAtRoute("Tables", new { id = item.Id }, item);
        }

        // DELETE tables/CounterComment/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteCounterComment(string id)
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