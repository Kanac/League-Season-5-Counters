using System;
using System.Collections.Generic;
using System.Text;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store;
using Windows.Storage;

namespace League_of_Legends_Counterpicks.Advertisement
{
    class AdRemover
    {
        public static async void Purchase()
        {
            if (!App.licenseInformation.ProductLicenses["AdRemoval"].IsActive)
            {
                try
                {
                    //StorageFolder installFolder = await Package.Current.InstalledLocation.GetFolderAsync("Assets");
                    //StorageFile appSimulatorStorageFile = await installFolder.GetFileAsync("WindowsStoreProxy.xml");

                    //await CurrentAppSimulator.ReloadSimulatorAsync(appSimulatorStorageFile);
                    //PurchaseResults result = await CurrentAppSimulator.RequestProductPurchaseAsync("AdRemoval");

                    PurchaseResults result = await CurrentApp.RequestProductPurchaseAsync("AdRemoval");
                }
                catch (Exception ex)
                {
                    // handle error or do nothing
                }
            }
        }

    }
}
