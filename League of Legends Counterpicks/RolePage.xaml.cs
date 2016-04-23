using League_of_Legends_Counterpicks.Advertisement;
using League_of_Legends_Counterpicks.Common;
using League_of_Legends_Counterpicks.DataModel;
using Microsoft.AdMediator.WindowsPhone81;
using Microsoft.Advertising.WinRT.UI;
using QKit;
using QKit.JumpList;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Hub Application template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace League_of_Legends_Counterpicks
{
    public sealed partial class RolePage : Page
    {
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        private readonly String APP_ID = "3366702e-67c7-48e7-bc82-d3a4534f3086";
        private Champions champions;
        private bool isInitialAllView = true;

        public RolePage()
        {
            this.NavigationCacheMode = NavigationCacheMode.Disabled;
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            DefaultViewModel["AdVisibility"] = App.licenseInformation.ProductLicenses["AdRemoval"].IsActive ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)  //e is the unique ID
        {
            App.IsInternetAvailable();

            CreateAdUnits();

            reviewApp();

            // Set up champion data 
            champions = await StatsDataSource.GetChampionsAsync();
            if (champions == null) {
                MessageDialog messageBox = new MessageDialog("Make sure your internet connection is working and try again!");
                await messageBox.ShowAsync();
                Application.Current.Exit();
            }

            var GroupedChampions = champions.ChampionInfos.ToAlphaGroups(x => x.Value.Name);
            DefaultViewModel["GroupedChampions"] = GroupedChampions;

            // Set up roles
            string selectedRole = (string)e.NavigationParameter;
            DefaultViewModel["Roles"] = StatsDataSource.GetRoles();
            DefaultViewModel["SelectedRole"] = selectedRole;


            // Configure view to either 'All' or filtered role
            if (selectedRole != "All")
            {
                // Do a null check incase of race condition with UI, and raise a flag incase UI hasn't loaded yet for its load to set visibility
                isInitialAllView = false;
                if (JumpList != null)
                    JumpList.Visibility = Visibility.Collapsed;

                DefaultViewModel["Filter"] = champions.ChampionInfos.Where(x => x.Value.Tags[0] == selectedRole).OrderBy(x => x.Value.Name);

                if (FilterGridView != null)
                    FilterGridView.Visibility = Visibility.Visible;
            }
        }
        
        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        /// <summary>
        /// Shows the details of an item clicked on in the <see cref="ChampionPage"/>
        /// </summary>
        /// <param name="sender">The GridView displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        /// 
        private async void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            //Ask user to purchase ad removal before proceeding
            checkAdRemoval();
            var championKey = ((KeyValuePair<string, ChampionInfo>)e.ClickedItem).Key;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Frame.Navigate(typeof(ChampionPage), championKey));
        }

        private void ChampImage_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            (sender as Image).Opacity = 0.5;
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();
     
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            AdGrid.Children.Clear();
            AdGrid2.Children.Clear();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();

            base.OnNavigatingFrom(e);
        }
        #endregion

        private void JumpList_Loaded(object sender, RoutedEventArgs e)
        {
            var jumpList = sender as AlphaJumpList;
            var gridParent = jumpList.Parent as Grid;

            // JumpList may not render properly the first time, so blast it a bit
            gridParent.Visibility = Visibility.Collapsed; gridParent.Visibility = Visibility.Visible;
            gridParent.Visibility = Visibility.Collapsed; gridParent.Visibility = Visibility.Visible;
            jumpList.Visibility = Visibility.Collapsed; jumpList.Visibility = Visibility.Visible;

            if (!isInitialAllView)
                (sender as AlphaJumpList).Visibility = Visibility.Collapsed;
        }

        // If user did not select all from main page, show the filtered role grid 
        private void FilterGridView_Loaded(object sender, RoutedEventArgs e)
        {
            if (!isInitialAllView)
                (sender as Grid).Visibility = Visibility.Visible;
        }

        // Helper method to show the role into view, enabling the role name textblock and emptying the filter box
        private void Show_RoleView() {
            RoleName.Visibility = Visibility.Visible;
            FilterBox.TextChanged -= FilterBox_TextChanged; // Prevent TextChanged event from firing after programatic text change 
            FilterBox.Text = "Type to Search";
            var selectedRole = (string)RoleFlyout.SelectedItem;

            if (selectedRole == "All")
            {
                JumpList.Visibility = Visibility.Visible;
                FilterGridView.Visibility = Visibility.Collapsed;
            }
            else
            {
                JumpList.Visibility = Visibility.Collapsed;
                DefaultViewModel["Filter"] = champions.ChampionInfos.Where(x => x.Value.Tags[0] == selectedRole).OrderBy(x => x.Value.Name);
                FilterGridView.Visibility = Visibility.Visible;
            }

            FilterBox.TextChanged += FilterBox_TextChanged; // Re-bind back the event handler
        }
        private void Role_Picked(ListPickerFlyout sender, ItemsPickedEventArgs args)
        {
            Show_RoleView();
        }

        private void FilterBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Set view to filter grid view of the query (Show nothing if nothing searched)
            String text = (sender as TextBox).Text;
            if (FilterBox.Text != String.Empty && champions != null && champions.ChampionInfos != null)
                DefaultViewModel["Filter"] = champions.ChampionInfos.Where(x => x.Value.Name.ToLower().StartsWith(text.ToLower())).OrderBy(x => x.Value.Name);
            else
                DefaultViewModel["Filter"] = null;
        }

        private void FilterBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // Clear role name when filter box is clicked, and show nothing until searched 
            var filterBox = sender as TextBox;
            (sender as TextBox).Text = String.Empty;
            RoleName.Visibility = Visibility.Collapsed;
            JumpList.Visibility = Visibility.Collapsed;
            FilterGridView.Visibility = Visibility.Visible;
            DefaultViewModel["Filter"] = null;
        }

        private void FilterBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // If query is still empty after losing focus, set the role back into view 
            if (((TextBox)sender).Text == String.Empty)
            {
                Show_RoleView();
            }
        }

        private async void reviewApp()
        {
            if (!localSettings.Values.ContainsKey("Views"))
                localSettings.Values.Add(new KeyValuePair<string, object>("Views", 3));
            else
                localSettings.Values["Views"] = 1 + Convert.ToInt32(localSettings.Values["Views"]);

            int viewCount = Convert.ToInt32(localSettings.Values["Views"]);


            //Only ask for review up to several times, once every 4 times this page is visited, and do not ask anymore once reviewed
            if (viewCount % 4 == 0 && viewCount <= 50 && Convert.ToInt32(localSettings.Values["Rate"]) != 1)
            {
                var reviewBox = new MessageDialog("Keep updates coming by rating this app 5 stars to support us!");
                reviewBox.Commands.Add(new UICommand { Label = "Yes! :)", Id = 0 });
                reviewBox.Commands.Add(new UICommand { Label = "Maybe later", Id = 1 });

                try
                {
                    var reviewResult = await reviewBox.ShowAsync();
                    if ((int)reviewResult.Id == 0)
                    {
                        try { await Launcher.LaunchUriAsync(new Uri("ms-windows-store:reviewapp?appid=" + APP_ID)); }
                        catch (Exception) { }
                        localSettings.Values["Rate"] = 1;
                    }
                }
                catch (Exception) { }
            }
        }
        private async void checkAdRemoval()
        {
            if (!App.licenseInformation.ProductLicenses["AdRemoval"].IsActive)
            {
                if (!localSettings.Values.ContainsKey("AdViews"))
                    localSettings.Values.Add(new KeyValuePair<string, object>("AdViews", 3));
                else
                    localSettings.Values["AdViews"] = 1 + Convert.ToInt32(localSettings.Values["AdViews"]);

                int viewCount = Convert.ToInt32(localSettings.Values["AdViews"]);

                //Only ask for IAP purchase up to several times, once every 25 times this page is visited, and do not ask anymore once bought
                if (viewCount % 25 == 0 && viewCount <= 100)
                {
                    var purchaseBox = new MessageDialog("See more counters, easy matchups and synergy picks for each champion at once! Remove ads now!");
                    purchaseBox.Commands.Add(new UICommand { Label = "Yes! :)", Id = 0 });
                    purchaseBox.Commands.Add(new UICommand { Label = "Maybe later :(", Id = 1 });

                    try
                    {
                        var reviewResult = await purchaseBox.ShowAsync();

                        if ((int)reviewResult.Id == 0)
                        {
                            AdRemover.Purchase();
                        }
                    }
                    catch (Exception) { }
                }
            }
        }

        private void Ad_Error(object sender, AdErrorEventArgs e)
        {

        }

        private async void test(object sender, RoutedEventArgs e)
        {
            // Gets the app's current memory usage   
            ulong AppMemoryUsageUlong = MemoryManager.AppMemoryUsage;

            // Gets the app's memory usage limit   
            ulong AppMemoryUsageLimitUlong = MemoryManager.AppMemoryUsageLimit;

            AppMemoryUsageUlong /= 1024 * 1024;
            AppMemoryUsageLimitUlong /= 1024 * 1024;
            string test = "App memory uage - " + AppMemoryUsageUlong.ToString();
            test += "\nApp memory usage limit - " + AppMemoryUsageLimitUlong.ToString();
            var message = new MessageDialog(test);
            await message.ShowAsync();
        }
        private void CreateAdUnits()
        {
            if (App.licenseInformation.ProductLicenses["AdRemoval"].IsActive)
                return;

            int count = 42;
            var limitMb = MemoryManager.AppMemoryUsageLimit / (1024 * 1024);
            if (limitMb < 200)
            {
                count = 19;
            }
            else if (limitMb > 700)
            {
                count = 60;
            }

            for (int i = 0; i < count; ++i)
            {
                AdControl ad = new AdControl();
                ad.ApplicationId = "bf747944-c75c-4f2a-a027-7c159b32261d";
                ad.AdUnitId = "295675";
                ad.Style = Application.Current.Resources["HorizontalAd"] as Style;
                ad.IsAutoRefreshEnabled = false;
                ad.Refresh();
                ad.IsAutoRefreshEnabled = true;
                ad.AutoRefreshIntervalInSeconds = 30;
                AdGrid.Children.Add(ad);
            }
        }
    }
}
