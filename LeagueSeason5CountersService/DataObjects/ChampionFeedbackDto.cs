using LeagueSeason5CountersService.CustomEntityData;
using Microsoft.WindowsAzure.Mobile.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeagueSeason5CountersService.DataObjects
{
    public class ChampionFeedbackDto : CustomEntity
    {
        public string Name { get; set; }
        public virtual ICollection<CommentDto> Comments { get; set; }
    }
}