using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using LeagueSeason5CountersService.DataObjects;
using LeagueSeason5CountersService.Models;
using System.Collections.Generic;
using LeagueSeason5CountersService.MappedDomainManager;
using System.Web.Http.Filters;
using System;
using AutoMapper;

namespace LeagueSeason5CountersService.Controllers
{
    public class CommentController : TableController<CommentDto>
    {
        private LeagueSeason5CountersContext context = new LeagueSeason5CountersContext();
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            DomainManager = new SimpleMappedEntityDomainManager<CommentDto, Comment>(context, Request, Services, comment => comment.Id);
        }

        // GET tables/Comment
        [ExpandProperty("UserRatings")]
        public IQueryable<CommentDto> GetAllComment()
        {
            return Query();
        }

        // GET tables/Comment/48D68C86-6EA6-4C25-AA33-223FC9A27959
        [ExpandProperty("UserRatings")]
        public SingleResult<CommentDto> GetComment(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Comment/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public async Task<CommentDto> PatchComment(string id, Delta<CommentDto> patch)
        {

            //Look up TodoItem from database so that EntityFramework updates
            //existing entry
            Comment currentComment = this.context.Comments.Include("UserRatings")
                                    .First(j => (j.Id == id));

            CommentDto updatedpatchEntity = patch.GetEntity();
            ICollection<UserRatingDto> updatedUserRatings;
            //Check if incoming request contains Items
            bool requestContainsRelatedEntities = patch.GetChangedPropertyNames()
                                    .Contains("UserRatings");

            if (requestContainsRelatedEntities)
            {
                //Remove related entities from the database. Comment following for loop if you do not
                //want to delete related entities from the database
                for (int i = 0; i < currentComment.UserRatings.Count
                    && updatedpatchEntity.UserRatings != null; i++)
                {
                    UserRatingDto itemDTO = updatedpatchEntity.UserRatings.FirstOrDefault(j =>
                                    (j.Id == currentComment.UserRatings.ElementAt(i).Id));
                    if (itemDTO == null)
                    {
                        this.context.UserRatings.Remove(currentComment.UserRatings.ElementAt(i));
                    }
                }

                //If request contains Items get the updated list from the patch
                Mapper.Map<CommentDto, Comment>(updatedpatchEntity, currentComment);
                updatedUserRatings = updatedpatchEntity.UserRatings;
            }
            else
            {
                //If request doest not have Items, then retain the original association
                CommentDto commentDTOUpdated = Mapper.Map<Comment, CommentDto>
                                                (currentComment);
                patch.Patch(commentDTOUpdated);
                Mapper.Map<CommentDto, Comment>(commentDTOUpdated, currentComment);
                updatedUserRatings = commentDTOUpdated.UserRatings;
            }

            if (updatedUserRatings != null)
            {
                //Update related Items
                currentComment.UserRatings = new List<UserRating>();
                foreach (UserRatingDto currentUserRatingDTO in updatedUserRatings)
                {
                    //Look up existing entry in database
                    UserRating existingUserRating = this.context.UserRatings
                                .FirstOrDefault(j => (j.Id == currentUserRatingDTO.Id));
                    //Convert client type to database type
                    existingUserRating = Mapper.Map<UserRatingDto, UserRating>(currentUserRatingDTO,
                            existingUserRating);
                    existingUserRating.Comment = currentComment;
                    currentComment.UserRatings.Add(existingUserRating);
                }
            }

            await this.context.SaveChangesAsync();

            //Convert to client type before returning the result
            var result = Mapper.Map<Comment, CommentDto>(currentComment);
            return result;
        }

        // POST tables/Comment
        public async Task<IHttpActionResult> PostComment(CommentDto item)
        {
            CommentDto current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Comment/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteComment(string id)
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