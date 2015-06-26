using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace League_of_Legends_Counterpicks.Converters
{
    public class FilterFormat : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            String filter = value as String;
            return "Filter for " + filter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
