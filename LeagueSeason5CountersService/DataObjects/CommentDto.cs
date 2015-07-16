using Microsoft.WindowsAzure.Mobile.Service;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace LeagueSeason5CountersService.DataObjects
{
    public class CommentDto
    {
        //Comment Properties
        public string Id { get; set; }
        public string Text { get; set; }
        public string User { get; set; }
        public int Score { get; set; }
        
        //ChampionFeedback properties
        public string ChampionFeedbackId { get; set; }
        public string ChampionFeedbackName { get; set; }

        //Navigation property to UserRatings
        public  ICollection<UserRatingDto> UserRatings { get; set; }
    }
}