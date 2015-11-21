using League_of_Legends_Counterpicks.Common;
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
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Microsoft.Advertising.WinRT.UI;
using System.Collections.Generic;

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
        private InterstitialAd MyVideoAd = new InterstitialAd();
        private InterstitialAd MyVideoAd2 = new InterstitialAd();
        private InterstitialAd MyVideoAd3 = new InterstitialAd();
        private InterstitialAd MyVideoAd4 = new InterstitialAd();
        private InterstitialAd MyVideoAd5 = new InterstitialAd();
        private InterstitialAd MyVideoAd6 = new InterstitialAd();
        private InterstitialAd MyVideoAd7 = new InterstitialAd();
        private InterstitialAd MyVideoAd8 = new InterstitialAd();
        private InterstitialAd MyVideoAd9 = new InterstitialAd();
        private InterstitialAd MyVideoAd10 = new InterstitialAd();
        private InterstitialAd MyVideoAd11 = new InterstitialAd();
        private InterstitialAd MyVideoAd12 = new InterstitialAd();
    
        public MainPage()
        {
            this.NavigationCacheMode = NavigationCacheMode.Disabled;
            this.InitializeComponent();
        
            // Hub is only supported in Portrait orientation
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            if (!App.licenseInformation.ProductLicenses["AdRemoval"].IsActive)
            {
                MyVideoAd.RequestAd(AdType.Video, "bf747944-c75c-4f2a-a027-7c159b32261d", "256751");
                MyVideoAd.AdReady += MyVideoAd_AdReady;
                MyVideoAd2.RequestAd(AdType.Video, "bf747944-c75c-4f2a-a027-7c159b32261d", "256751");
                MyVideoAd2.AdReady += MyVideoAd_AdReady;
                MyVideoAd3.RequestAd(AdType.Video, "bf747944-c75c-4f2a-a027-7c159b32261d", "256751");
                MyVideoAd3.AdReady += MyVideoAd_AdReady;
                MyVideoAd4.RequestAd(AdType.Video, "bf747944-c75c-4f2a-a027-7c159b32261d", "256751");
                MyVideoAd4.AdReady += MyVideoAd_AdReady;
                MyVideoAd5.RequestAd(AdType.Video, "bf747944-c75c-4f2a-a027-7c159b32261d", "256751");
                MyVideoAd5.AdReady += MyVideoAd_AdReady;
                MyVideoAd7.RequestAd(AdType.Video, "bf747944-c75c-4f2a-a027-7c159b32261d", "256751");
                MyVideoAd7.AdReady += MyVideoAd_AdReady;
                MyVideoAd8.RequestAd(AdType.Video, "bf747944-c75c-4f2a-a027-7c159b32261d", "256751");
                MyVideoAd8.AdReady += MyVideoAd_AdReady;
                MyVideoAd9.RequestAd(AdType.Video, "bf747944-c75c-4f2a-a027-7c159b32261d", "256751");
                MyVideoAd9.AdReady += MyVideoAd_AdReady;
                MyVideoAd10.RequestAd(AdType.Video, "bf747944-c75c-4f2a-a027-7c159b32261d", "256751");
                MyVideoAd10.AdReady += MyVideoAd_AdReady;
                MyVideoAd11.RequestAd(AdType.Video, "bf747944-c75c-4f2a-a027-7c159b32261d", "256751");
                MyVideoAd11.AdReady += MyVideoAd_AdReady;
                MyVideoAd12.RequestAd(AdType.Video, "bf747944-c75c-4f2a-a027-7c159b32261d", "256751");
                MyVideoAd12.AdReady += MyVideoAd_AdReady;
            } 
        }

        private void MyVideoAd_AdReady(object sender, object e)
        {
            if (!App.licenseInformation.ProductLicenses["AdRemoval"].IsActive)
            {
                if (localSettings.Values["MainViews"] == null || ((int)(localSettings.Values["MainViews"])) % 24 != 0 || App.firstLoad)
                {
                    MyVideoAd.Show();
                    localSettings.Values["MainViews"] = 0; // guarantee its initialized to 0 if null to begin with 
                    App.firstLoad = false;
                }

                localSettings.Values["MainViews"] = ((int)(localSettings.Values["MainViews"])) + 1;
            }

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
  
            // Load all the roles (which contains all the champions) from the json file 
            var roles = StatsDataSource.GetRoles();
            this.DefaultViewModel["Roles"] = roles;
            await StatsDataSource.GetChampionsAsync();

            // Toast background task setup 
            if (e.PageState == null || (bool)e.PageState["firstLoad"] == true)
            {
                CheckAppVersion();
                string version = localSettings.Values["AppVersion"] as string;

                // Only show imminent toasts up to 2 times that the app is launched 
                if (localSettings.Values[version] == null)
                    localSettings.Values[version] = 0;

                if ((int)localSettings.Values[version] < 2)
                {
                    setupFeatureToast(); // Flashes a new feature message 
                    setupReuseToast(); // Creates a message indicating user to reuse app after 30 minutes of opening
                    localSettings.Values[version] = 1 + (int)localSettings.Values[version];
                }
                await setupToast();  // Background toast in 72 hours talking about new champion data
            }
        }

        private void setupFeatureToast()
        {
            ToastTemplateType toastTemplate = ToastTemplateType.ToastImageAndText02;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

            XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[1].AppendChild(toastXml.CreateTextNode("Kindred data has arrived!"));

            ToastNotification toast = new ToastNotification(toastXml);
            toast.Tag = "FeatureToast";
            ToastNotificationManager.History.Remove("FeatureToast"); // Remove previous toasts in action centre history, if any, before sending 
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        private void setupReuseToast()
        {
            // Check if a reuse toast is already scheduled
            if (ToastNotificationManager.CreateToastNotifier().GetScheduledToastNotifications().Select(x => x.Id = "Reuse").Count() > 0)
            {
                return;
            }

            ToastTemplateType toastTemplate = ToastTemplateType.ToastImageAndText02;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

            XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode("League of Legends"));
            toastTextElements[1].AppendChild(toastXml.CreateTextNode("In your ranked pick phase, remember to use this app for the advantage you need!"));

            IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            XmlElement audio = toastXml.CreateElement("audio");
            audio.SetAttribute("src", "ms-appx:///Assets/yourturn.mp3");
            toastNode.AppendChild(audio);

            ToastNotification toast = new ToastNotification(toastXml);
            DateTime dueTime = DateTime.Now.AddMinutes(50);
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

        /// <summary>
        /// Shows the details of a clicked group in the <see cref="RolePage"/>.
        /// </summary>
        private void GroupSection_ItemClick(object sender, ItemClickEventArgs e)
        {
            var roleId = ((string)e.ClickedItem);
             Frame.Navigate(typeof(RolePage), roleId);
        }
        private void Role_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            (sender as TextBlock).Foreground = new SolidColorBrush((Color)Application.Current.Resources["SystemColorControlAccentColor"]);
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
                foreach (var r in grid.RowDefinitions)
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

        private void Ad_Error(object sender, AdErrorEventArgs e)
        {

        }
    }
}
