namespace Leaderboard.DataObjects
{
    public class PlayerRankDto
    {
        public string Id { get; set; }
        public string PlayerName { get; set; }
        public int Score { get; set; }
        public int Rank { get; set; }
    }
}