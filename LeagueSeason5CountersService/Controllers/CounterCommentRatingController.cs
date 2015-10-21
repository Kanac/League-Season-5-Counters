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

namespace LeagueSeason5CountersService.Controllers
{
    public class CounterCommentRatingController : TableController<CounterCommentRatingDto>
    {
        private LeagueSeason5CountersContext context = new LeagueSeason5CountersContext();

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            DomainManager = new SimpleMappedEntityDomainManager<CounterCommentRatingDto, CounterCommentRating>(context, Request, Services, comment => comment.Id);
        }

        // GET tables/CounterCommentRating
        [EnableQuery(PageSize = 1000)]
        public IQueryable<CounterCommentRatingDto> GetAllCounterCommentRating()
        {
            return Query(); 
        }

        // GET tables/CounterCommentRating/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<CounterCommentRatingDto> GetCounterCommentRating(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/CounterCommentRating/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public async Task<CounterCommentRatingDto> PatchCounterCommentRating(string id, Delta<CounterCommentRatingDto> patch)
        {
            //Get the database counter rating
            CounterCommentRating rating = this.context.CounterCommentRatings.First(u => (u.Id == id));
            //Get the database counter 
            CounterComment counter = this.context.CounterComments.First(c => c.Id == rating.CounterCommentId);
            //Get the updated counter rating Dto
            CounterCommentRatingDto updatedpatchEntity = patch.GetEntity();
            //Re-calculate the new counter score
            counter.Score += updatedpatchEntity.Score - rating.Score;
            //Push the new dto into the database
            Mapper.Map<CounterCommentRatingDto, CounterCommentRating>(updatedpatchEntity, rating);
            await this.context.SaveChangesAsync();
            //Return the new rating with server published data
            return Mapper.Map<CounterCommentRating, CounterCommentRatingDto>(rating, updatedpatchEntity);
        }

        // POST tables/CounterCommentRating
        public async Task<IHttpActionResult> PostCounterCommentRating(CounterCommentRatingDto item)
        {
            CounterComment comment = this.context.CounterComments.First(c => c.Id == item.CounterCommentId);
            CounterCommentRating newRating = Mapper.Map<CounterCommentRatingDto, CounterCommentRating>(item);
            comment.CounterCommentRatings.Add(newRating);
            comment.Score += newRating.Score;
            await this.context.SaveChangesAsync();
            //Remap to DTO with the server published data to go with it
            Mapper.Map<CounterCommentRating, CounterCommentRatingDto>(newRating, item);
            return CreatedAtRoute("Tables", new { id = item.Id }, item);
        }

        // DELETE tables/CounterCommentRating/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteCounterCommentRating(string id)
        {
             return DeleteAsync(id);
        }

    }
}