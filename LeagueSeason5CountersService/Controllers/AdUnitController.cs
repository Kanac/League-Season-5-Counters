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
    public class AdUnitController : TableController<AdUnit>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            LeagueSeason5CountersContext context = new LeagueSeason5CountersContext();
            DomainManager = new EntityDomainManager<AdUnit>(context, Request, Services);
        }

        // GET tables/AdUnit
        public IQueryable<AdUnit> GetAllAdUnit()
        {
            return Query(); 
        }

        // GET tables/AdUnit/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<AdUnit> GetAdUnit(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/AdUnit/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<AdUnit> PatchAdUnit(string id, Delta<AdUnit> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/AdUnit
        public async Task<IHttpActionResult> PostAdUnit(AdUnit item)
        {
            AdUnit current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/AdUnit/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteAdUnit(string id)
        {
             return DeleteAsync(id);
        }

    }
}