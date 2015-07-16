using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using LeagueSeason5CountersService.DataObjects;
using LeagueSeason5CountersService.Models;

namespace LeagueSeason5CountersService.Controllers
{
    public class ChampionFeedbackController : TableController<ChampionFeedback>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            LeagueSeason5CountersContext context = new LeagueSeason5CountersContext();
            DomainManager = new EntityDomainManager<ChampionFeedback>(context, Request, Services);
        }

        // GET tables/ChampionFeedback
        public IQueryable<ChampionFeedback> GetAllChampionFeedback()
        {
            return Query(); 
        }

        // GET tables/ChampionFeedback/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<ChampionFeedback> GetChampionFeedback(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/ChampionFeedback/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<ChampionFeedback> PatchChampionFeedback(string id, Delta<ChampionFeedback> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/ChampionFeedback
        public async Task<IHttpActionResult> PostChampionFeedback(ChampionFeedback item)
        {
            ChampionFeedback current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/ChampionFeedback/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteChampionFeedback(string id)
        {
             return DeleteAsync(id);
        }

    }
}