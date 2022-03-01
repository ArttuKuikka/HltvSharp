using HltvApi.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace HltvApi.Parsing
{
    public static partial class HltvParser
    {
        public static Task<TeamInfo> GetTeam(int teamid, WebProxy proxy = null)
        {
            return FetchPage($"team/{teamid}/-", (response) => GetInfoParse(response, teamid), proxy);
        }

        private static TeamInfo GetInfoParse(Task<HttpResponseMessage> response, int id = 0)
        {
            var content = response.Result.Content;
            string htmlContent = content.ReadAsStringAsync().Result;

            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(htmlContent);

            HtmlNode document = html.DocumentNode;


            var teamInfo = new TeamInfo();




            return teamInfo;
        }

        private static async Task<List<TeamPlayerInfo>> GetPlayers(TeamSearchItem team)
        {
            if (team == null) { return null; }

            if (team.Id == 0) { return null; }

            if (team.Name == null) { return null; }

            var teamPlayerInfoList = new List<TeamPlayerInfo>();

            //Get html

            return teamPlayerInfoList;
        }

        private static async Task<List<RecentMatchItem>> GetRecentMatches(TeamSearchItem team)
        {
            if (team == null) { return null; }

            if (team.Id == 0) { return null; }

            if (team.Name == null) { return null; }

            var RecentMatchItemList = new List<RecentMatchItem>();




            var RawHtml = "vaihda"; //VAIHDA

            HtmlDocument doc = new HtmlDocument();




            doc.LoadHtml(RawHtml);

            foreach (HtmlNode table in doc.DocumentNode.SelectNodes("//table[@class='" + "table-container match-table" + "']").Skip(1))
            {
                foreach (HtmlNode row in table.SelectNodes("tbody"))
                {

                    foreach (HtmlNode teamrow in row.SelectNodes("//tr[@class='" + "team-row" + "']"))
                    {
                        HtmlNode datecell = teamrow.SelectSingleNode("//td[@class='" + "date-cell" + "']");

                        Console.WriteLine(datecell.Attributes["data-unix"].Value);
                    }

                }


            }



            return RecentMatchItemList;
        }

        private static async Task<List<UpcomingMatchItem>> GetUpcomingMatches(TeamSearchItem team) //trash
        {
            if (team == null) { return null; }

            if (team.Id == 0) { return null; }

            if (team.Name == null) { return null; }



            var list = new List<UpcomingMatchItem>();



            return list;
        }
        private static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
    }
}
