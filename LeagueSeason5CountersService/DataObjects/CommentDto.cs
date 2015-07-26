using Microsoft.WindowsAzure.Mobile.Service;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using LeagueSeason5CountersService.CustomEntityData;
using System.ComponentModel.DataAnnotations;

namespace LeagueSeason5CountersService.DataObjects
{
    public class CommentDto : CustomEntity
    {
        public string Text { get; set; }
        public string User { get; set; }
        public int Score { get; set; }
        public PageEnum.Page Page { get; set; }
        public string ChampionFeedbackName { get; set; }
        public string ChampionFeedbackId { get; set; }
        public virtual ICollection<UserRatingDto> UserRatings { get; set; }

    }
}