using LeagueSeason5CountersService.CustomEntityData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeagueSeason5CountersService.DataObjects
{
    public class AdUnit : CustomEntity
    {
        public int Ad { get; set; }
        public string App { get; set; }
        public DateTime LastUseddate { get; set; }
        public bool InUse { get; set; }
    }
}