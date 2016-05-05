using League_of_Legends_Counterpicks.Common;
using League_of_Legends_Counterpicks.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using Windows.UI.Core;
using Windows.System;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Microsoft.Advertising.WinRT.UI;


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
        private Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        private CommentDataSource commentViewModel = new CommentDataSource(App.MobileService);
        private Champions champions;
        private ChampionInfo champInfo;
        private TextBlock selectedTextBlock;

        public ChampionPage()
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
            App.IsInternetAvailable();

            CreateAdUnits();

            // Setup the underlying UI 
            string champKey = (string)e.NavigationParameter;
            champions = await StatsDataSource.GetChampionsAsync();
       
            // Could be passed either the key or name, check for either case
            champInfo = champions.ChampionInfos.Where(x => x.Key == champKey).FirstOrDefault().Value;
            if (champInfo == null)
                champInfo = champions.ChampionInfos.Values.Where(x => x.Name == champKey).FirstOrDefault();

            this.DefaultViewModel["Champion"] = champInfo;
            this.DefaultViewModel["Filter"] = champions.ChampionInfos.OrderBy(x => x.Value.Name);


            // If navigating via a counterpick, on loading that page, remove the previous history so the back page will go to main or role, not champion
            var prevPage = Frame.BackStack.ElementAt(Frame.BackStackDepth - 1);
            if (prevPage.SourcePageType.Equals(typeof(ChampionPage)))
            {
                Frame.BackStack.RemoveAt(Frame.BackStackDepth - 1);
            }

            // Grab the champion feedback from the server 
            await commentViewModel.GetChampionFeedbackAsync(champInfo.Name);

            // Check if an there was no champion retrieved as well as an error message (must be internet connection problem)
            if (commentViewModel.ChampionFeedback == null){
                MessageDialog messageBox = new MessageDialog("Make sure your internet connection is working and try again!");
                await messageBox.ShowAsync();
                Application.Current.Exit();
            }
           
            // Collapse the progress ring once counters have been loaded assuming the textboxes load first
            DefaultViewModel["LoadingVisibility"] = Visibility.Collapsed;

            // Make updates to champion comments observable
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

        private async void StatPage_Navigate(object sender, RoutedEventArgs e)
        {
            //ulong AppMemoryUsageUlong = MemoryManager.AppMemoryUsage;

            //ulong AppMemoryUsageLimitUlong = MemoryManager.AppMemoryUsageLimit;

            //AppMemoryUsageUlong /= 1024 * 1024;
            //AppMemoryUsageLimitUlong /= 1024 * 1024;
            //string test = "App memory uage - " + AppMemoryUsageUlong.ToString();
            //test += "\nApp memory usage limit - " + AppMemoryUsageLimitUlong.ToString();
            //var message = new MessageDialog(test);
            //await message.ShowAsync();
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Frame.Navigate(typeof(StatsPage), champInfo.Key));
        }

        // Flyout the comment section for the particular counter/easy/synergy matchup
        private void Comment_Flyout(object sender, TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
            BottomAppBar.Visibility = Visibility.Collapsed;
            selectedTextBlock = sender as TextBlock;
        }

        private void Comment_FlyoutClosed(object sender, object e)
        {
            BottomAppBar.Visibility = Visibility.Visible;
        }

        private async void Submit_CounterComment(object sender, TappedRoutedEventArgs e)
        {
            Button button = sender as Button;
            TextBox nameBox = button.FindName("CounterCommentNameBox") as TextBox;
            TextBox commentBox = button.FindName("CounterCommentBox") as TextBox;
            Counter counter = button.DataContext as Counter;

            // Ensure all collections are loaded first
            if (counter.CounterComments == null)
            {
                MessageDialog emptyBox = new MessageDialog("Wait for data to finish loading!");
                await emptyBox.ShowAsync();
                return;
            }

            // Ensure user inputs names
            if (String.IsNullOrEmpty(nameBox.Text) || nameBox.Text.Count() > 25)
            {
                MessageDialog emptyBox = new MessageDialog("Enter your name first!");
                await emptyBox.ShowAsync();
                return;
            }

            if (nameBox.Text.Count() > 25)
            {
                MessageDialog emptyBox = new MessageDialog("Write a shorter name!");
                await emptyBox.ShowAsync();
                return;
            }

            // Check for empty message
            if (String.IsNullOrEmpty(commentBox.Text))
            {
                MessageDialog emptyBox = new MessageDialog("Write a message first!");
                await emptyBox.ShowAsync();
                return;
            }

            if (commentBox.Text.Count() > 2000)
            {
                MessageDialog emptyBox = new MessageDialog("Write a shorter message!");
                await emptyBox.ShowAsync();
                return;
            }

            // Ensure user inputs a non-garbage feedback for ascii and chinese
            if ((commentBox.Text[0] <= 255 && commentBox.Text[0] >= 0 && (commentBox.Text.Count() < 30 || commentBox.Text.Distinct().Count() < 5 || !commentBox.Text.Contains(' '))) || ((commentBox.Text[0] >= 0x4E00 && commentBox.Text[0] <= 0x9FA5) && commentBox.Text.Count() < 8))
            {
                MessageDialog emptyBox = new MessageDialog("Write a proper and long enough message first!");
                await emptyBox.ShowAsync();
                return;
            }

            // Ensure at least 8 chars for non-ascii or non-chinese cases
            if ((commentBox.Text.Count() < 8 || commentBox.Text.Distinct().Count() < 5))
            {
                MessageDialog emptyBox = new MessageDialog("Write an actual message first!");
                await emptyBox.ShowAsync();
                return;
            }

            string commentText = commentBox.Text;
            commentBox.Text = String.Empty;
            (button.FindName("EmptyMessage") as TextBlock).Visibility = Visibility.Collapsed;

            // Submit the comment and a self-user rating of 1
            var comment = await commentViewModel.SubmitCounterCommentAsync(commentText, nameBox.Text, counter);
            await commentViewModel.SubmitCounterCommentRating(comment, 1);   

            // Update the view
            counter.SortCounterComments();
            int count = counter.CounterComments.Count();
            selectedTextBlock.Text = (count == 1) ? count + " comment" : count + " comments";
            if (count > 0) selectedTextBlock.Foreground = new SolidColorBrush(Colors.Yellow);
        }

        // Normal method of handling counter tapped
        private async void Champ_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Image counterImage = (sender as Image);
            counterImage.Opacity = 0.5;
            var counter = counterImage.DataContext as Counter;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Frame.Navigate(typeof(ChampionPage), counter.Name));

        }

        // Reverse relationship for easy matchups 
        private async void EasyMatchup_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Image easyMatchupImage = (sender as Image);
            easyMatchupImage.Opacity = 0.5;
            var easyMatchup = easyMatchupImage.DataContext as Counter;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Frame.Navigate(typeof(ChampionPage), easyMatchup.ChampionFeedbackName));
        }


        // Bidirectional relationship for synergy picks
        private async void SynergyChamp_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Image synergyImage = (sender as Image);
            synergyImage.Opacity = 0.5;
            var synergy = synergyImage.DataContext as Counter;
            string championName;

            // Choose the synergy name that is not the current champion's name for this page (We want to navigate on the other end of the relationship)
            if (synergy.ChampionFeedbackName == commentViewModel.ChampionFeedback.Name)
                championName = synergy.Name;
            else
                championName = synergy.ChampionFeedbackName;

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Frame.Navigate(typeof(ChampionPage), championName));
        }

        private void Synergy_Loaded(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            var synergyImage = sender as Image;
            var synergyChamp = synergyImage.DataContext as Counter;
            //If null, simply return until datacontext changes to a proper form and comes back here again
            if (synergyChamp == null)
                return;

            // Get the champion's name for the page
            var champName = commentViewModel.ChampionFeedback.Name;
            string imageName;

            // The synergy relationship could be either way -- but we know we want the image that's not the champion for the page
            if (synergyChamp.ChampionFeedbackName == champName)
                imageName = synergyChamp.Name;
            else
                imageName = synergyChamp.ChampionFeedbackName;

            // Change name to key (the format for images)
            imageName = champions.ChampionInfos.Where(x => x.Value.Name == imageName).FirstOrDefault().Key;
            var uri = "ms-appx:///Assets/" + imageName + "_Square_0.png";
            synergyImage.Source = new BitmapImage(new Uri(uri, UriKind.Absolute));
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


        // Using dataloaded instead for counter votes due to the fact that >5 counters causes the datacontext of the subsequent images to be null briefly
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

        private async void CounterCommentUpvote_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var upvote = (Image)sender;
            var counterComment = upvote.DataContext as CounterComment;

            //Find the exisiting counter rating, if any
            var existingRating = counterComment.CounterCommentRatings.Where(x => x.UniqueUser == commentViewModel.GetDeviceId()).FirstOrDefault();
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
            await commentViewModel.SubmitCounterCommentRating(counterComment, 1);

        }

        private async void CounterCommentDownvote_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var downvote = (Image)sender;
            var counterComment = downvote.DataContext as CounterComment;

            var existingRating = counterComment.CounterCommentRatings.Where(x => x.UniqueUser == commentViewModel.GetDeviceId()).FirstOrDefault();
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

            await commentViewModel.SubmitCounterCommentRating(counterComment, -1);
        }


        // Using dataloaded instead for counter votes due to the fact that >5 counters causes the datacontext of the subsequent images to be null briefly
        private void CounterCommentUpvote_DataLoaded(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            Image upvote = sender as Image;
            var counterComment = upvote.DataContext as CounterComment;
            if (counterComment == null)
                return;
            var existingRating = counterComment.CounterCommentRatings.Where(x => x.UniqueUser == commentViewModel.GetDeviceId()).FirstOrDefault();
            if (existingRating != null)
            {
                if (existingRating.Score == 1)
                {
                    upvote.Source = new BitmapImage(new Uri("ms-appx:///Assets/upvote.png", UriKind.Absolute));
                }
            }
        }

        private void CounterCommentDownvote_DataLoaded(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            Image downvote = sender as Image;
            var counterComment = downvote.DataContext as CounterComment;
            if (counterComment == null)
                return;
            var existingRating = counterComment.CounterCommentRatings.Where(x => x.UniqueUser == commentViewModel.GetDeviceId()).FirstOrDefault();
            if (existingRating != null)
            {
                if (existingRating.Score == -1)
                {
                    downvote.Source = new BitmapImage(new Uri("ms-appx:///Assets/downvote.png", UriKind.Absolute));
                }
            }
        }

        private async void Submit_Counter(object sender, TappedRoutedEventArgs e)
        {
            // Get the query
            var filter = DefaultViewModel["Filter"] as IOrderedEnumerable<KeyValuePair<string, ChampionInfo>>;

            // Ensure all collections are loaded first
            if (commentViewModel.ChampionFeedback == null || filter == null)
            {
                MessageDialog emptyBox = new MessageDialog("Wait for data to finish loading!");
                await emptyBox.ShowAsync();
                return;
            }

            // Ensure an option is checked 
            if (ChampionCounterRadio.IsChecked == false && ChampionEasyRadio.IsChecked == false && ChampionSynergyRadio.IsChecked == false)
            {
                MessageDialog messageBox = new MessageDialog("Choose whether this champion is a counter, easy matchup, or synergy pick!");
                await messageBox.ShowAsync();
                return;
            }

            // Only allow counter submision if there is exactly one selected
            if (filter.Count() != 1)
            {
                MessageDialog messageBox = new MessageDialog("Select a champion first!");
                await messageBox.ShowAsync();
                return;
            }

            // Get the selected champion's name
            var selectedChamp = filter.FirstOrDefault().Value.Name;

            if (selectedChamp == commentViewModel.ChampionFeedback.Name)
            {
                MessageDialog messageBox = new MessageDialog("You cannot submit a champion as a counter of itself!");
                await messageBox.ShowAsync();
                return;
            }
            
            // Get ready to process selection
            PageEnum.ClientChampionPage championPageType = 0;

            // Prevent duplicate counter submissions for counters 
            if (ChampionCounterRadio.IsChecked == true)
            {
                championPageType = PageEnum.ClientChampionPage.Counter;
                MainPivot.SelectedIndex = MainPivot.Items.IndexOf(CounterSection);

                if (commentViewModel.ChampionFeedback.Counters.Where(c => c.Page == PageEnum.ChampionPage.Counter && c.Name == selectedChamp).Count() == 1)
                {
                    String message = selectedChamp + " is already a counter!";
                    MessageDialog messageBox = new MessageDialog(message);
                    await messageBox.ShowAsync();
                    return;
                }
                
            }

            // Prevent duplicate counter submissions for easy matchups (it is reversed) 
            if (ChampionEasyRadio.IsChecked == true)
            {
                championPageType = PageEnum.ClientChampionPage.EasyMatchup;
                MainPivot.SelectedIndex = MainPivot.Items.IndexOf(EasyMatchupSection);

                if (commentViewModel.ChampionFeedback.EasyMatchups.Where(c => c.ChampionFeedbackName == selectedChamp).Count() == 1)
                {
                    String message = selectedChamp + " is already an easy matchup!";
                    MessageDialog messageBox = new MessageDialog(message);
                    await messageBox.ShowAsync();
                    return;
                }

            }

            // Prevent duplicate synergy submissions
            if (ChampionSynergyRadio.IsChecked == true)
            {
                championPageType = PageEnum.ClientChampionPage.Synergy;
                MainPivot.SelectedIndex = MainPivot.Items.IndexOf(SynergySection);

                // Check both ways (whether the synergy is the child as a counter or the parent as a champion feedback)
                if (commentViewModel.ChampionFeedback.Counters.Where(c => c.Page == PageEnum.ChampionPage.Synergy && (c.Name == selectedChamp || c.ChampionFeedbackName == selectedChamp)).Count() == 1)
                {
                    String message = selectedChamp + " is already a synergy pick!";
                    MessageDialog messageBox = new MessageDialog(message);
                    await messageBox.ShowAsync();
                    return;
                }

                NoSynergyChampions.Visibility = Visibility.Collapsed;
            }

            // Ensure the flyout is hidden
            ChampionFlyout.Hide();

            // Finally submit the counter
            var counter = await commentViewModel.SubmitCounter(selectedChamp, championPageType);
            await commentViewModel.SubmitCounterRating(counter, 1);

            // Sort the appropriate collection
            if (championPageType == PageEnum.ClientChampionPage.Counter)
                commentViewModel.ChampionFeedback.SortCounters();
            else if (championPageType == PageEnum.ClientChampionPage.EasyMatchup)
                commentViewModel.ChampionFeedback.SortEasyMatchups();
            else
                commentViewModel.ChampionFeedback.SortSynergy();

        }

        private void Filter_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox filterBox = sender as TextBox;
            filterBox.Text = String.Empty;
        }

        private void Filter_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox filterBox = sender as TextBox;
            DefaultViewModel["Filter"] = champions.ChampionInfos.Where(x => x.Value.Name.ToLower().Contains(filterBox.Text.ToLower())).OrderBy(x => x.Value.Name);
        }

        private void Filter_Click(object sender, ItemClickEventArgs e)
        {
            string counter = ((KeyValuePair<string, ChampionInfo>)e.ClickedItem).Value.Name;
            DefaultViewModel["Filter"] = champions.ChampionInfos.Where(x => x.Value.Name == counter);
            FilterBox.Text = counter;
        }

        private void Name_Focus(object sender, RoutedEventArgs e)
        {
            var nameBox = sender as TextBox;
            if (nameBox.Text == "Your Name")
                nameBox.Text = String.Empty;
        }

        private async void Send_Feedback(object sender, TappedRoutedEventArgs e)
        {
            
            // Ensure all collections are loaded first
            if (commentViewModel.ChampionFeedback.Comments == null)
            {
                MessageDialog emptyBox = new MessageDialog("Wait for data to finish loading!");
                await emptyBox.ShowAsync();
                return;
            }

            // Ensure user selects comment type
            if (CounterRadio.IsChecked == false && PlayingRadio.IsChecked == false)
            {
                MessageDialog emptyBox = new MessageDialog("Choose your comment as countering or playing as!");
                await emptyBox.ShowAsync();
                return;
            }

            // Ensure user inputs names
            if (String.IsNullOrEmpty(NameBox.Text) || NameBox.Text == "Your name")
            {
                MessageDialog emptyBox = new MessageDialog("Enter your name first!");
                await emptyBox.ShowAsync();
                return;
            }

            // Ensure user inputs proper feedback (if english string, check for non-spam with enough words, if chinese, check for at least 8 characters)
            if ((FeedbackBox.Text[0] <= 255 && FeedbackBox.Text[0] >= 0 && (FeedbackBox.Text.Count() < 30 || FeedbackBox.Text.Distinct().Count() < 5 || !FeedbackBox.Text.Contains(' '))) || ((FeedbackBox.Text[0] >= 0x4E00 && FeedbackBox.Text[0] <= 0x9FA5) && FeedbackBox.Text.Count() < 8))
            {
                MessageDialog emptyBox = new MessageDialog("Write a proper and long enough message first!");
                await emptyBox.ShowAsync();
                return;
            }

            // Ensure at least 8 chars for non-ascii or non-chinese cases
            if ((FeedbackBox.Text.Count() < 8 || FeedbackBox.Text.Distinct().Count() < 5))
            {
                MessageDialog emptyBox = new MessageDialog("Write an actual message first!");
                await emptyBox.ShowAsync();
                return;
            }

            PageEnum.CommentPage page = 0;
            // After ensuring the data is allowed, scroll to the proper hub section according to the type of comment (doing this before actual submission prevents lag)
            if (CounterRadio.IsChecked  == true)
            {
                MainPivot.SelectedIndex = MainPivot.Items.IndexOf(CounterCommentSection);
                page = PageEnum.CommentPage.Counter;
                NoCounterComments.Visibility = Visibility.Collapsed;

            }
            else if (PlayingRadio.IsChecked == true)
            {
                MainPivot.SelectedIndex = MainPivot.Items.IndexOf(PlayingCommentSection);
                page = PageEnum.CommentPage.Playing;
                NoPlayingComments.Visibility = Visibility.Collapsed;
            }

            // Clear textbox to prevent double post
            string feedbackText = FeedbackBox.Text;
            FeedbackBox.Text = String.Empty;

            // Close the fullscreen flyout
            CommentFlyout.Hide();

            // Submit the comment and a self-user rating of 1
            var comment = await commentViewModel.SubmitCommentAsync(feedbackText, NameBox.Text, page);
            await commentViewModel.SubmitUserRating(comment, 1);   // This will then generate Upvote_Loaded to highlight the upvote image

            // Update the view
            commentViewModel.ChampionFeedback.SortComments();

        }

        private async void Upvote_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var upvote = (Image)sender;
            var comment = upvote.DataContext as Comment;

            var existingRating = comment.UserRatings.Where(x => x.UniqueUser == commentViewModel.GetDeviceId()).FirstOrDefault();
            // If there was a previous rating, change vote images respectively
            if (existingRating != null && existingRating.Score != 0)
            {
                // Pressing upvote again? Unhighlight the button.
                if (existingRating.Score == 1)
                {
                    upvote.Source = new BitmapImage(new Uri("ms-appx:///Assets/upvoteblank.png", UriKind.Absolute));
                }

                // Going from downvote to upvote? Change to highlighted upvote 
                else if (existingRating.Score == -1)
                {
                    var downvote = ((Image)sender).FindName("DownvoteImage") as Image;
                    downvote.Source = new BitmapImage(new Uri("ms-appx:///Assets/downvoteblank.png", UriKind.Absolute));
                    upvote.Source = new BitmapImage(new Uri("ms-appx:///Assets/upvote.png", UriKind.Absolute));
                }
            }
            // Otherwise, highlight the upvote button
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


        private void Upvote_DataLoaded(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            Image upvote = sender as Image;
            var comment = upvote.DataContext as Comment;
            // If data context hasn't loaded yet, return for now until it does (will come back into this function)
            if (comment == null)
                return;

            var existingRating = comment.UserRatings.Where(x => x.UniqueUser == commentViewModel.GetDeviceId()).FirstOrDefault();
            if (existingRating != null)
            {
                if (existingRating.Score == 1)
                {
                    upvote.Source = new BitmapImage(new Uri("ms-appx:///Assets/upvote.png", UriKind.Absolute));
                }
            }
        }

        private void Downvote_DataLoaded(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            Image downvote = sender as Image;
            var comment = downvote.DataContext as Comment;

            // If data context hasn't loaded yet, return for now until it does (will come back into this function)
            if (comment == null)
                return;

            var existingRating = comment.UserRatings.Where(x => x.UniqueUser == commentViewModel.GetDeviceId()).FirstOrDefault();
            if (existingRating != null)
            {
                if (existingRating.Score == -1)
                {
                    downvote.Source = new BitmapImage(new Uri("ms-appx:///Assets/downvote.png", UriKind.Absolute));
                }
            }
        }
    
        private void CreateAdUnits()
        {
            if (App.licenseInformation.ProductLicenses["AdRemoval"].IsActive)
                return;

            int count = 35;
            var limitMb = MemoryManager.AppMemoryUsageLimit / (1024 * 1024);
            if (limitMb > 700)
            {
                count = 70;
            }

            for (int i = 0; i < count; ++i)
            {
                AdControl ad = new AdControl();
                ad.ApplicationId = "bf747944-c75c-4f2a-a027-7c159b32261d";
                ad.AdUnitId = "240175";
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
