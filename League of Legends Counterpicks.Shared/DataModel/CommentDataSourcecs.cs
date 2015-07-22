using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Windows.System.UserProfile;

namespace League_of_Legends_Counterpicks.DataModel
{

    //Comment Model
    public class PageEnum {
        public enum Page
        {
            Counter,
            Playing
        }

    }

    public class ChampionFeedback : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ChampionFeedback()
        {
            Comments = new ObservableCollection<Comment>();
        }
        
        public string Name { get; set; }
        public ObservableCollection<Comment> Comments { get; set; }
        public string Id { get; set; }

        public void SortComments()
        {
            var sorted = Comments.OrderByDescending(x => x.Score).ToList();
            for (int i = 0; i < sorted.Count(); i++)
                Comments.Move(Comments.IndexOf(sorted[i]), i);

            NotifyPropertyChanged("Comments");
        }

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

    }

    public class Comment : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public Comment() {
            UserRatings = new ObservableCollection<UserRating>();
        }
        public string Text { get; set; }
        public string User { get; set; }
        private int score { get; set; }
        public int Score { get 
        {
            return score;
        }
            set {
                if (value != score) {
                    score = value;
                    NotifyPropertyChanged("Score");
                }
                    
            }
        }

        public PageEnum.Page Page { get; set; }
     
        public ICollection<UserRating> UserRatings { get; set; }

        public string Id { get; set; }

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }

    public class UserRating 
    {
        public string UniqueUser { get; set; }
        public int Score { get; set; }

        public string Id { get; set; }
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

        //This value will change as user changes champions 
        //This is to prevent excess loading of all champion data 
        private ChampionFeedback _ChampionFeedback;
        public ChampionFeedback ChampionFeedback
        {
            get { return _ChampionFeedback; }
            set
            {
                _ChampionFeedback = value;
                NotifyPropertyChanged("ChampionFeedback");
            }
        }

        public String DeviceId { get; set; }

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


        // Service operations
        public async Task GetChampionFeedbackAsync(string championName)
        {
            try
            {
                IMobileServiceTable<ChampionFeedback> table = _client.GetTable<ChampionFeedback>();
                var result = await table.Where(c => c.Name == championName).ToListAsync();
                if (result.Count != 0)
                {   //If there was a match found for champion, 
                    ChampionFeedback = result[0];    //Get the first (and only) result
                    ChampionFeedback.SortComments();
                }
                else
                    ChampionFeedback = null;   //Otherwise set it to null to be checked in code later


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
                await table.InsertAsync(champion);
                this.ChampionFeedback = champion;
               
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

        public async Task<Comment> SubmitCommentAsync(String text, String name, PageEnum.Page type)
        {
            IsPending = true;
            ErrorMessage = null;

            //Create the new comment
            var comment = new Comment()
            {
                Score = 0,
                Text = text,
                User = name,
                Page = type,
                Id = Guid.NewGuid().ToString(),
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
                var table =  _client.GetTable<ChampionFeedback>();
                this.ChampionFeedback.Comments.Add(comment);
                await table.UpdateAsync(this.ChampionFeedback);
                ChampionFeedback.SortComments();
                return comment;

            }
            catch (MobileServiceInvalidOperationException ex)
            {
                ErrorMessage = ex.Message;
                return null;
            }
            catch (HttpRequestException ex2)
            {
                ErrorMessage = ex2.Message;
                return null;
            }
            finally
            {
                IsPending = false;
            }
        }

        public async Task SubmitUserRating(Comment comment, int score) {

            IsPending = true;
            ErrorMessage = null;

            //Check if the user already rated the comment
            var existingRating = comment.UserRatings.Where(x => x.UniqueUser == GetDeviceId()).FirstOrDefault();
            //Comment reference might change via new UpdateAsync reference, so update the reference 
            comment = ChampionFeedback.Comments.Where(x => x.Id == comment.Id).FirstOrDefault();
            //If already rated, update to the new score 
            if (existingRating != null) {
                comment.Score -= existingRating.Score;  //revert the previous score 

                //Case for pressing the opposite vote button -- change rating to it
                if (existingRating.Score != score)
                {
                    comment.Score += score;
                    existingRating.Score = score;
                }
                //Case for pressing the same vote button -- simply a score of 0
                else {

                    existingRating.Score = 0;
                }
            }
            //Create a new rating otherwise
            else{
                var userRating = new UserRating()
                {
                    Score = score,
                    UniqueUser = GetDeviceId(),
                    Id = Guid.NewGuid().ToString()
                };

                //Update the comment 
                comment.UserRatings.Add(userRating);
                comment.Score += score;
            }
            try
            {
                var table = _client.GetTable<ChampionFeedback>();
                await table.UpdateAsync(ChampionFeedback);
                ChampionFeedback.SortComments();
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

        public string GetDeviceId()
        {
            if (DeviceId != null)
                return DeviceId;

            var token = Windows.System.Profile.HardwareIdentification.GetPackageSpecificToken(null);
            var hardwareId = token.Id;
            var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(hardwareId);

            byte[] bytes = new byte[hardwareId.Length];
            dataReader.ReadBytes(bytes);

            DeviceId =  BitConverter.ToString(bytes).Replace("-", "");
            return DeviceId;
        }//Note: This function may throw an exception. 


        //public async Task GetAllCommentsAsync()
        //{
        //    IsPending = true;
        //    ErrorMessage = null;

        //    try
        //    {
        //        IMobileServiceTable<Comment> table = _client.GetTable<Comment>();
        //        Comments = await table.OrderBy(x => x.Score).ToCollectionAsync();
        //    }
        //    catch (MobileServiceInvalidOperationException ex)
        //    {
        //        ErrorMessage = ex.Message;
        //    }
        //    catch (HttpRequestException ex2)
        //    {
        //        ErrorMessage = ex2.Message;
        //    }
        //    finally
        //    {
        //        IsPending = false;
        //    }
        //}

        //public async Task GetAllChampionFeedbackAsync()
        //{
        //    IsPending = true;
        //    ErrorMessage = null;

        //    try
        //    {
        //        IMobileServiceTable<ChampionFeedback> table = _client.GetTable<ChampionFeedback>();
        //        ChampionFeedbackCollection = await table.OrderBy(x => x.Name).ToCollectionAsync();
        //    }
        //    catch (MobileServiceInvalidOperationException ex)
        //    {
        //        ErrorMessage = ex.Message;
        //    }
        //    catch (HttpRequestException ex2)
        //    {
        //        ErrorMessage = ex2.Message;
        //    }
        //    finally
        //    {
        //        IsPending = false;
        //    }
        //}
    }
}
