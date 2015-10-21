using Microsoft.WindowsAzure.Mobile.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LeagueSeason5CountersService.CustomEntityData;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeagueSeason5CountersService.DataObjects
{
    public class Counter : CustomEntity
    {
        public Counter() {
            CounterRatings = new List<CounterRating>();
            CounterComments = new List<CounterComment>();
        }
        public string Name { get; set; }
        public int Score { get; set; }
        public string ChampionFeedbackName { get; set; }
        public string ChampionFeedbackId { get; set; }
        public PageEnum.ChampionPage Page { get; set; }
        [ForeignKey("ChampionFeedbackId")]
        public virtual ChampionFeedback ChampionFeedback { get; set; }
        public virtual ICollection<CounterRating> CounterRatings { get; set; }
        public virtual ICollection<CounterComment> CounterComments { get; set; }
    }
}