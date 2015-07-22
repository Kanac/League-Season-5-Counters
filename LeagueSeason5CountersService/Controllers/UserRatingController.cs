using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using LeagueSeason5CountersService.DataObjects;
using LeagueSeason5CountersService.Models;
using LeagueSeason5CountersService.MappedDomainManager;

namespace LeagueSeason5CountersService.Controllers
{
    public class UserRatingController : TableController<UserRatingDto>
    {
        private LeagueSeason5CountersContext context = new LeagueSeason5CountersContext();

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            DomainManager = new SimpleMappedEntityDomainManager<UserRatingDto, UserRating>(context, Request, Services, userRating => userRating.Id);
        }

        // GET tables/UserRating
        public IQueryable<UserRatingDto> GetAllUserRating()
        {
            return Query(); 
        }

        // GET tables/UserRating/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<UserRatingDto> GetUserRating(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/UserRating/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<UserRatingDto> PatchUserRating(string id, Delta<UserRatingDto> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/UserRating
        public async Task<IHttpActionResult> PostUserRating(UserRatingDto item)
        {
            UserRatingDto current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/UserRating/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteUserRating(string id)
        {
             return DeleteAsync(id);
        }

    }
}