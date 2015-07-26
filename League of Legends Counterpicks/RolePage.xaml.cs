using League_of_Legends_Counterpicks.Common;
using League_of_Legends_Counterpicks.Data;
using Microsoft.AdMediator.WindowsPhone81;
using QKit;
using QKit.JumpList;
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
        private ObservableCollection<Role> roles;
        private string roleId, savedRoleId;
        private bool firstLoad = true;
        private int sectionIndex;

        public RolePage()
        {
            this.InitializeComponent();
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
            //Check if the navigation paramater changes to determine the roleId to use, otherwise use the roleId set by navigating to ChampionPage
            if ((string)e.NavigationParameter != savedRoleId) {
                roleId = e.NavigationParameter as string;
                savedRoleId = roleId;
            }

            roles = await DataSource.GetRolesAsync();  //Gets a reference to all the roles -- no data is seralized again (already done on bootup)
            var allRole = roles[0]; //Gets the all role 
            var GroupedChampions = JumpListHelper.ToAlphaGroups(allRole.Champions, x => x.UniqueId);
            allRole.GroupedChampions = GroupedChampions;
            DefaultViewModel["Roles"] = roles;
            //Smoothes out the loading process to get to desired page immedaitely 
            if (roleId == "Filter")
                sectionIndex = MainHub.Sections.Count() - 1;
            else
                sectionIndex = roles.IndexOf(DataSource.GetRole(roleId));
            if (firstLoad)
                MainHub.DefaultSectionIndex = sectionIndex;
            else
                MainHub.ScrollToSection(MainHub.Sections.ElementAt(sectionIndex));
        }
        
    


            //Data context set to the individual role (dictionary key to value)
        

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
        /// Shows the details of an item clicked on in the <see cref="ChampionPage"/>
        /// </summary>
        /// <param name="sender">The GridView displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            //Store the hub section before proceeding to champion page, so that the back button goes back to it
            roleId = MainHub.SectionsInView[0].Name;

            var championId = ((Champion)e.ClickedItem).UniqueId;
            if (!Frame.Navigate(typeof(ChampionPage), championId))
            {
                var resourceLoader = ResourceLoader.GetForCurrentView("Resources");
                throw new Exception(resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }

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

        #endregion

        //Purpose of this is to load the role page twice since there is a bug with the Qkit jumpList that doesnt render properly on first load
        private void JumpList_Loaded(object sender, RoutedEventArgs e)
        {
            if (firstLoad){
                firstLoad = false;
                var jumpList = sender as AlphaJumpList;
                jumpList.Visibility = Windows.UI.Xaml.Visibility.Visible;
                All.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                All.Visibility = Windows.UI.Xaml.Visibility.Visible;
                MainHub.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                MainHub.Visibility = Windows.UI.Xaml.Visibility.Visible;
                jumpList.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                jumpList.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            String text = (sender as TextBox).Text;
            Role filter = DataSource.FilterChampions(text);
            if (String.IsNullOrEmpty(text))    //Don't show every champion with no query
                DefaultViewModel["Filter"] = null;
            else
                DefaultViewModel["Filter"] = filter;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).Text = String.Empty;
        }
    
  
    }
}
