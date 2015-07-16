using Microsoft.WindowsAzure.Mobile.Service;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace LeagueSeason5CountersService.DataObjects
{
    public class UserRatingDto
    {
        public string Id { get; set; }
        public string UniqueUser { get; set; }
        public int Score { get; set; }
        public int CommentId { get; set; }

    }
}