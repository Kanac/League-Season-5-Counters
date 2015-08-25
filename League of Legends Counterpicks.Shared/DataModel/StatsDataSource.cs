using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using League_of_Legends_Counterpicks.Data;

namespace League_of_Legends_Counterpicks.DataModel
{
    public class ChampionData
    {
        public ChampionData()
        {
            Matchups = new List<Matchup>();
        }
        public string Key { get; set; }
        public string Role { get; set; }
        public List<Matchup> Matchups { get; set; }
    }

    public class Matchup
    {
        public string Key { get; set; }
        public int Games { get; set; }
        public double WinRate { get; set; }
        public double StatScore { get; set; }
    }

    public class Champions {
        [JsonProperty("Data")]
        public Dictionary<string, ChampionInfo> ChampionInfos { get; set; }
        public string Type { get; set; }
        public string Version { get; set;}
       }

    public class ChampionInfo {
        public int Id { get; set; }
        public List<string> Tags { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }

    }

    public static class StatsDataSource
    {
        public static string ErrorMessage { get; set; }
        private static Champions Champions {get;set;}
        private static HttpClient httpClient{ get; set; }
        static StatsDataSource() {
            httpClient = new HttpClient();
        }
        public async static Task<ChampionData> GetCounterStatsAsync(string champName) {
            ChampionData championData = new ChampionData() { Key = champName, Role = DataSource.GetRoleId(champName) };
            try
            {
                // Get the html for the champion page
                string response = await httpClient.GetStringAsync("http://champion.gg/champion/" + champName);
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(response);

                // Get the html we need for champion statistics 
                var champInfoHtml = doc.DocumentNode.Descendants("script").Where(x => x.InnerHtml.Contains("matchupData.championData")).FirstOrDefault().InnerHtml;
                var matchupStartIndex = champInfoHtml.IndexOf("\"matchups\":[");
                var champStatsStartHtml = champInfoHtml.Substring(matchupStartIndex);
                var matchupEndIndex = champStatsStartHtml.IndexOf("]") + 1;
                var champStatsHtml = "{" + champStatsStartHtml.Substring(0, matchupEndIndex) + "}";

                // Parse Json into .Net objects
                JObject champsStatsJson = JObject.Parse(champStatsHtml);
                List<JToken> matchupsJson = champsStatsJson["matchups"].Children().ToList();
                foreach (JToken statsJson in matchupsJson)
                {
                    Matchup matchup = JsonConvert.DeserializeObject<Matchup>(statsJson.ToString());
                    championData.Matchups.Add(matchup);
                }

                return championData;

            }
            catch (HttpRequestException hre)
            {
                ErrorMessage = hre.Message;
                return null;
            }

            catch (JsonException e)
            {
                ErrorMessage = e.Message;
                return null;
            }

            catch (Exception e) {
                ErrorMessage = e.Message;
                return null;
            }
        }

        public static Champions GetChampions() {
            if (Champions != null)
                return Champions;
            else
                return null;
        }

        public async static Task<Champions> GetChampionsAsync() {
            if (Champions != null)
                return Champions;

            Champions = new Champions();
            try
            {
                string response = await httpClient.GetStringAsync("https://global.api.pvp.net/api/lol/static-data/na/v1.2/champion?champData=tags&api_key=097a42b8-6117-405a-a95f-77e84f82827d");
                JObject champsJson = JObject.Parse(response);
                Champions = JsonConvert.DeserializeObject<Champions>(champsJson.ToString());
                return Champions;
            }
            catch (HttpRequestException hre)
            {
                ErrorMessage = hre.Message;
                return null;
            }
            catch (JsonException e)
            {
                ErrorMessage = e.Message;
                return null;
            }

            catch (Exception e)
            {
                ErrorMessage = e.Message;
                return null;
            }
            }

        public static List<string> GetRoles() {
            return new List<string>(){"All", "Assassin", "Fighter", "Mage", "Marksman", "Support", "Tank"};
        }
    }


}
