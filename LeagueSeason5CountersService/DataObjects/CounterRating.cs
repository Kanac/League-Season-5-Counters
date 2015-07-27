using Microsoft.WindowsAzure.Mobile.Service;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using LeagueSeason5CountersService.CustomEntityData;

namespace LeagueSeason5CountersService.DataObjects
{
    public class CounterRating : CustomEntity
    {
        public string UniqueUser { get; set; }
        public int Score { get; set; }
        public string CounterId { get; set; }
        [ForeignKey("CounterId")]
        public virtual Counter Counter { get; set; }
    }
}