using League_of_Legends_Counterpicks.DataModel;
using Microsoft.Advertising.WinRT.UI;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace League_of_Legends_Counterpicks.Helper
{
    public static class HelperMethods
    {
        public static string appId = "bf747944-c75c-4f2a-a027-7c159b32261d";

        public static void CreateAdUnits(int id, string appId, Grid grid, int count)
        {
            if (App.licenseInformation.ProductLicenses["AdRemoval"].IsActive)
                return;

            for (int i = 0; i < count; ++i)
            {
                CreateSingleAdUnit(id, appId, grid);
            }
        }

        public static void CreateSingleAdUnit(int id, string appId, Grid grid)
        {
            AdControl ad = new AdControl();
            ad.ApplicationId = appId;
            ad.AdUnitId = id.ToString();
            ad.Style = Application.Current.Resources["HorizontalAdSmall"] as Style;
            ad.IsAutoRefreshEnabled = false;
            ad.Refresh();
            ad.IsAutoRefreshEnabled = true;
            ad.AutoRefreshIntervalInSeconds = 30;
            grid.Children.Add(ad);
        }
    }
}
