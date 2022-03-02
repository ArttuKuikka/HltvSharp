﻿using Fizzler.Systems.HtmlAgilityPack;
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
            var profileteamstats = document.SelectNodes("//div[@class='profile-team-stat']");

            //WorldRanking
            team.WorldRank = int.Parse(profileteamstats[0].ChildNodes["span"].ChildNodes["a"].InnerText.Replace("#", string.Empty));
            
            //AveragePlayerAge
            if(profileteamstats[2] != null)
            {
                if(profileteamstats[2].InnerText.Contains("Average player age"))
                {
                    team.AveragePlayerAge = double.Parse(profileteamstats[2].ChildNodes["span"].InnerText.Replace(".", ","));
                }
            }

            //Coach
            if(profileteamstats[3] != null)
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


            Console.WriteLine(team);

            team.Players = GetPlayers(document);

            team.RecentMatches = GetRecentMatches(document);

            team.UpcomingMatches = GetUpcomingMatches(document);

            return team;
        }

        private static List<Player> GetPlayers(HtmlNode document)
        {
            var PlayerList = new List<Player>();

            var table = document.SelectNodes("//table[@class='table-container players-table']")[0];

            HtmlNode tbody = null;
            if(table.SelectNodes("//tbody")[1] == null)
            {
                tbody = table.SelectNodes("//tbody")[0];
            }
            else
            {
                tbody = table.SelectNodes("//tbody")[1];
            }

            var tbodyhtml = new HtmlDocument();
            tbodyhtml.LoadHtml(tbody.InnerHtml); 

            HtmlNode tb = tbodyhtml.DocumentNode;

            foreach (var PlayerCell in tb.SelectNodes("//tr").Skip(1)) //idk why it doesnt work if i use tbody maybe bug in hmtl agility pack
            {
                var Player = new Player();


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
                Player.rating = double.Parse(PlayerCell.SelectNodes("//td")[4].ChildNodes["div"].InnerText.Replace(".",","));

                PlayerList.Add(Player);
            }


            return PlayerList;
        }

        private static List<Match> GetUpcomingMatches(HtmlNode document)
        {

            var MatchList = new List<Match>();

            var table = document.SelectNodes("//table[@class='table-container match-table']")[0];

            var tbody = table.SelectNodes("//tbody")[3];

            var tbodyhtml = new HtmlDocument();
            tbodyhtml.LoadHtml(tbody.InnerHtml);

            HtmlNode tb = tbodyhtml.DocumentNode;

            foreach (var teamrow in tb.QuerySelectorAll(".team-row"))
            {
                var Match = new Match();

                MatchList.Add(Match);
            }



            return MatchList;
        }

        private static List<Match> GetRecentMatches(HtmlNode document)
        {

            var MatchList = new List<Match>();

            var table = document.SelectNodes("//table[@class='table-container match-table']")[0];

            var tbody = table.SelectNodes("//tbody")[4];

            var tbodyhtml = new HtmlDocument();
            tbodyhtml.LoadHtml(tbody.InnerHtml);

            HtmlNode tb = tbodyhtml.DocumentNode;

            foreach (var teamrow in tb.QuerySelectorAll(".team-row")) //tee for loop että saa kaikki tablet
            {
                var Match = new Match();

                //Date
                var date = long.Parse(teamrow.ChildNodes["td"].ChildNodes["span"].Attributes["data-unix"].Value);
                Match.date = DateTimeFromUnixTimestampMillis(date);

                var teamcell = teamrow.SelectNodes("//td[@class='team-center-cell']");

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



            return MatchList;
        }

       
        
    }
}
