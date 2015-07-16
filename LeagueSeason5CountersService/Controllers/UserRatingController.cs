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
    public class UserRatingController : TableController<UserRating>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            LeagueSeason5CountersContext context = new LeagueSeason5CountersContext();
            DomainManager = new EntityDomainManager<UserRating>(context, Request, Services);
        }

        // GET tables/UserRating
        public IQueryable<UserRating> GetAllUserRating()
        {
            return Query(); 
        }

        // GET tables/UserRating/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<UserRating> GetUserRating(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/UserRating/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<UserRating> PatchUserRating(string id, Delta<UserRating> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/UserRating
        public async Task<IHttpActionResult> PostUserRating(UserRating item)
        {
            UserRating current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/UserRating/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteUserRating(string id)
        {
             return DeleteAsync(id);
        }

    }
}