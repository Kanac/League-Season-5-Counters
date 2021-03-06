﻿using League_of_Legends_Counterpicks.DataModel;
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
        public static void CreateAdUnits(int id, string resource, Grid grid, int count)
        {
            if (App.licenseInformation.ProductLicenses["AdRemoval"].IsActive)
                return;

            count = 5;

            for (int i = 0; i < count; ++i)
            {
                CreateSingleAdUnit(id, resource, grid);
            }
        }

        public static void CreateSingleAdUnit(int id, string resource, Grid grid)
        {
            AdControl ad = new AdControl();
            ad.ApplicationId = App.AppId;
            ad.AdUnitId = id.ToString();
            ad.Style = Application.Current.Resources[resource] as Style;
            ad.IsAutoRefreshEnabled = false;
            ad.Refresh();
            ad.IsAutoRefreshEnabled = true;
            ad.AutoRefreshIntervalInSeconds = 30;
            grid.Children.Add(ad);
        }
    }
}
