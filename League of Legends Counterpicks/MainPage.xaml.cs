using League_of_Legends_Counterpicks.Common;
using League_of_Legends_Counterpicks.Data;
using Microsoft.Advertising.Mobile.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
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
using Windows.ApplicationModel.Background;
using System.IO.Compression;
using Windows.ApplicationModel.Email;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store;
using League_of_Legends_Counterpicks.Advertisement;

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
        private List<String> suggestions;
        private Boolean firstLoad = true;

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
            suggestions = new List<String>(roles[0].Champions.Select(x => x.UniqueId)); //Get all the champion names and place it in a list
            this.DefaultViewModel["Roles"] = roles;     //Which in turn is the data context for the hub page, with only one thing as a whole in it

            //Toast background task setup 
            if (firstLoad)
            {
                firstLoad = false;
                CheckAppVersion();
                var toastTaskName = "ToastBackgroundTask";
                var taskRegistered = false;

                foreach (var task in Windows.ApplicationModel.Background.BackgroundTaskRegistration.AllTasks)
                {
                    if (task.Value.Name == toastTaskName)
                    {
                        taskRegistered = true;
                        break;
                    }
                }

                if (!taskRegistered)
                {
                    await Windows.ApplicationModel.Background.BackgroundExecutionManager.RequestAccessAsync();
                    var builder = new BackgroundTaskBuilder();
                    builder.Name = toastTaskName;
                    builder.TaskEntryPoint = "Tasks.ToastBackground";
                    var hourlyTrigger = new TimeTrigger(30, false);
                    builder.SetTrigger(hourlyTrigger);

                    BackgroundTaskRegistration task = builder.Register();
                }
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



        private async void Share_Clicked(object sender, RoutedEventArgs e)
        {
            EmailMessage mail = new EmailMessage();
            mail.Subject = "Amazing League of Legends App";
            String body = "Try this free app out to help you climb elo easily in League of Legends@https://www.windowsphone.com/en-us/store/app/league-season-5-counters/3366702e-67c7-48e7-bc82-d3a4534f3086";
            body = body.Replace("@", "\n");
            mail.Body = body;
            await EmailManager.ShowComposeNewEmailAsync(mail);
        }

        private void ChangeLog_Clicked(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ChangeLog));
        }


        private async void CheckAppVersion()
        {
            String appVersion = String.Format("{0}.{1}.{2}.{3}",
                    Package.Current.Id.Version.Build,
                    Package.Current.Id.Version.Major,
                    Package.Current.Id.Version.Minor,
                    Package.Current.Id.Version.Revision);

            String lastVersion = Windows.Storage.ApplicationData.Current.LocalSettings.Values["AppVersion"] as String;

            if (lastVersion == null || lastVersion != appVersion)
            {
                // Our app has been updated
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["AppVersion"] = appVersion;

                // Call RemoveAccess
                BackgroundExecutionManager.RemoveAccess();
            }

            BackgroundAccessStatus status = await BackgroundExecutionManager.RequestAccessAsync();
        }

        private void Tweet(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Twitter));
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(RolePage), "Filter");
        }


        private async void Comment_Click(object sender, RoutedEventArgs e)
        {
            EmailRecipient sendTo = new EmailRecipient() { Address = "testgglol@outlook.com" };
            EmailMessage mail = new EmailMessage();

            mail.Subject = "Feedback for League Season 5 Counters";
            mail.To.Add(sendTo);
            await EmailManager.ShowComposeNewEmailAsync(mail);
        }

        private void Ad_Loaded(object sender, RoutedEventArgs e)
        {
       
            var ad = sender as AdControl;
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

        private void AdBlock_Click(object sender, RoutedEventArgs e)
        {
            AdRemover.Purchase();
        }


  
    }
}
