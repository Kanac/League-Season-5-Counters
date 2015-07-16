using Microsoft.WindowsAzure.Mobile.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LeagueSeason5CountersService.CustomEntityData;

namespace LeagueSeason5CountersService.DataObjects
{
    public class ChampionFeedback : CustomEntity
    {
        public ChampionFeedback(){
            Comments = new List<Comment>();
        }
        public string Name { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}