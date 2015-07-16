using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using Leaderboard.DataObjects;
using Leaderboard.Models;

namespace Leaderboard.Controllers
{
    public class PlayerController : TableController<Player>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            LeaderboardContext context = new LeaderboardContext();
            DomainManager = new EntityDomainManager<Player>(context, Request, Services);
        }

        // GET tables/Player
        public IQueryable<Player> GetAllPlayer()
        {
            return Query();
        }

        // GET tables/Player/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Player> GetPlayer(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Player/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Player> PatchPlayer(string id, Delta<Player> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/Player/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public async Task<IHttpActionResult> PostPlayer(Player item)
        {
            Player current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Player/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeletePlayer(string id)
        {
            return DeleteAsync(id);
        }

    }
}