using Microsoft.WindowsAzure.Mobile.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LeagueSeason5CountersService.CustomEntityData;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeagueSeason5CountersService.DataObjects
{
    public class ChampionFeedback : CustomEntity
    {
        public ChampionFeedback(){
            Comments = new List<Comment>();
            Counters = new List<Counter>();
        }
        [Index(IsUnique = true)]
        [StringLength(200)]
        public string Name { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Counter> Counters { get; set; }
    }
}