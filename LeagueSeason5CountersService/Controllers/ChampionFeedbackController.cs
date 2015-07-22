using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using LeagueSeason5CountersService.DataObjects;
using LeagueSeason5CountersService.Models;
using LeagueSeason5CountersService.MappedDomainManager;
using System.Web.Http.Description;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System;
using System.Web.Http.Filters;
using AutoMapper;

namespace LeagueSeason5CountersService.Controllers
{
    public class ChampionFeedbackController : TableController<ChampionFeedbackDto>
    {
        private LeagueSeason5CountersContext context = new LeagueSeason5CountersContext();

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            DomainManager = new SimpleMappedEntityDomainManager<ChampionFeedbackDto, ChampionFeedback>
                                (context, Request, Services, championFeedback => championFeedback.Id);
        }

        // GET tables/ChampionFeedback
        [ExpandProperty("Comments/UserRatings")]
        public IQueryable<ChampionFeedbackDto> GetAllChampionFeedback()
        {
            return base.Query();
        }



        // GET tables/ChampionFeedback/48D68C86-6EA6-4C25-AA33-223FC9A27959
        [ExpandProperty("Comments/UserRatings")]
        public SingleResult<ChampionFeedbackDto> GetChampionFeedback(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/ChampionFeedback/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public async Task<ChampionFeedbackDto> PatchChampionFeedback(string id, Delta<ChampionFeedbackDto> patch)
        {
            //return await UpdateAsync(id, patch);
            //Look up ChampionFeedback from database so that EntityFramework updates
            //existing entry
            ChampionFeedback currentChampionFeedback = this.context.ChampionFeedbacks.Include("Comments.UserRatings")
                                    .First(j => (j.Id == id));

            ChampionFeedbackDto updatedpatchEntity = patch.GetEntity();
            ICollection<CommentDto> updatedComments;
            //Check if incoming request contains Items
            bool requestContainsRelatedEntities = patch.GetChangedPropertyNames()
                                    .Contains("Comments");

            if (requestContainsRelatedEntities)
            {
                //Remove related entities from the database. Comment following for loop if you do not
                //want to delete related entities from the database
                for (int i = 0; i < currentChampionFeedback.Comments.Count
                    && updatedpatchEntity.Comments != null; i++)
                {
                    CommentDto itemDTO = updatedpatchEntity.Comments.FirstOrDefault(j =>
                                    (j.Id == currentChampionFeedback.Comments.ElementAt(i).Id));
                    if (itemDTO == null)  //If comment is null, it was marked as removed, so remove it 
                    {
                        this.context.Comments.Remove(currentChampionFeedback.Comments.ElementAt(i));
                    }
                }

                //If request contains Comments get the updated list from the patch
                //Mapper.Map<ChampionFeedbackDto, ChampionFeedback>(updatedpatchEntity, currentChampionFeedback);
                updatedComments = updatedpatchEntity.Comments;
            }
            else
            {
                //If request doest not have Comments, then retain the original association
                Mapper.Map<ChampionFeedbackDto, ChampionFeedback>(updatedpatchEntity, currentChampionFeedback);
                ChampionFeedbackDto championFeedbackDTOUpdated = Mapper.Map<ChampionFeedback, ChampionFeedbackDto>
                                                (currentChampionFeedback);
                patch.Patch(championFeedbackDTOUpdated);
                Mapper.Map<ChampionFeedbackDto, ChampionFeedback>(championFeedbackDTOUpdated, currentChampionFeedback);
                updatedComments = championFeedbackDTOUpdated.Comments;
            }

            if (updatedComments != null)  //If there were updated comments
            {
                //Update related Items
                //currentChampionFeedback.Comments = new List<Comment>();
                foreach (CommentDto currentCommentDTO in updatedComments)
                {
                    //Look up existing entry in database
                    Comment existingComment = this.context.Comments
                                .FirstOrDefault(j => (j.Id == currentCommentDTO.Id));

                    //If this is not a new comment, and no rating changed, don't re-add it (there is no change to this comment -- move on) 
                    if (existingComment != null && currentCommentDTO.Score == existingComment.Score)
                        continue;

                    //if the rating of this comment changed, either a new rating was added or a userRating score changed 
                    else if (existingComment != null && currentCommentDTO.Score != existingComment.Score)
                    {
                        foreach (var updatedRating in currentCommentDTO.UserRatings) {
                            //Find the corresponding existing rating to this potentially updated rating
                            UserRating existingRating = existingComment.UserRatings.FirstOrDefault(x => (x.Id == updatedRating.Id));
                            //If the corresponding existing rate is null, then that means a new rating was added (create it on the database)
                            if (existingRating == null) {
                                existingRating = Mapper.Map<UserRatingDto, UserRating>(updatedRating);
                                existingComment.UserRatings.Add(existingRating);
                            }
                            //Otherwise, check if the rating changed
                            else if (existingRating.Score != updatedRating.Score) {
                                //Update the existing rating score 
                                existingRating.Score = updatedRating.Score;
                            }
                        }

                        //Update comment score as well
                        existingComment.Score = currentCommentDTO.Score;
                    }

                    //Otherwise, this is a new comment -- add it
                    else
                    {
                        //Convert client type to database type
                        existingComment = Mapper.Map<CommentDto, Comment>(currentCommentDTO,
                                existingComment);
                        var existingRatings = Mapper.Map<ICollection<UserRatingDto>, ICollection<UserRating>>(currentCommentDTO.UserRatings);
                        existingComment.ChampionFeedback = currentChampionFeedback;
                        existingComment.UserRatings = existingRatings;
                        currentChampionFeedback.Comments.Add(existingComment);
                    }
                }
            }

            await this.context.SaveChangesAsync();

            //Convert to client type before returning the result
            var result = Mapper.Map<ChampionFeedback, ChampionFeedbackDto>(currentChampionFeedback);
            return result;
        }

        // POST tables/ChampionFeedback
        public async Task<IHttpActionResult> PostChampionFeedback(ChampionFeedbackDto item)
        {
            ChampionFeedbackDto current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/ChampionFeedback/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteChampionFeedback(string id)
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