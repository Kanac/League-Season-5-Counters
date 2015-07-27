﻿using Microsoft.WindowsAzure.MobileServices;
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
using League_of_Legends_Counterpicks.Data;

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
            Counters = new ObservableCollection<Counter>();
        }
        
        public string Name { get; set; }
        public ObservableCollection<Comment> Comments { get; set; }
        public ObservableCollection<Counter> Counters { get; set; }
        public string Id { get; set; }

        public void SortComments()
        {
            var sorted = Comments.OrderByDescending(x => x.Score).ToList();
            for (int i = 0; i < sorted.Count(); i++)
                Comments.Move(Comments.IndexOf(sorted[i]), i);

            NotifyPropertyChanged("Comments");
        }

        public void SortCounters()
        {
            var sorted = Counters.OrderByDescending(x => x.Score).ToList();
            for (int i = 0; i < sorted.Count(); i++){
                //Move the elemented to its new location
                Counters.Move(Counters.IndexOf(sorted[i]), i);
                //Calculate its new progress bar value as a function of its index and the total numbers of counters 
                Counters.ElementAt(i).Value = (Counters.Count() - i) * 100 / Counters.Count();
            }

            NotifyPropertyChanged("Counters");
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
        public string ChampionFeedbackName { get; set; }
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
        public string ChampionFeedbackId { get; set; }
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }


    public class Counter : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public Counter()
        {
            CounterRatings = new ObservableCollection<CounterRating>();
        }
        public int Value { get; set; }  //Local value to be used to set progress bar percentage
        public string Name { get; set; }
        public string ChampionFeedbackName { get; set; }
        private int score { get; set; }
        public int Score
        {
            get
            {
                return score;
            }
            set
            {
                if (value != score)
                {
                    score = value;
                    NotifyPropertyChanged("Score");
                }

            }
        }

        public ICollection<CounterRating> CounterRatings { get; set; }
        public string Id { get; set; }
        public string ChampionFeedbackId { get; set; }
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
        public string CommentId { get; set; }
    }

    public class CounterRating
    {
        public string UniqueUser { get; set; }
        public int Score { get; set; }
        public string Id { get; set; }
        public string CounterId { get; set; }
    }


    //Comment View Model
    class CommentDataSource : INotifyPropertyChanged
    {
        MobileServiceClient _client;

        public CommentDataSource(MobileServiceClient client)
        {
            _client = client;
            champTable = _client.GetTable<ChampionFeedback>();
            commentTable = _client.GetTable<Comment>();
            userTable = _client.GetTable<UserRating>();
            counterTable = _client.GetTable<Counter>();
            counterRatingTable = _client.GetTable<CounterRating>();
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

        public IMobileServiceTable<ChampionFeedback> champTable { get; set; }
        public IMobileServiceTable<Comment> commentTable { get; set; }
        public IMobileServiceTable<UserRating> userTable { get; set; }
        public IMobileServiceTable<Counter> counterTable { get; set; }
        public IMobileServiceTable<CounterRating> counterRatingTable { get; set; }


        //public async Task InitLocalStoreAsync(string champName)
        //{
        //    if (!App.MobileService.SyncContext.IsInitialized)
        //    {
        //        try
        //        {
        //            var store = new MobileServiceSQLiteStore("localstore.db");
        //            store.DefineTable<ChampionFeedback>();
        //            store.DefineTable<Comment>();
        //            store.DefineTable<UserRating>();
        //            await App.MobileService.SyncContext.InitializeAsync(store);
        //        }
        //        catch (Exception error)
        //        {
        //            var test = error.Message;
        //        }
        //    }

        //    await SyncAsync(champName);
        //}

        //private async Task SyncAsync(string champName)
        //{
        //    try
        //    {
        //        await App.MobileService.SyncContext.PushAsync();
        //        await table.PullAsync(champName, table.CreateQuery());
        //    }
        //    catch (Exception e)
        //    {

        //    }
        //}

        // Service operations

        public async Task GetChampionFeedbackAsync(string championName)
        {
            try
            {
                var result = await champTable.Where(c => c.Name == championName).ToListAsync();
                if (result.Count != 0)
                {   //If there was a match found for champion, 
                    ChampionFeedback = result[0];    //Get the first (and only) result
                    ChampionFeedback.SortComments();
                    ChampionFeedback.SortCounters();
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
                await champTable.InsertAsync(champion);
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

        public async Task SeedCounterAsync(Champion champion)
        {
            IsPending = true;
            ErrorMessage = null;

            try
            {
                //Iterate through the existing counters and seed the database with a default score 
                for (int index = 0; index < champion.Counters.Count(); index++) {
                    var counter = new Counter()
                    {
                        Name = champion.Counters.ElementAt(index),
                        Score = 7 - index,
                        ChampionFeedbackId = ChampionFeedback.Id,
                        ChampionFeedbackName = ChampionFeedback.Name,
                        Id = Guid.NewGuid().ToString(),
                        Value = (champion.Counters.Count() - index) * 100 / champion.Counters.Count(),
                    };

                    ChampionFeedback.Counters.Add(counter);
                    await counterTable.InsertAsync(counter);
                }

                ChampionFeedback.SortCounters();

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
                ChampionFeedbackName = ChampionFeedback.Name,
                ChampionFeedbackId = ChampionFeedback.Id,
                Id = Guid.NewGuid().ToString(),
            };

            try
            {
                this.ChampionFeedback.Comments.Add(comment);
                await commentTable.InsertAsync(comment);
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
            //If already rated, update to the new score 
            if (existingRating != null) {
                //Case for pressing the opposite vote button -- change rating to it
                if (existingRating.Score != score){
                    existingRating.Score = score;
                }
                //Case for pressing the same vote button -- simply a score of 0
                else {
                    existingRating.Score = 0;
                }
                try { await userTable.UpdateAsync(existingRating); }
                catch (Exception e) { ErrorMessage = e.Message; }
            }
            //Create a new rating otherwise
            else{
                var newRating = new UserRating()
                {
                    Score = score,
                    UniqueUser = GetDeviceId(),
                    Id = Guid.NewGuid().ToString(),
                    CommentId = comment.Id,
                };

                //Update the comment 
                comment.UserRatings.Add(newRating);
                try { await userTable.InsertAsync(newRating); }
                catch (Exception e) { ErrorMessage = e.Message; }

            }
            try
            {
                var updatedComment = await commentTable.LookupAsync(comment.Id);
                comment.Score = updatedComment.Score;
                comment.UserRatings = updatedComment.UserRatings;
                //ChampionFeedback.SortComments();
            }
            catch (MobileServicePreconditionFailedException ex)
            {
                ErrorMessage = ex.Message;
            }
            //Server conflict 
            catch (MobileServiceInvalidOperationException ex1)
            {
                ErrorMessage = ex1.Message;
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

        public async Task<Counter> SubmitCounter(Champion champion) { 
            IsPending = true;
            ErrorMessage = null;

            //Create the new comment
            var counter = new Counter()
            {
                Score = 0,
                ChampionFeedbackName = ChampionFeedback.Name,
                ChampionFeedbackId = ChampionFeedback.Id,
                Id = Guid.NewGuid().ToString(),
                Name = champion.UniqueId,
            };

            try
            {
                this.ChampionFeedback.Counters.Add(counter);
                await counterTable.InsertAsync(counter);
                return counter;

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


        public async Task SubmitCounterRating(Counter counter, int score)
        {

            IsPending = true;
            ErrorMessage = null;

            //Check if the user already rated the counter
            var existingRating = counter.CounterRatings.Where(x => x.UniqueUser == GetDeviceId()).FirstOrDefault();
            //If already rated, update to the new score 
            if (existingRating != null){
                //Case for pressing the opposite vote button -- change rating to it
                if (existingRating.Score != score){
                    existingRating.Score = score;
                }
                //Case for pressing the same vote button -- simply a score of 0
                else{
                    existingRating.Score = 0;
                }
                //Update the rating in database
                try { await counterRatingTable.UpdateAsync(existingRating); }
                catch (Exception e) { ErrorMessage = e.Message; }
            }
            //Create a new rating otherwise
            else{
                var newRating = new CounterRating(){
                    Score = score,
                    UniqueUser = GetDeviceId(),
                    Id = Guid.NewGuid().ToString(),
                    CounterId = counter.Id,
                };

                //Update the counter 
                counter.CounterRatings.Add(newRating);
                //Insert rating into database
                try { await counterRatingTable.InsertAsync(newRating); }
                catch (Exception e) { ErrorMessage = e.Message; }
            }
            try{
                var updatedCounter = await counterTable.LookupAsync(counter.Id);
                counter.Score = updatedCounter.Score;
                counter.CounterRatings = updatedCounter.CounterRatings;
            }
            catch (MobileServicePreconditionFailedException ex)
            {
                ErrorMessage = ex.Message;
            }
            //Server conflict 
            catch (MobileServiceInvalidOperationException ex1)
            {
                ErrorMessage = ex1.Message;
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
