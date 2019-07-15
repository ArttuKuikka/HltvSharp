using Fizzler.Systems.HtmlAgilityPack;
using HltvApi.Models;
using HltvApi.Models.Enums;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HltvApi.Parsing
{
    public static partial class HltvParser
    {
        public static Task<List<MatchResult>> GetMatchResults(int offset = 0)
        {
            return FetchPage<List<MatchResult>>("results?offset=" + offset, ParseResultsPage);
        }

        private static List<MatchResult> ParseResultsPage(Task<HttpResponseMessage> response)
        {
            var content = response.Result.Content;
            string htmlContent = content.ReadAsStringAsync().Result;

            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(htmlContent);

            HtmlNode document = html.DocumentNode;

            var resultsHolderNode = document.QuerySelectorAll(".results-holder").Last();
            var resultConNodes = resultsHolderNode.QuerySelectorAll(".result-con");

            List<MatchResult> matchResults = new List<MatchResult>();

            foreach (HtmlNode resultNode in resultConNodes)
            {
                MatchResult model = new MatchResult();

                //Match ID
                string matchPageUrl = resultNode.QuerySelector(".a-reset").Attributes["href"].Value;
                model.Id = int.Parse(matchPageUrl.Split("/", StringSplitOptions.RemoveEmptyEntries)[1]);

                //Match date
                long unixDateMilliseconds = long.Parse(resultNode.Attributes["data-zonedgrouping-entry-unix"].Value);
                model.Date = DateTimeFromUnixTimestampMillis(unixDateMilliseconds);

                //Event ID and name
                Event eventModel = new Event();
                string eventImageUrl = resultNode.QuerySelector(".event-logo").Attributes["src"].Value;
                if (eventImageUrl.ToLowerInvariant().Contains("nologo"))
                    eventModel.Id = null;
                else
                    eventModel.Id = int.Parse(eventImageUrl.Split("/").Last().Split(".").First());

                eventModel.Name = resultNode.QuerySelector(".event-name").InnerText;
                model.Event = eventModel;

                //Number of stars
                model.Stars = resultNode.QuerySelectorAll(".stars i").Count();

                var teamNodes = resultNode.QuerySelectorAll(".team-cell").ToList();
                var resultScoreNode = resultNode.QuerySelector(".result-score");
                var scoreSpanNodes = resultScoreNode.QuerySelectorAll("span").ToList();

                //Team 1 ID and name
                Team team1Model = new Team();
                string team1LogoUrl = teamNodes[0].QuerySelector("img").Attributes["src"].Value;
                team1Model.Id = int.Parse(team1LogoUrl.Split('/').Last());
                team1Model.Name = teamNodes[0].QuerySelector("img").Attributes["alt"].Value;
                model.Team1 = team1Model;
                model.Team1Score = int.Parse(scoreSpanNodes[0].InnerText);

                //Team 2 ID and name
                Team team2Model = new Team();
                string team2LogoUrl = teamNodes[1].QuerySelector("img").Attributes["src"].Value;
                team2Model.Id = int.Parse(team1LogoUrl.Split('/').Last());
                team2Model.Name = teamNodes[1].QuerySelector("img").Attributes["alt"].Value;
                model.Team2 = team2Model;
                model.Team2Score = int.Parse(scoreSpanNodes[1].InnerText);

                //Winning team
                model.WinningTeam = model.Team1Score > model.Team2Score ? model.Team1 : model.Team2;

                //Map and format
                string mapText = resultNode.QuerySelector(".map-text").InnerText;
                if (mapText.Contains("bo"))
                    model.Format = mapText;
                else
                {
                    model.Format = "bo1";
                    model.Map = MapSlug.MapSlugs[mapText];
                }

                matchResults.Add(model);
            }
            return matchResults;
        }
    }
}
