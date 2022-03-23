using Fizzler.Systems.HtmlAgilityPack;
using HltvSharp.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace HltvSharp.Parsing
{
    public static partial class HltvParser
    {
        
        public static Task<Team> GetTeam(int teamid, WebProxy proxy = null)
        {
            return FetchPage($"team/{teamid}/-", (response) => GetInfoParse(response, teamid), proxy);
        }

        private static Team GetInfoParse(Task<HttpResponseMessage> response, int id = 0)
        {

            //load html
            var content = response.Result.Content;
            string htmlContent = content.ReadAsStringAsync().Result;

            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(htmlContent);

            HtmlNode document = html.DocumentNode;

            //start of actual code
            var team = new Team();

            //name
            team.Name = document.QuerySelector(".profile-team-name").InnerText;

            //country
            team.Country = document.QuerySelector(".team-country").InnerText;

            //id
            team.Id = int.Parse(document.SelectNodes("//link[@rel='canonical']")[0].Attributes["href"].Value.Split('/')[4]);

            //Team stats
            var profileteamstats = document.QuerySelectorAll(".profile-team-stat").ToArray();

            //WorldRanking
            if (int.TryParse(profileteamstats[0].ChildNodes["span"].InnerText.Replace("#", string.Empty), out var rank))
            {
                team.WorldRank = rank;
            }
            

            //AveragePlayerAge
            if (profileteamstats.ElementAtOrDefault(2) != null)
            {
                if (profileteamstats[2].InnerText.Contains("Average player age"))
                {
                    team.AveragePlayerAge = double.Parse(profileteamstats[2].ChildNodes["span"].InnerText.Replace(".", ","));
                }
            }

            //winrate
            if(double.TryParse(document.SelectNodes("//div[@class='highlighted-stat']")[1].ChildNodes["div"].InnerText.Replace("%", String.Empty).Replace(".", ","), out var winrate))
            {
                team.winRateProcentage = winrate;
            }

            //Coach
            if (profileteamstats.ElementAtOrDefault(3) != null)
            {
                if (profileteamstats[3].InnerText.Contains("Coach"))
                {
                    var Coach = new Coach();

                    //id
                    Coach.id = int.Parse(profileteamstats[3].ChildNodes["a"].Attributes["href"].Value.Split("/")[2]);

                    //country
                    Coach.Country = profileteamstats[3].ChildNodes["a"].ChildNodes["img"].Attributes["title"].Value;

                    //firstname
                    Coach.Name = profileteamstats[3].ChildNodes["a"].InnerText;

                    team.Coach = Coach;
                }
            }


            

            team.Players = GetPlayers(document);

            team.RecentMatches = GetRecentMatches(document);

            team.UpcomingMatches = GetUpcomingMatches(document);

            return team;
        }

        private static List<Player> GetPlayers(HtmlNode document)
        {
            var PlayerList = new List<Player>();

            if(!document.InnerHtml.Contains("table-container players-table"))
            {
                return null;
            }

            var table = document.SelectNodes("//table[@class='table-container players-table']")[0];
            var ht = new HtmlDocument();
            ht.LoadHtml(table.InnerHtml);

            table = ht.DocumentNode;

            var tbody = table.SelectNodes("//tbody")[0];

            var tbodyhtml = new HtmlDocument();
            tbodyhtml.LoadHtml(tbody.InnerHtml);

            HtmlNode tb = tbodyhtml.DocumentNode;

            foreach (var PlayerCellFE in tb.SelectNodes("//tr")) 
            {
                var Player = new Player();

                var htm = new HtmlDocument();
                htm.LoadHtml(PlayerCellFE.InnerHtml);

                var PlayerCell = htm.DocumentNode;
                //id
                Player.Id = int.Parse(PlayerCell.ChildNodes["td"].ChildNodes["a"].Attributes["href"].Value.Split('/')[2]);

                //name
                Player.Name = PlayerCell.SelectNodes("//img[@class='playerBox-bodyshot']")[0].Attributes["title"].Value;

                //Player image
                Player.playerImgUrl = PlayerCell.SelectNodes("//img[@class='playerBox-bodyshot']")[0].Attributes["src"].Value;

                //Country
                Player.Country = PlayerCell.SelectNodes("//img[@class='gtSmartphone-only flag']")[0].Attributes["title"].Value;

                //status
                Player.status = PlayerCell.QuerySelector(".player-status").InnerText;

                //Time on Team
                Player.timeOnTeam = PlayerCell.SelectNodes("//td")[2].ChildNodes["div"].InnerText;

                //Maps played
                Player.mapsPlayed = int.Parse(PlayerCell.SelectNodes("//td")[3].ChildNodes["div"].InnerText);

                //Rating
                Player.rating = double.Parse(PlayerCell.SelectNodes("//td")[4].ChildNodes["div"].InnerText.Replace(".", ","));

                PlayerList.Add(Player);
            }


            return PlayerList;
        }

        private static List<Match> GetUpcomingMatches(HtmlNode document)
        {

            var MatchList = new List<Match>();

            var table = document.SelectNodes("//table[@class='table-container match-table']")[0];
            var ht = new HtmlDocument();
            ht.LoadHtml(table.InnerHtml);

            table = ht.DocumentNode;

            for (var i = 0; i < 10; i++)
            {
                try
                {
                    if (table.SelectNodes("//tbody")[i] == null) { continue; }
                }
                catch
                {
                    continue;
                }

                var tbody = table.SelectNodes("//tbody")[i];

                

                var tbodyhtml = new HtmlDocument();
                tbodyhtml.LoadHtml(tbody.InnerHtml);

                HtmlNode tb = tbodyhtml.DocumentNode;

                foreach (var teamrow in tb.QuerySelectorAll(".team-row"))
                {
                    var Match = new Match();

                    //Date
                    var date = long.Parse(teamrow.ChildNodes["td"].ChildNodes["span"].Attributes["data-unix"].Value);
                    Match.date = DateTimeFromUnixTimestampMillis(date);

                    var K = new HtmlDocument();
                    K.LoadHtml(teamrow.InnerHtml);

                    var s = K.DocumentNode;

                    var teamcell = s.SelectNodes("//td[@class='team-center-cell']");

                    //Team 1 name
                    Match.team1name = teamcell[0].ChildNodes["div"].ChildNodes["a"].InnerText;

                    //team 1 id 
                    Match.team1id = int.Parse(teamcell[0].ChildNodes["div"].ChildNodes["a"].Attributes["href"].Value.Split('/')[2]);

                    //team 1 icon url
                    Match.team1iconurl = teamcell[0].ChildNodes["div"].ChildNodes["span"].ChildNodes["a"].ChildNodes["img"].Attributes["src"].Value;

                    //team 2 name
                    Match.team2name = teamcell[0].ChildNodes[5].ChildNodes[1].InnerText;

                    //team 2 id
                    Match.team2id = int.Parse(teamcell[0].ChildNodes[5].ChildNodes["a"].Attributes["href"].Value.Split('/')[2]);

                    //team 2 icon url
                    Match.team2iconurl = teamcell[0].ChildNodes[5].ChildNodes["span"].ChildNodes["a"].ChildNodes["img"].Attributes["src"].Value;




                    MatchList.Add(Match);
                }



            }
            return MatchList;
        }


    

    private static List<Match> GetRecentMatches(HtmlNode document)
    {

            if (!document.InnerHtml.Contains("table-container match-table"))
            {
                return null;
            }
            var MatchList = new List<Match>();

            var tablearray = document.SelectNodes("//table[@class='table-container match-table']");

            if(tablearray.ElementAtOrDefault(1) == null) { return null; }

            var table = tablearray[1];

            var ht = new HtmlDocument();
            ht.LoadHtml(table.InnerHtml);

            table = ht.DocumentNode;

            for (var i = 0; i < 10; i++)
            {
                try
                {
                    if (table.SelectNodes("//tbody")[i] == null) { continue; }
                }
                catch
                {
                    continue;
                }

                var tbody = table.SelectNodes("//tbody")[i];

                

            var tbodyhtml = new HtmlDocument();
            tbodyhtml.LoadHtml(tbody.InnerHtml);

            HtmlNode tb = tbodyhtml.DocumentNode;

            foreach (var teamrow in tb.QuerySelectorAll(".team-row"))
            {
                var Match = new Match();

                //Date
                var date = long.Parse(teamrow.ChildNodes["td"].ChildNodes["span"].Attributes["data-unix"].Value);
                Match.date = DateTimeFromUnixTimestampMillis(date);

                var K = new HtmlDocument();
                K.LoadHtml(teamrow.InnerHtml);

                var s = K.DocumentNode;

                var teamcell = s.SelectNodes("//td[@class='team-center-cell']");

                //Team 1 name
                Match.team1name = teamcell[0].ChildNodes["div"].ChildNodes["a"].InnerText;

                //team 1 id 
                Match.team1id = int.Parse(teamcell[0].ChildNodes["div"].ChildNodes["a"].Attributes["href"].Value.Split('/')[2]);

                //team 1 icon url
                Match.team1iconurl = teamcell[0].ChildNodes["div"].ChildNodes["span"].ChildNodes["a"].ChildNodes["img"].Attributes["src"].Value;

                //team 2 name
                Match.team2name = teamcell[0].ChildNodes[5].ChildNodes["a"].InnerText;

                //team 2 id
                Match.team2id = int.Parse(teamcell[0].ChildNodes[5].ChildNodes["a"].Attributes["href"].Value.Split('/')[2]);

                //team 2 icon url
                Match.team2iconurl = teamcell[0].ChildNodes[5].ChildNodes["span"].ChildNodes["a"].ChildNodes["img"].Attributes["src"].Value;


                //team 1 score
                Match.team1Score = int.Parse(teamcell[0].ChildNodes[3].ChildNodes[0].InnerText);

                //team 2 score
                Match.team2Score = int.Parse(teamcell[0].ChildNodes[3].ChildNodes[2].InnerText);


                MatchList.Add(Match);
            }



        }
        return MatchList;
    }



}
}
