using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using LeagueSeason5CountersService.DataObjects;
using LeagueSeason5CountersService.Models;
using System.Collections.Generic;

namespace LeagueSeason5CountersService.Controllers
{
    public class CommentController : TableController<Comment>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            LeagueSeason5CountersContext context = new LeagueSeason5CountersContext();
            DomainManager = new EntityDomainManager<Comment>(context, Request, Services);
        }

        // GET tables/Comment
        public IQueryable<Comment> GetAllComment()
        {
            return Query();
        }

        // GET tables/Comment/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Comment> GetComment(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Comment/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Comment> PatchComment(string id, Delta<Comment> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/Comment
        public async Task<IHttpActionResult> PostComment(Comment item)
        {
            Comment current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Comment/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteComment(string id)
        {
             return DeleteAsync(id);
        }

    }
}