using Fizzler.Systems.HtmlAgilityPack;
using HltvApi.Models;
using HltvApi.Models.Enums;
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
        public static Task<List<UpcomingMatch>> GetUpcomingMatches(WebProxy proxy = null)
        {
            return FetchPage("matches", ParseMatchesPage, proxy);
        }

        private static List<UpcomingMatch> ParseMatchesPage(Task<HttpResponseMessage> response)
        {
            var content = response.Result.Content;
            string htmlContent = content.ReadAsStringAsync().Result;

            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(htmlContent);

            HtmlNode document = html.DocumentNode;

            var upcomingMatchNodes = document.QuerySelectorAll(".upcoming-match");

            List<UpcomingMatch> upcomingMatches = new List<UpcomingMatch>();

            foreach (HtmlNode upcomingMatchNode in upcomingMatchNodes)
            {
                try
                {
                    UpcomingMatch model = new UpcomingMatch();

                    //Match ID
                    string matchPageUrl = upcomingMatchNode.Attributes["href"].Value;
                    model.Id = int.Parse(matchPageUrl.Split('/', StringSplitOptions.RemoveEmptyEntries)[1]);

                    //Match date
                    long unixDateMilliseconds = long.Parse(upcomingMatchNode.Attributes["data-zonedgrouping-entry-unix"].Value);
                    model.Date = DateTimeFromUnixTimestampMillis(unixDateMilliseconds);

                    //Event ID and name
                    Event eventModel = new Event();
                    string eventImageUrl = upcomingMatchNode.QuerySelector(".event-logo").Attributes["src"].Value;
                    eventModel.Id = int.Parse(eventImageUrl.Split("/").Last().Split(".").First());
                    eventModel.Name = upcomingMatchNode.QuerySelector(".event-name").InnerText;
                    model.Event = eventModel;

                    //Number of stars
                    model.Stars = upcomingMatchNode.QuerySelectorAll(".stars i").Count();

                    var teamNodes = upcomingMatchNode.QuerySelectorAll(".team-cell").ToList();
                    var resultScoreNode = upcomingMatchNode.QuerySelector(".result-score");

                    //Team 1 ID and name
                    Team team1Model = new Team();
                    string team1LogoUrl = teamNodes[0].QuerySelector("img").Attributes["src"].Value;
                    team1Model.Id = int.Parse(team1LogoUrl.Split('/').Last());
                    team1Model.Name = teamNodes[0].QuerySelector("img").Attributes["alt"].Value;
                    model.Team1 = team1Model;

                    //Team 2 ID and name
                    Team team2Model = new Team();
                    string team2LogoUrl = teamNodes[1].QuerySelector("img").Attributes["src"].Value;
                    team2Model.Id = int.Parse(team1LogoUrl.Split('/').Last());
                    team2Model.Name = teamNodes[1].QuerySelector("img").Attributes["alt"].Value;
                    model.Team2 = team2Model;

                    //Map and format
                    string mapText = upcomingMatchNode.QuerySelector(".map-text").InnerText;
                    if (mapText.Contains("bo"))
                        model.Format = mapText;
                    else
                    {
                        model.Format = "bo1";
                        model.Map = MapSlug.MapSlugs[mapText];
                    }

                    upcomingMatches.Add(model);
                }
                catch (Exception)
                {

                }
            }
            return upcomingMatches;
        }
    }
}
