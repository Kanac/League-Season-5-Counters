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
    class RoleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Champions champions = value as Champions;
            if (champions.ChampionInfos == null)
                return null;

            string role = parameter as string;
            return champions.ChampionInfos.Where(x => x.Value.Tags[0] == role).OrderBy(x => x.Value.Name);

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
