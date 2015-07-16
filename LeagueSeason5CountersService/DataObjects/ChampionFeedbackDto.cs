using Microsoft.WindowsAzure.Mobile.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeagueSeason5CountersService.DataObjects
{
    public class ChampionFeedbackDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ICollection<CommentDto> Comments { get; set; }
    }
}