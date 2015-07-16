using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.System.UserProfile;

namespace League_of_Legends_Counterpicks.DataModel
{


    //Comment Model
    public class ChampionFeedback 
    {
        public ChampionFeedback()
        {
            Comments = new List<Comment>();
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }

    public class Comment
    {
        public Comment()
        {
            UserRatings = new List<UserRating>();
        }
        public string Id { get; set; }
        public string Text { get; set; }
        public string User { get; set; }
        public int Score { get; set; }
        public string ChampionFeedbackId { get; set; }
        public virtual ChampionFeedback ChampionFeedback { get; set; }
        public virtual ICollection<UserRating> UserRatings { get; set; }
    }

    public class UserRating 
    {
        public string Id { get; set; }
        public string UniqueUser { get; set; }
        public int Score { get; set; }
        public string CommentId { get; set; }
        public virtual Comment Comment { get; set; }
    }


    //Comment View Model
    class CommentDataSource : INotifyPropertyChanged
    {
        MobileServiceClient _client;

        public CommentDataSource(MobileServiceClient client)
        {
            _client = client;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }

        // View model properties
        private MobileServiceCollection<ChampionFeedback, ChampionFeedback> _ChampionFeedbackCollection;
        public MobileServiceCollection<ChampionFeedback, ChampionFeedback> ChampionFeedbackCollection
        {
            get { return _ChampionFeedbackCollection; }
            set
            {
                _ChampionFeedbackCollection = value;
                NotifyPropertyChanged("ChampionFeedbackCollection");
            }
        }

        private MobileServiceCollection<Comment, Comment> _Comments;
        public MobileServiceCollection<Comment, Comment> Comments
        {
            get { return _Comments; }
            set
            {
                _Comments = value;
                NotifyPropertyChanged("Comments");
            }
        }

        private bool _IsPending;
        public bool IsPending
        {
            get { return _IsPending; }
            set
            {
                _IsPending = value;
                NotifyPropertyChanged("IsPending");
            }
        }

        private string _ErrorMessage = null;
        public string ErrorMessage
        {
            get { return _ErrorMessage; }
            set
            {
                _ErrorMessage = value;
                NotifyPropertyChanged("ErrorMessage");
            }
        }

        private MobileServiceCollection<UserRating, UserRating> _UserRatings;
        public MobileServiceCollection<UserRating, UserRating> UserRatings
        {
            get { return _UserRatings; }
            set
            {
                _UserRatings = value;
                NotifyPropertyChanged("UserRatings");
            }
        }



        // Service operations
        public async Task GetAllChampionFeedbackAsync()
        {
            IsPending = true;
            ErrorMessage = null;

            try
            {
                IMobileServiceTable<ChampionFeedback> table = _client.GetTable<ChampionFeedback>();
                ChampionFeedbackCollection = await table.OrderBy(x => x.Name).ToCollectionAsync();
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (HttpRequestException ex2)
            {
                ErrorMessage = ex2.Message;
            }
            finally
            {
                IsPending = false;
            }
        }

        public async Task AddChampionFeedbackAsync(ChampionFeedback champion)
        {
            IsPending = true;
            ErrorMessage = null;

            try
            {
                IMobileServiceTable<ChampionFeedback> table = _client.GetTable<ChampionFeedback>();
                var test = table.SystemProperties;
                await table.InsertAsync(champion);
                ChampionFeedbackCollection.Add(champion);
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (HttpRequestException ex2)
            {
                ErrorMessage = ex2.Message;
            }
            finally
            {
                IsPending = false;
            }
        }

        public async Task SubmitCommentAsync(ChampionFeedback champion, String text)
        {
            IsPending = true;
            ErrorMessage = null;

            //Create the new comment
            var comment = new Comment()
            {
                Score = 0,
                Text = text,
                User = await UserInformation.GetFirstNameAsync(),
                UserRatings = new List<UserRating>()
            };

            //Add the comment to the champion
            //champion.Comments.Add(comment);

            //Sort the comments after submission
            //var comments = new List<Comment>(champion.Comments);
            //comments.Sort(delegate(Comment p1, Comment p2)
            //{
            //    return p1.Score.CompareTo(p2.Score);
            //});
            //champion.Comments = comments;

            try
            {
                //Insert comment to database
                IMobileServiceTable<Comment> table = _client.GetTable<Comment>();
                await table.InsertAsync(comment);
                champion.Comments.Add(comment);
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (HttpRequestException ex2)
            {
                ErrorMessage = ex2.Message;
            }
            finally
            {
                IsPending = false;
            }
        }

        public async Task GetAllCommentsAsync()
        {
            IsPending = true;
            ErrorMessage = null;

            try
            {
                IMobileServiceTable<Comment> table = _client.GetTable<Comment>();
                Comments = await table.OrderBy(x => x.Score).ToCollectionAsync();
            }
            catch (MobileServiceInvalidOperationException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (HttpRequestException ex2)
            {
                ErrorMessage = ex2.Message;
            }
            finally
            {
                IsPending = false;
            }
        }
    }
}
