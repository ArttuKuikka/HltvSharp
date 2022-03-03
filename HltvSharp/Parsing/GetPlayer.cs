using Fizzler.Systems.HtmlAgilityPack;
using HltvSharp.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HltvSharp.Parsing
{
    public static partial class HltvParser
    {
        public static Task<Player> GetPlayer(int playerid, WebProxy proxy = null)
        {
            return FetchPage($"player/{playerid}/-", (response) => GetPlayerParse(response, playerid), proxy);
        }
        private static Player GetPlayerParse(Task<HttpResponseMessage> response, int id = 0)
        {
            //load html
            var content = response.Result.Content;
            string htmlContent = content.ReadAsStringAsync().Result;

            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(htmlContent);

            HtmlNode document = html.DocumentNode;


            var player = new Player();

            //id
            player.Id = int.Parse(document.SelectNodes("//meta[@property='og:url']/@content")[0].GetAttributeValue("content", String.Empty).Split('/', StringSplitOptions.RemoveEmptyEntries)[3]);

            //name
            var nick = document.QuerySelector(".playerNickname").InnerText;
            var realname = document.QuerySelector(".playerRealname").InnerText;
            player.Name = $"{realname.Split(' ')[1]} '{nick}' {realname.Split(' ')[2]}"; //split ei toimi oikein

            //PlayerImageUrl
            player.playerImgUrl = document.QuerySelector(".bodyshot-img").Attributes["src"].Value;

            //Country
            player.Country = document.QuerySelector(".flag").Attributes["title"].Value;

            //age
            var age = document.QuerySelector(".playerInfo").QuerySelector(".playerAge").SelectNodes("//span[@itemprop='text']")[0].InnerText;
            age = Regex.Replace(age, "[^0-9]", "");
            player.age = int.Parse(age);

            //current team
            player.currentTeam = document.QuerySelector(".playerInfo").QuerySelector(".playerTeam").QuerySelector(".listRight").ChildNodes[1].InnerText;

            player.teams = GetTeams(document);

            return player;
        }
        
        private static List<Team> GetTeams(HtmlNode document)
        {
            List<Team> teams = new List<Team>();

            return teams;
        }

        private static List<Models.Match> GetMatches(HtmlNode document)
        {
            List<Models.Match> matches = new List<Models.Match>();

            return matches;
        }

    }
}
