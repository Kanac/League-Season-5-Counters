using League_of_Legends_Counterpicks.DataModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using System.Linq;
using League_of_Legends_Counterpicks.Common;

namespace League_of_Legends_Counterpicks.Converters
{
    class EmptyMessageVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var defaultViewModel = value as ObservableDictionary;
            var loadingVisibility = defaultViewModel["LoadingVisibility"];

            // Ensure data has finished loading before checking results 
            if (loadingVisibility != null && (Visibility)loadingVisibility == Visibility.Collapsed)
            {
                string param = (string)parameter;
                if (param == "Synergy")
                {
                    var synergyChampions = ((ChampionFeedback)defaultViewModel["ChampionFeedback"]).Counters.Where(x => x.Page == PageEnum.ChampionPage.Synergy);
                    if (synergyChampions.Count() == 0)
                        return Visibility.Visible;
                }
                else if (param == "CounterComments")
                {
                    var counterComments = ((ChampionFeedback)defaultViewModel["ChampionFeedback"]).Comments.Where(x => x.Page == PageEnum.CommentPage.Counter);
                    if (counterComments.Count() == 0)
                        return Visibility.Visible;
                }

                else if (param == "PlayingAsComments")
                {
                    var playingAsComments = ((ChampionFeedback)defaultViewModel["ChampionFeedback"]).Comments.Where(x => x.Page == PageEnum.CommentPage.Playing);
                    if (playingAsComments.Count() == 0)
                        return Visibility.Visible;
                }
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
