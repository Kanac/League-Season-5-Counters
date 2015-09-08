using League_of_Legends_Counterpicks.Advertisement;
using League_of_Legends_Counterpicks.Common;
using League_of_Legends_Counterpicks.Data;
using League_of_Legends_Counterpicks.DataModel;
using Microsoft.Advertising.WinRT.UI;
using QKit;
using QKit.JumpList;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources;
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
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
        private List<AdControl> adList = new List<AdControl>();
        private Champions champions;
        private DispatcherTimer dispatcherTimer;
        private bool isInitialAllView = true;

        public RolePage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
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
            // Check for internet connection
            App.IsInternetAvailable();
            
            // Re-sync the timer if page is refreshed (ad will load again - set timer back to 0)
            if (dispatcherTimer != null)
                dispatcherTimer.Stop();

            reviewApp();

            // Set up champion data 
            champions = await StatsDataSource.GetChampionsAsync();
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

            setupAdTimer();
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
        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            //Ask user to purchase ad removal before proceeding
            checkAdRemoval();
            var championKey = ((KeyValuePair<string, ChampionInfo>)e.ClickedItem).Key;
            Frame.Navigate(typeof(ChampionPage), championKey);
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
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (dispatcherTimer != null)
                dispatcherTimer.Stop();

            AdGrid.Children.Clear();
            AdGrid2.Children.Clear();
            adList.Clear();

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

        private void FilterGridView_Loaded(object sender, RoutedEventArgs e)
        {
            if (!isInitialAllView)
                (sender as Grid).Visibility = Visibility.Visible;
        }

        // Helper method to show the role into view, enabling the role name textblock and emptying the filter box
        private void Show_RoleView() {
            RoleName.Visibility = Visibility.Visible;
            FilterBox.Text = String.Empty;
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
        }
        private void Role_Picked(ListPickerFlyout sender, ItemsPickedEventArgs args)
        {
            // Set role into view 
            Show_RoleView();
        }

        private void FilterBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Set view to filter grid view of the query (Show nothing if nothing searched)
            if (RoleName.Visibility == Visibility.Collapsed) // Don't call this if text was programmatically changed
            {
                String text = (sender as TextBox).Text;
                if (FilterBox.Text != String.Empty)
                    DefaultViewModel["Filter"] = champions.ChampionInfos.Where(x => x.Value.Name.ToLower().StartsWith(text.ToLower())).OrderBy(x => x.Value.Name);
                else
                    DefaultViewModel["Filter"] = null;
            }
        }

        private void FilterBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // Clear role name when filter box is clicked, and show nothing until searched 
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


        private void Search_Click(object sender, RoutedEventArgs e)
        {
            if (FilterBox != null)
                FilterBox.Focus(FocusState.Programmatic);
        }

        private void Ad_Loaded(object sender, RoutedEventArgs e)
        {
            var ad = sender as AdControl;
            // Check if the ad list already has a reference to this ad before inserting
            if (adList.Where(x => x.AdUnitId == ad.AdUnitId).Count() == 0)
                adList.Add(ad);
         
            if (App.licenseInformation.ProductLicenses["AdRemoval"].IsActive)
            {
                // Hide the app for the purchaser
                ad.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                // Otherwise show the ad
                ad.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
        }

        private void setupAdTimer()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 33);
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, object e)
        {
            foreach (var ad in adList)
                ad.Refresh();
        }


        private void GridAd_Loaded(object sender, RoutedEventArgs e)
        {
            var grid = sender as Grid;
            if (App.licenseInformation.ProductLicenses["AdRemoval"].IsActive)
            {
                var rowDefinitions = grid.RowDefinitions;
                foreach (var r in rowDefinitions)
                {
                    if (r.Height.Value == 80)
                    {
                        r.SetValue(RowDefinition.HeightProperty, new GridLength(0));
                    }
                }
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

        
    }
}
