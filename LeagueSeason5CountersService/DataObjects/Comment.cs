using Microsoft.WindowsAzure.Mobile.Service;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using LeagueSeason5CountersService.CustomEntityData;
namespace LeagueSeason5CountersService.DataObjects

{
    public class Comment : CustomEntity
    {
        public Comment() {
            UserRatings = new List<UserRating>();
        }
        public string Text { get; set; }
        public string User { get; set; }
        public int Score { get; set; }
        public PageEnum.Page Page { get; set;}
        public string ChampionFeedbackName { get; set; }
        public string ChampionFeedbackId { get; set; }
        [ForeignKey("ChampionFeedbackId")] 
        public virtual ChampionFeedback ChampionFeedback { get; set; }
        public virtual ICollection<UserRating> UserRatings { get; set; }

        
    }
}