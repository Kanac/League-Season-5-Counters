using League_of_Legends_Counterpicks.Common;
using League_of_Legends_Counterpicks.Data;
using Microsoft.Advertising.WinRT.UI.XamlAdControl_XamlTypeInfo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Hub Application template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace League_of_Legends_Counterpicks
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");

        public MainPage()
        {
            this.InitializeComponent();

            // Hub is only supported in Portrait orientation
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;

            this.NavigationCacheMode = NavigationCacheMode.Required;

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
            get { return this.defaultViewModel; }       //Default view model contains all the groups 
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
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // TODO: Create an appropriate data model for your problem domain to replace the sample data
            var roles = await DataSource.GetRolesAsync() ;     //On loading the hub page, add all the groups to the default view model
            this.DefaultViewModel["Roles"] = roles;     //Which in turn is the data context for the hub page, with only one thing as a whole in it
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
            // TODO: Save the unique state of the page here.
        }

        /// <summary>
        /// Shows the details of a clicked group in the <see cref="RolePage"/>.
        /// </summary>
        private void GroupSection_ItemClick(object sender, ItemClickEventArgs e)
        {
            var roleId = ((Role)e.ClickedItem).UniqueId;            //Get the ID of the group clicked in the first page that contains all the groups

            if (!Frame.Navigate(typeof(RolePage), roleId))          //Navigate to the group clicked, to the page with its items
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));     //Boolean frame.navigate is return type, method is still done in navigation
            }
        }

        /// <summary>
        /// Shows the details of an item clicked on in the <see cref="ChampionPage"/>
        /// </summary>
        //private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        //{
        //    // Navigate to the appropriate destination page, configuring the new page
        //    // by passing required information as a navigation parameter
        //    var itemId = ((SampleDataItem)e.ClickedItem).UniqueId;              // Above ^, except for individual item page
        //    if (!Frame.Navigate(typeof(ItemPage), itemId))  
        //    {
        //        throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
        //    }
        //}

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

        #endregion

        private void FilterTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
        }

        private void FilterTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            FilterTextBox.Text = string.Empty;
        }

        private void FilterTextBox_Enter(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                string filter = FilterTextBox.Text;
                var group = DataSource.FilterChampions(filter);

                if (group.Champions.Count == 1)
                    Frame.Navigate(typeof(ChampionPage), group.Champions[0].UniqueId);
                else
                    Frame.Navigate(typeof(RolePage), group);    
            }

        }




  
    }
}
