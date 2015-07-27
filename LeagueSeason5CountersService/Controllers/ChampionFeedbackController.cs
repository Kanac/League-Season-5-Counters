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
        [ExpandProperty("Comments/UserRatings, Counters/CounterRatings")]
        public IQueryable<ChampionFeedbackDto> GetAllChampionFeedback()
        {
            return base.Query();
        }



        // GET tables/ChampionFeedback/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public ChampionFeedbackDto GetChampionFeedback(string id)
        {
            return Mapper.Map<ChampionFeedback, ChampionFeedbackDto>(context.ChampionFeedbacks.Where(c => c.Id == id).FirstOrDefault());
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
            //Check if incoming request contains Items
  

                //Remove related entities from the database. Comment following for loop if you do not
                //want to delete related entities from the database
                //for (int i = 0; i < currentChampionFeedback.Comments.Count
                //    && updatedpatchEntity.Comments != null; i++)
                //{
                //    CommentDto itemDTO = updatedpatchEntity.Comments.FirstOrDefault(j =>
                //                    (j.Id == currentChampionFeedback.Comments.ElementAt(i).Id));
                //    if (itemDTO == null)  //If comment is null, it was marked as removed, so remove it 
                //    {
                //        this.context.Comments.Remove(currentChampionFeedback.Comments.ElementAt(i));
                //    }
                //}

                //If request contains Comments get the updated list from the patch
                //Mapper.Map<ChampionFeedbackDto, ChampionFeedback>(updatedpatchEntity, currentChampionFeedback);
                Mapper.Map<ChampionFeedbackDto, ChampionFeedback>(updatedpatchEntity, currentChampionFeedback);



            //if (updatedComments != null)  //If there were updated comments
            //{
            //    //Update related Items -- only one thing is meant to be done at once -- either add new comment, update rating or add rating
            //    //currentChampionFeedback.Comments = new List<Comment>();
            //    foreach (CommentDto currentCommentDTO in updatedComments)
            //    {
            //        //Look up existing entry in database
            //        Comment existingComment = this.context.Comments
            //                    .FirstOrDefault(j => (j.Id == currentCommentDTO.Id));

            //        //if the rating of this comment changed, either a new rating was added or a userRating score changed 
            //        if (existingComment != null )
            //        {
            //            foreach (var updatedRating in currentCommentDTO.UserRatings) {
            //                //Find the corresponding existing rating to this potentially updated rating
            //                UserRating existingRating = existingComment.UserRatings.FirstOrDefault(x => (x.Id == updatedRating.Id));
            //                //If the corresponding existing rate is null, then that means a new rating was added (create it on the database)
            //                if (existingRating == null) {
            //                    existingRating = Mapper.Map<UserRatingDto, UserRating>(updatedRating);
            //                    existingComment.UserRatings.Add(existingRating);
            //                    existingComment.Score += existingRating.Score;
            //                    break;
            //                }
            //                //Otherwise, check if the rating changed (also ensure that a particular rating has an updatedAt value set to indicate that it was the one updated
            //                //to prevent other behind-server ratings from interfering)
            //                else if (existingRating.Score != updatedRating.Score && updatedRating.UniqueUser == "RatingUpdate" ) 
            //                {
            //                    //Update the existing rating score 
            //                    existingComment.Score += updatedRating.Score - existingRating.Score; //revert the previous rating 
            //                    existingRating.Score = updatedRating.Score; //set the new score for the rating
            //                    break;


            //                }
            //            }
            //        }

            //        //Otherwise, this is a new comment -- add it
            //        else
            //        {
            //            //Convert client type to database type
            //            existingComment = Mapper.Map<CommentDto, Comment>(currentCommentDTO, existingComment);
            //            existingComment.ChampionFeedback = currentChampionFeedback;
            //            currentChampionFeedback.Comments.Add(existingComment);
            //            break;
            //        }
            //    }
            //}

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