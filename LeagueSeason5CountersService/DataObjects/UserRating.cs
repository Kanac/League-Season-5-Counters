using Microsoft.WindowsAzure.Mobile.Service;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using LeagueSeason5CountersService.CustomEntityData;

namespace LeagueSeason5CountersService.DataObjects
{
    public class UserRating : CustomEntity
    {
        public string UniqueUser { get; set; }
        public int Score { get; set; }
        public string CommentId { get; set; }
        [ForeignKey("CommentId")] 
        public virtual Comment Comment { get; set; }
    }
}