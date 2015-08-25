using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace League_of_Legends_Counterpicks.Converters
{
    class CommentFormat : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string champName = value as string;
            string param = parameter as string;

            return (param == "Counter") ? "Countering " + champName : "Playing as " + champName;
     
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
