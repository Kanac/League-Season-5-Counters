using League_of_Legends_Counterpicks.Common;
using League_of_Legends_Counterpicks.Data;
using League_of_Legends_Counterpicks.DataModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using Windows.System;
using Windows.ApplicationModel.Email;

// The Hub Application template is documented at http://go.microsoft.com/fwlink/?LinkID=391641

namespace League_of_Legends_Counterpicks
{
    /// <summary>
    /// A page that displays details for a single item within a group.
    /// </summary>
    public sealed partial class ChampionPage : Page
    {
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly String APP_ID = "3366702e-67c7-48e7-bc82-d3a4534f3086";
        private ObservableCollection<String> counters = new ObservableCollection<string>();
        private Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        private String feedback = String.Empty;
        private CommentDataSource commentViewModel = new CommentDataSource(App.MobileService);
        private ChampionFeedback championFeedback;


        public ChampionPage()
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

        //Iterate view count of champion page each time its viewed for purposes of how often to show rate and review page
        private async void reviewApp()
        {
            if (!localSettings.Values.ContainsKey("Views"))
                localSettings.Values.Add(new KeyValuePair<string, object>("Views", 0));
            else
                localSettings.Values["Views"] = 1 + Convert.ToInt32(localSettings.Values["Views"]);

            int viewCount = Convert.ToInt32(localSettings.Values["Views"]);

            //Only ask for review up to 10 times, once every 5 times this page is visited, and do not ask anymore once reviewed
            if (viewCount % 5 == 0 && viewCount <= 50 && Convert.ToInt32(localSettings.Values["Rate"]) != 1)
            {
                var reviewBox = new MessageDialog("Please rate this app 5 stars to support us!");
                reviewBox.Commands.Add(new UICommand { Label = "Yes! :)", Id = 0 });
                reviewBox.Commands.Add(new UICommand { Label = "Maybe later :(", Id = 1 });

                var reviewResult = await reviewBox.ShowAsync();

                if ((int)reviewResult.Id == 0)
                {
                    await Launcher.LaunchUriAsync(new Uri("ms-windows-store:reviewapp?appid=" + APP_ID));
                    localSettings.Values["Rate"] = 1;
                }
            }
        }
        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>.
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            reviewApp();
            var champName = (string)e.NavigationParameter;

            Champion champion = DataSource.GetChampion(champName);
            this.DefaultViewModel["Champion"] = champion;
            this.DefaultViewModel["Role"] = DataSource.GetRoleId(champion.UniqueId);
            counters = (this.DefaultViewModel["Champion"] as Champion).Counters;

            //If navigating via a counterpick, on loading that page, remove the previous history so the back page will go to main or role, not champion
            var prevPage = Frame.BackStack.ElementAt(Frame.BackStackDepth - 1);
            if (prevPage.SourcePageType.Equals(typeof(ChampionPage)))
            {
                Frame.BackStack.RemoveAt(Frame.BackStackDepth - 1);
                Debug.WriteLine("Done");
            }


            await commentViewModel.GetAllChampionFeedbackAsync();
            await commentViewModel.GetAllCommentsAsync();
            var query = commentViewModel.ChampionFeedbackCollection.Where(x => x.Name.Equals(champName));
            var count = query.Count();

            //Check if the ChampionFeedbackCollection is created on the server yet
            if (count == 0)
            {
                championFeedback = new ChampionFeedback()
                {
                    Name = champName
                };

                await commentViewModel.AddChampionFeedbackAsync(championFeedback);

            }
            //Otherwise, use the one created already
            else
            {
                championFeedback = query.ElementAt(0);
            }

            this.defaultViewModel["Comments"] = championFeedback.Comments;

            // TODO: Create an appropriate data model for your problem domain to replace the sample data


        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/>.</param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            // TODO: Save the unique state of the page here.
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
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion



        private void Champ_Tapped(object sender, TappedRoutedEventArgs e)
        {
            String champ = (sender as Image).Name.Substring("Champ".Length);
            int champIndex = Int32.Parse(champ) - 1;
            var championId = counters.ElementAt(champIndex);
            Frame.Navigate(typeof(ChampionPage), championId);

        }

        private async void Send_Feedback(object sender, TappedRoutedEventArgs e)
        {
            //if (String.IsNullOrEmpty(feedback)) {
            //    MessageDialog emptyBox = new MessageDialog("Write a message first!");
            //    await emptyBox.ShowAsync();
            //    return;
            //}
            //EmailRecipient sendTo = new EmailRecipient() {Address = "testgglol@outlook.com"};
            //EmailMessage mail = new EmailMessage();

            //var champ = this.DefaultViewModel["Champion"] as Champion;
            //mail.Subject = "Feedback for " + champ.UniqueId;
            //mail.Body = feedback;
            //mail.To.Add(sendTo);
            //await EmailManager.ShowComposeNewEmailAsync(mail);

            await commentViewModel.SubmitCommentAsync(championFeedback, feedback);

        }

        private void Feedback_Written(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            feedback = textBox.Text;

        }

    }


}
