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
        public async Task<UserRatingDto> PatchUserRating(string id, Delta<UserRatingDto> patch)
        {
            //Get the corresponding UserRating and Comment in the database from the DTO's PK and FK
            UserRating rating = this.context.UserRatings.First(u => (u.Id == id));
            Comment comment = this.context.Comments.First(c => c.Id == rating.CommentId);
            //Get the updated DTO sent here
            UserRatingDto updatedpatchEntity = patch.GetEntity();
            //Revert the previous rating score and add the new rating's score
            comment.Score += updatedpatchEntity.Score - rating.Score;  
            //Copy the new user rating properties over to the database rating
            Mapper.Map<UserRatingDto, UserRating>(updatedpatchEntity, rating);
            await this.context.SaveChangesAsync();
            //Return the new rating with server published data
            return Mapper.Map<UserRating, UserRatingDto>(rating, updatedpatchEntity);
        }

        // POST tables/UserRating
        public async Task<IHttpActionResult> PostUserRating(UserRatingDto item)
        {
            Comment comment = this.context.Comments.First(c => c.Id == item.CommentId);
            UserRating newRating = Mapper.Map<UserRatingDto, UserRating>(item);
            comment.UserRatings.Add(newRating);
            comment.Score += newRating.Score;
            await this.context.SaveChangesAsync();
            //Remap to DTO with the server published data to go with it
            Mapper.Map<UserRating, UserRatingDto>(newRating, item);
            return CreatedAtRoute("Tables", new { id = item.Id }, item);
        }

        // DELETE tables/UserRating/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteUserRating(string id)
        {
             return DeleteAsync(id);
        }

    }
}