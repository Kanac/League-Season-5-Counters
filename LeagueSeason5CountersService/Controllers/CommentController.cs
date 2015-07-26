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
        public CommentDto GetComment(string id)
        {
            return Mapper.Map<Comment, CommentDto>(context.Comments.Where(c => c.Id == id).FirstOrDefault());
        }

        // PATCH tables/Comment/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public async Task<CommentDto> PatchComment(string id, Delta<CommentDto> patch)
        {

            return await UpdateAsync(id, patch);
        }

        // POST tables/Comment
        public async Task<IHttpActionResult> PostComment(CommentDto item)
        {
            //Find the corresponing champion to this comment
            var champFeedback = this.context.ChampionFeedbacks.Where(c => c.Id == item.ChampionFeedbackId).FirstOrDefault();
            //Map the comment DTO to the server type
            var newComment = Mapper.Map<CommentDto, Comment>(item);
            //Add it to the comment collection of the champion
            champFeedback.Comments.Add(newComment);
            await this.context.SaveChangesAsync();

            //Map the new comment back to the item with new server published information
            Mapper.Map<Comment, CommentDto>(newComment, item);
            return CreatedAtRoute("Tables", new { id = item.Id }, item);
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