using Microsoft.WindowsAzure.Mobile.Service;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using LeagueSeason5CountersService.CustomEntityData;
namespace LeagueSeason5CountersService.DataObjects

{
    public class CounterCommentDto : CustomEntity
    {
        public string Text { get; set; }
        public string User { get; set; }
        public int Score { get; set; }
        public string CounterId { get; set; }
        public string ChampionFeedbackName { get; set; }
        public string CounterName { get; set; }
        public virtual ICollection<CounterCommentRatingDto> CounterCommentRatings { get; set; }

    }
}