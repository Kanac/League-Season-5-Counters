﻿using League_of_Legends_Counterpicks.Common;
using League_of_Legends_Counterpicks.DataModel;
using Microsoft.AdMediator.WindowsPhone81;
using System;
using System.Linq;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Email;
using Windows.ApplicationModel;
using League_of_Legends_Counterpicks.Advertisement;
using League_of_Legends_Counterpicks.Helper;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Microsoft.Advertising.WinRT.UI;
using System.Collections.Generic;
using Windows.System;
using Windows.UI.Xaml.Data;

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
        private Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
  
        public MainPage()
        {
            this.NavigationCacheMode = NavigationCacheMode.Disabled;
            this.InitializeComponent();
        
            // Hub is only supported in Portrait orientation
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            DefaultViewModel["AdVisibility"] = App.licenseInformation.ProductLicenses["AdRemoval"].IsActive ? Visibility.Collapsed : Visibility.Visible;

            this.RequestedTheme = ElementTheme.Dark;
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
            // Check for internet connection
            App.IsInternetAvailable();

            int id = await AdData.GetAdId();
            int count = MemoryManager.AppMemoryUsageLimit / (1024 * 1024) > 700 ? 5 : 4;
            HelperMethods.CreateSingleAdUnit(id, "HorizontalAdSmall", AdGrid);
            HelperMethods.CreateAdUnits(id, "HorizontalAdSmall", AdGrid2, count);

            // Load all the roles (which contains all the champions) from the json file 
            var roles = StatsDataSource.GetRoles();
            this.DefaultViewModel["Roles"] = roles;
            await StatsDataSource.GetChampionsAsync();

            // Toast background task setup 
            if (e.PageState == null || (bool)e.PageState["firstLoad"] == true)
            {
                CheckAppVersion();
                string version = localSettings.Values["AppVersion"] as string;

                // Only show imminent toasts up to 5 times that the app is launched 
                if (localSettings.Values[version] == null)
                    localSettings.Values[version] = 0;

                if ((int)localSettings.Values[version] < 5)
                {
                    setupFeatureToast(); // Flashes a new feature message 
                    setupReuseToast(50); // Creates a message indicating user to reuse app after 50 minutes of opening
                    setupReuseToast(50*8); // Creates a message indicating user to reuse app after 8 hours of opening
                    localSettings.Values[version] = 1 + (int)localSettings.Values[version];
                }
                await setupToast();  // Background toast in 50 hours talking about new champion data
            }
        }

        private void setupFeatureToast()
        {
            ToastTemplateType toastTemplate = ToastTemplateType.ToastImageAndText02;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

            XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[1].AppendChild(toastXml.CreateTextNode("Season 6 Ivern data has arrived!"));

            ToastNotification toast = new ToastNotification(toastXml);
            toast.Tag = "FeatureToast";
            ToastNotificationManager.History.Remove("FeatureToast"); // Remove previous toasts in action centre history, if any, before sending 
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        private void setupReuseToast(int min)
        {
            // Check if a reuse toast is already scheduled
            if (ToastNotificationManager.CreateToastNotifier().GetScheduledToastNotifications().Where(x => x.Id == "Reuse").Count() > 1)
            {
                return;
            }

            ToastTemplateType toastTemplate = ToastTemplateType.ToastImageAndText02;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

            XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode("League of Legends"));
            Random random = new Random();
            int val = random.Next(3);
            if (val == 0)
            {
                toastTextElements[1].AppendChild(toastXml.CreateTextNode("New Champion data has been uploaded!"));
            }
            else if (val == 1)
            {
                toastTextElements[1].AppendChild(toastXml.CreateTextNode("In Champ Select, reference this app for your advantage!"));
            }
            else
            {
                toastTextElements[1].AppendChild(toastXml.CreateTextNode("Users have submitted new data to this app!"));
            }

            IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            XmlElement audio = toastXml.CreateElement("audio");
            //audio.SetAttribute("src", "ms-appx:///Assets/yourturn.mp3");
            toastNode.AppendChild(audio);

            ToastNotification toast = new ToastNotification(toastXml);
            DateTime dueTime = DateTime.Now.AddMinutes(min);
            ScheduledToastNotification scheduledToast = new ScheduledToastNotification(toastXml, dueTime);
            scheduledToast.Id = "Reuse";
            ToastNotificationManager.CreateToastNotifier().AddToSchedule(scheduledToast);
        }

        private async Task setupToast()
        {
            CheckAppVersion();
            var toastTaskName = "ToastBackTask";
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
            e.PageState["firstLoad"] = false;
        }

        private void Role_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(RolePage), (sender as Button).Tag);
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
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();

            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            // Terminate the ads to clear from memory immediately rather than relying on garbage collector 
            AdGrid.Children.Clear();
            AdGrid2.Children.Clear();
            base.OnNavigatingFrom(e);

        }

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
    
        
        private void Tweet(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Twitter));
        }

        private async void Comment_Click(object sender, RoutedEventArgs e)
        {
            EmailRecipient sendTo = new EmailRecipient() { Address = "testgglol@outlook.com" };
            EmailMessage mail = new EmailMessage();

            mail.Subject = "Feedback for League Season 5 Counters";
            mail.To.Add(sendTo);
            await EmailManager.ShowComposeNewEmailAsync(mail);
        }

        private void AdBlock_Click(object sender, RoutedEventArgs e)
        {
            AdRemover.Purchase();
        }
    }
}
