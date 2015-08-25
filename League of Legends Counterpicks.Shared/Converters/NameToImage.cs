using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;
using League_of_Legends_Counterpicks.DataModel;
using System.Threading.Tasks;
using System.Linq;

namespace League_of_Legends_Counterpicks.Converters
{
    public class NameToImage : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var key = value as string;
            var param = parameter as string;

            if (param == "Name") {
                var champions = StatsDataSource.GetChampions();
                key = champions.ChampionInfos.Where(x => x.Value.Name == key).FirstOrDefault().Key;
            }

            var uri = "ms-appx:///Assets/" + key + "_Square_0.png";
            return uri;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
