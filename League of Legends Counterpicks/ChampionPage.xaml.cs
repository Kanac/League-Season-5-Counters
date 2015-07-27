using League_of_Legends_Counterpicks.Common;
using League_of_Legends_Counterpicks.Data;
using League_of_Legends_Counterpicks.DataModel;
using League_of_Legends_Counterpicks.Helper;
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
using System.Threading;


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
        private Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        private String feedback = String.Empty;
        private String name = String.Empty;
        private CommentDataSource commentViewModel = new CommentDataSource(App.MobileService);
        private bool emptyComments, emptyPlayingComments;
        private PageEnum.Page? pageType;
        private TextBlock counterMessage, playingMessage;
        private TextBox filterBox;
        private Role champions = DataSource.GetAllChampions();


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
                localSettings.Values.Add(new KeyValuePair<string, object>("Views", 3));
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
            champions = DataSource.GetAllChampions();
            this.DefaultViewModel["Champion"] = champion;
            this.DefaultViewModel["Role"] = DataSource.GetRoleId(champion.UniqueId);
            this.DefaultViewModel["Filter"] = champions.Champions;


            //If navigating via a counterpick, on loading that page, remove the previous history so the back page will go to main or role, not champion
            var prevPage = Frame.BackStack.ElementAt(Frame.BackStackDepth - 1);
            if (prevPage.SourcePageType.Equals(typeof(ChampionPage))){
                Frame.BackStack.RemoveAt(Frame.BackStackDepth - 1);
            }

            //Champion feedback code 
            //Grab the champion feedback from the server 
            await commentViewModel.GetChampionFeedbackAsync(champName);

            //Create a new Champion Feedback if one was not made
            if (commentViewModel.ChampionFeedback == null){
                var championFeedback = new ChampionFeedback(){
                    Name = champName
                };

                await commentViewModel.AddChampionFeedbackAsync(championFeedback);
            }

            //Seperate conditional incase champion feedback was already made, but counters weren't implemented yet
            if (commentViewModel.ChampionFeedback.Counters.Count == 0) {
                await commentViewModel.SeedCounterAsync(champion);
            }

          
            //Check if comments exist for counter comments. If not, show a message indicating so. 
            if (commentViewModel.ChampionFeedback.Comments.Where(x => x.Page == PageEnum.Page.Counter).Count() == 0) {
                if (counterMessage == null)
                    emptyComments = true;
                else
                    counterMessage.Visibility = Windows.UI.Xaml.Visibility.Visible;

            }

            //Check if comments exist for playing comments. If not, show a message indicating so. 
            if (commentViewModel.ChampionFeedback.Comments.Where(x => x.Page == PageEnum.Page.Playing).Count() == 0)
            {
                if (playingMessage == null)
                    emptyPlayingComments = true;
                else
                    playingMessage.Visibility = Windows.UI.Xaml.Visibility.Visible;

            }

            //Make updates to champion comments observable
            this.DefaultViewModel["ChampionFeedback"] = commentViewModel.ChampionFeedback;
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
            Image counterImage = (sender as Image);
            var counter = counterImage.DataContext as Counter;
            Frame.Navigate(typeof(ChampionPage), counter.Name);

        }

        private async void Send_Feedback(object sender, TappedRoutedEventArgs e)
        {
            //Ensure user selects comment type
            if (this.pageType == null)
            {
                MessageDialog emptyBox = new MessageDialog("Choose your comment as countering or playing as!");
                await emptyBox.ShowAsync();
                return;
            }

            //Ensure user inputs names
            if (String.IsNullOrEmpty(name) || name == "Your name")
            {
                MessageDialog emptyBox = new MessageDialog("Enter your name first!");
                await emptyBox.ShowAsync();
                return;
            }

            //Ensure user inputs feedback
            if (String.IsNullOrEmpty(feedback))
            {
                MessageDialog emptyBox = new MessageDialog("Write a message first!");
                await emptyBox.ShowAsync();
                return;
            }
           
            //Submit the comment and a self-user rating of 1
            var comment = await commentViewModel.SubmitCommentAsync(feedback, name, (PageEnum.Page)pageType);
            await commentViewModel.SubmitUserRating(comment, 1);   //This will then generate Upvote_Loaded to highlight the upvote image

            //Update the view
            commentViewModel.ChampionFeedback.SortComments();

            //Scroll to the proper hub section according to the type of comment 
            if (pageType == PageEnum.Page.Counter)
                MainHub.ScrollToSection(CounterCommentSection);
            else if (pageType == PageEnum.Page.Playing)
                MainHub.ScrollToSection(PlayingCommentSection);

            //Clear the feedback message box incase user double presses
            ((sender as Button).FindName("FeedbackBox") as TextBox).Text = String.Empty;
            feedback = String.Empty;

            //Update the view (remove no comment message and reference counter and playing respectively)
            counterMessage.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            playingMessage.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void Feedback_Written(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            feedback = textBox.Text;

        }

        private void Name_Written(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            name = textBox.Text;
        }

        private void Name_Focus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            textBox.Text = String.Empty;
        }

        private async void Upvote_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var upvote = (Image)sender;
            var comment = upvote.DataContext as Comment;

            var existingRating = comment.UserRatings.Where(x => x.UniqueUser == commentViewModel.GetDeviceId()).FirstOrDefault();
            //If there was a previous rating, change vote images respectively
            if (existingRating != null && existingRating.Score != 0)
            {
                //Pressing upvote again? Unhighlight the button.
                if (existingRating.Score == 1)
                {
                    upvote.Source = new BitmapImage(new Uri("ms-appx:///Assets/upvoteblank.png", UriKind.Absolute));
                }

                //Going from downvote to upvote? Change to highlighted upvote 
                else if (existingRating.Score == -1)
                {
                    var downvote = ((Image)sender).FindName("DownvoteImage") as Image;
                    downvote.Source = new BitmapImage(new Uri("ms-appx:///Assets/downvoteblank.png", UriKind.Absolute));
                    upvote.Source = new BitmapImage(new Uri("ms-appx:///Assets/upvote.png", UriKind.Absolute));
                }
            }
            //Otherwise, highlight the upvote button
            else {
                upvote.Source = new BitmapImage(new Uri("ms-appx:///Assets/upvote.png", UriKind.Absolute));
            }
            await commentViewModel.SubmitUserRating(comment, 1);
        }


        private async void Downvote_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var downvote = (Image)sender;
            var comment = downvote.DataContext as Comment;

            var existingRating = comment.UserRatings.Where(x => x.UniqueUser == commentViewModel.GetDeviceId()).FirstOrDefault();
            if (existingRating != null && existingRating.Score != 0)
            {
                if (existingRating.Score == 1)
                {
                    var upvote = ((Image)sender).FindName("UpvoteImage") as Image;
                    downvote.Source = new BitmapImage(new Uri("ms-appx:///Assets/downvote.png", UriKind.Absolute));
                    upvote.Source = new BitmapImage(new Uri("ms-appx:///Assets/upvoteblank.png", UriKind.Absolute));
                }

                else if (existingRating.Score == -1)
                {
                    downvote.Source = new BitmapImage(new Uri("ms-appx:///Assets/downvoteblank.png", UriKind.Absolute));
                }
            }
            else {
                downvote.Source = new BitmapImage(new Uri("ms-appx:///Assets/downvote.png", UriKind.Absolute));

            }

            await commentViewModel.SubmitUserRating(comment, -1);
        }

        private void Upvote_Loaded(object sender, RoutedEventArgs e)
        {
            Image upvote = sender as Image;
            var comment = upvote.DataContext as Comment;
            var existingRating = comment.UserRatings.Where(x => x.UniqueUser == commentViewModel.GetDeviceId()).FirstOrDefault();
            if (existingRating != null)
            {
                if (existingRating.Score == 1)
                {
                    upvote.Source = new BitmapImage(new Uri("ms-appx:///Assets/upvote.png", UriKind.Absolute));
                }
            }
        }

        private void Downvote_Loaded(object sender, RoutedEventArgs e)
        {
            Image downvote = sender as Image;
            var comment = downvote.DataContext as Comment;
            var existingRating = comment.UserRatings.Where(x => x.UniqueUser == commentViewModel.GetDeviceId()).FirstOrDefault();
            if (existingRating != null)
            {
                if (existingRating.Score == -1)
                {
                    downvote.Source = new BitmapImage(new Uri("ms-appx:///Assets/downvote.png", UriKind.Absolute));
                }
            }
        }


        private async void CounterUpvote_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var upvote = (Image)sender;
            var counter = upvote.DataContext as Counter;

            //Find the exisiting counter rating, if any
            var existingRating = counter.CounterRatings.Where(x => x.UniqueUser == commentViewModel.GetDeviceId()).FirstOrDefault();
            //If there was a previous rating, change vote images respectively
            if (existingRating != null && existingRating.Score != 0)
            {
                //Pressing upvote again? Unhighlight the button.
                if (existingRating.Score == 1)
                {
                    upvote.Source = new BitmapImage(new Uri("ms-appx:///Assets/upvoteblank.png", UriKind.Absolute));
                }

                //Going from downvote to upvote? Change to highlighted upvote 
                else if (existingRating.Score == -1)
                {
                    var downvote = ((Image)sender).FindName("DownvoteImage") as Image;
                    downvote.Source = new BitmapImage(new Uri("ms-appx:///Assets/downvoteblank.png", UriKind.Absolute));
                    upvote.Source = new BitmapImage(new Uri("ms-appx:///Assets/upvote.png", UriKind.Absolute));
                }
            }
            //Otherwise, highlight the upvote button
            else
            {
                upvote.Source = new BitmapImage(new Uri("ms-appx:///Assets/upvote.png", UriKind.Absolute));
            }
            await commentViewModel.SubmitCounterRating(counter, 1);

        }

        private async void CounterDownvote_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var downvote = (Image)sender;
            var counter = downvote.DataContext as Counter;

            var existingRating = counter.CounterRatings.Where(x => x.UniqueUser == commentViewModel.GetDeviceId()).FirstOrDefault();
            if (existingRating != null && existingRating.Score != 0)
            {
                if (existingRating.Score == 1)
                {
                    var upvote = ((Image)sender).FindName("UpvoteImage") as Image;
                    downvote.Source = new BitmapImage(new Uri("ms-appx:///Assets/downvote.png", UriKind.Absolute));
                    upvote.Source = new BitmapImage(new Uri("ms-appx:///Assets/upvoteblank.png", UriKind.Absolute));
                }

                else if (existingRating.Score == -1)
                {
                    downvote.Source = new BitmapImage(new Uri("ms-appx:///Assets/downvoteblank.png", UriKind.Absolute));
                }
            }
            else
            {
                downvote.Source = new BitmapImage(new Uri("ms-appx:///Assets/downvote.png", UriKind.Absolute));

            }

            await commentViewModel.SubmitCounterRating(counter, -1);

        }



        private void CounterUpvote_DataLoaded(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            Image upvote = sender as Image;
            var counter = upvote.DataContext as Counter;
            if (counter == null)
                return;
            var existingRating = counter.CounterRatings.Where(x => x.UniqueUser == commentViewModel.GetDeviceId()).FirstOrDefault();
            if (existingRating != null)
            {
                if (existingRating.Score == 1)
                {
                    upvote.Source = new BitmapImage(new Uri("ms-appx:///Assets/upvote.png", UriKind.Absolute));
                }
            }
        }

        private void CounterDownvote_DataLoaded(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            Image downvote = sender as Image;
            var counter = downvote.DataContext as Counter;
            if (counter == null)
                return;
            var existingRating = counter.CounterRatings.Where(x => x.UniqueUser == commentViewModel.GetDeviceId()).FirstOrDefault();
            if (existingRating != null)
            {
                if (existingRating.Score == -1)
                {
                    downvote.Source = new BitmapImage(new Uri("ms-appx:///Assets/downvote.png", UriKind.Absolute));
                }
            }
        }



        private void CounterMessage_Loaded(object sender, RoutedEventArgs e)
        {
            counterMessage = sender as TextBlock;
            if (emptyComments)
                counterMessage.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void PlayingComments_Loaded(object sender, RoutedEventArgs e)
        {
            playingMessage = sender as TextBlock;
            if (emptyPlayingComments)
                playingMessage.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void Counter_Checked(object sender, RoutedEventArgs e)
        {
            pageType = PageEnum.Page.Counter;
        }

        private void Playing_Checked(object sender, RoutedEventArgs e)
        {
            pageType = PageEnum.Page.Playing;
        }

        private void Comment_Clicked(object sender, RoutedEventArgs e)
        {
            MainHub.ScrollToSection(SubmitFeedbackSection);
        }

        private void Filter_GotFocus(object sender, RoutedEventArgs e)
        {
            var filterBox = sender as TextBox;
            filterBox.Text = String.Empty;
        }

        private void Filter_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox filterBox = sender as TextBox;
            Role filter = DataSource.FilterChampions(filterBox.Text);
            DefaultViewModel["Filter"] = filter.Champions;
        }

        private void Counter_Click(object sender, ItemClickEventArgs e)
        {
            Champion counter = e.ClickedItem as Champion;
            DefaultViewModel["Filter"] = DataSource.FilterChampions(filterBox.Text).Champions;
            filterBox.Text = counter.UniqueId;
        }

        private void FilterBox_Loaded(object sender, RoutedEventArgs e)
        {
            filterBox = sender as TextBox;
        }

        private async void Submit_Counter(object sender, TappedRoutedEventArgs e)
        {
            var filter = ((DefaultViewModel["Filter"]) as ObservableCollection<Champion>);

            //Only allow counter submision if there is exactly one selected
            if (filter.Count() != 1){
                MessageDialog messageBox = new MessageDialog("Select a champion first!");
                await messageBox.ShowAsync();
                return;

            }

            //Prevent duplicate counter submissions
            else if (commentViewModel.ChampionFeedback.Counters.Where(c => c.Name == filter.FirstOrDefault().UniqueId).Count() == 1) {
                String message = filter.FirstOrDefault().UniqueId + " is already a counter!";
                MessageDialog messageBox = new MessageDialog(message);
                await messageBox.ShowAsync();
                return;
            }

            //Otherwise, finally submit the counter
            var counter = await commentViewModel.SubmitCounter(filter.FirstOrDefault());
            await commentViewModel.SubmitCounterRating(counter, 1);
            commentViewModel.ChampionFeedback.SortCounters();
            MainHub.ScrollToSection(CounterSection);


        }

    }


}
