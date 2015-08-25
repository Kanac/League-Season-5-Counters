using League_of_Legends_Counterpicks.DataModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using System.Linq;

namespace League_of_Legends_Counterpicks.Converters
{
    public class StatsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            List<Matchup> counters = (List<Matchup>)value;

            if (parameter as string == "Counter")
                return counters.Where(c => c.WinRate < 50 && c.Games > 100).OrderBy(x => x.WinRate);
            else
                return counters.Where(c => c.WinRate > 50 && c.Games > 100).OrderByDescending(x => x.WinRate);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
