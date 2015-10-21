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
    public class CounterRatingController : TableController<CounterRatingDto>
    {
        LeagueSeason5CountersContext context = new LeagueSeason5CountersContext();

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            DomainManager = new SimpleMappedEntityDomainManager<CounterRatingDto, CounterRating>(context, Request, Services, counterRating => counterRating.Id);
        }

        // GET tables/CounterRating
        [EnableQuery(PageSize = 1000)]
        public IQueryable<CounterRatingDto> GetAllCounterRating()
        {
            return Query(); 
        }

        // GET tables/CounterRating/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<CounterRatingDto> GetCounterRating(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/CounterRating/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public async Task<CounterRatingDto> PatchCounterRating(string id, Delta<CounterRatingDto> patch)
        {
            //Get the database counter rating
            CounterRating rating = this.context.CounterRatings.First(u => (u.Id == id));
            //Get the database counter 
            Counter counter = this.context.Counters.First(c => c.Id == rating.CounterId);
            //Get the updated counter rating Dto
            CounterRatingDto updatedpatchEntity = patch.GetEntity();
            //Re-calculate the new counter score
            counter.Score += updatedpatchEntity.Score - rating.Score;  
            //Push the new dto into the database
            Mapper.Map<CounterRatingDto, CounterRating>(updatedpatchEntity, rating);
            await this.context.SaveChangesAsync();
            //Return the new rating with server published data
            return Mapper.Map<CounterRating, CounterRatingDto>(rating, updatedpatchEntity);
        }

        // POST tables/CounterRating
        public async Task<IHttpActionResult> PostCounterRating(CounterRatingDto item)
        {
            Counter counter = this.context.Counters.First(c => c.Id == item.CounterId);
            CounterRating newRating = Mapper.Map<CounterRatingDto, CounterRating>(item);
            counter.CounterRatings.Add(newRating);
            counter.Score += newRating.Score;
            await this.context.SaveChangesAsync();
            //Remap to DTO with the server published data to go with it
            Mapper.Map<CounterRating, CounterRatingDto>(newRating, item);
            return CreatedAtRoute("Tables", new { id = item.Id }, item);
        }

        // DELETE tables/CounterRating/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteCounterRating(string id)
        {
             return DeleteAsync(id);
        }

    }
}