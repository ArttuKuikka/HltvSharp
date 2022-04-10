using Fizzler.Systems.HtmlAgilityPack;
using HltvSharp.Models;
using HltvSharp.Models.Enums;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
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
        public static Task<Article> GetArticle(int id, WebProxy proxy = null)
        {
            return FetchPage("news/" + id + "/-", article, proxy);
        }

        private static Article article(Task<HttpResponseMessage> response)
        {
            var content = response.Result.Content;
            string htmlContent = content.ReadAsStringAsync().Result;

            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(htmlContent);

            HtmlNode document = html.DocumentNode;

            var Article = new Article();

            //title
            Article.Title = document.QuerySelector(".headline").InnerText.Replace("&quot;⁠", string.Empty);

            //author
            Article.Author = document.QuerySelector(".authorName").InnerText;

            //date
            var unix = document.QuerySelector(".date").Attributes["data-unix"].Value;
            var dt = DateTimeFromUnixTimestampMillis(long.Parse(unix));
            Article.Date = dt;

            //header
            Article.Header = document.QuerySelector(".headertext").InnerText;




            var articlebody = document.QuerySelector(".newstext-con");

            var qt = '\u0022';

            var ArticleArray = new JArray();

            foreach (var segment in articlebody.ChildNodes.Skip(3))
            {
                if(segment.Name == "div")
                {
                    if(segment.FirstChild.Name == "img")
                    {
                        var img = new
                        {
                            Type = "Image",
                            ImageUrl = segment.ChildNodes["img"].Attributes["src"].Value,
                            ImageText = segment.QuerySelector(".imagetext").InnerText
                        };

                        ArticleArray.Add(JToken.FromObject(img));
                    }
                    else if (segment.HasClass("featured-quote"))
                    {
                        var quotetext = segment.QuerySelector(".featured-quote-quote");
                        var author = segment.QuerySelector(".featured-quote-author");

                        var authorurl = "";
                        if(author.FirstChild.Attributes["href"] != null)
                        {
                            authorurl = author.FirstChild.Attributes["href"].Value;
                        }

                        var quote = new
                        {
                            Type = "Quote",
                            Text = quotetext.InnerText,
                            AuthorName = author.InnerText.Replace("&quot;", qt.ToString()),
                            AuthorUrl = authorurl
                        };

                        ArticleArray.Add(JToken.FromObject(quote));
                    }
                    else if (segment.HasClass("newsitem-match-result"))
                    {
                        //top segment with eventname and type
                        var top = segment.QuerySelector(".newsitem-match-result-top");

                        var ename = top.QuerySelector("a").InnerText;
                        var eurl = top.QuerySelector("a").Attributes["href"].Value.Split('/')[2];

                        //middle segment with teams and results
                        var midle = segment.QuerySelector(".newsitem-match-result-middle");


                        //results for both teams
                        var teams = midle.QuerySelectorAll(".newsitem-match-result-team-con");

                        var team1name = teams.First().QuerySelector(".newsitem-match-result-team").QuerySelector("a").InnerText;
                        var team1id = teams.First().QuerySelector(".newsitem-match-result-team").QuerySelector("a").Attributes["href"].Value.Split('/')[2];
                        var team1logourl = teams.First().QuerySelector("img").Attributes["src"].Value;


                        var team2name = teams.Last().QuerySelector(".newsitem-match-result-team").QuerySelector("a").InnerText;
                        var team2id = teams.Last().QuerySelector(".newsitem-match-result-team").QuerySelector("a").Attributes["href"].Value.Split('/')[2];
                        var team2logourl = teams.Last().QuerySelectorAll("img").Last().Attributes["src"].Value;

                        //score, id and date
                        var scorecon = midle.QuerySelector(".newsitem-match-result-score-con");

                        var id = scorecon.QuerySelector("a").Attributes["href"].Value.Split('/')[2];

                        var dateunix = scorecon.QuerySelector(".newsitem-match-result-date").Attributes["data-unix"].Value;


                        //teams scores
                        var div = scorecon.ChildNodes[2];

                        var team1score = div.ChildNodes[1].InnerText;
                        var team2score = div.ChildNodes[5].InnerText;


                        //mapscores
                        var mapscorelist = new List<newsitemmatchresult>();

                        var mapresults = segment.QuerySelectorAll(".newsitem-match-result-map");

                        foreach(var res in mapresults)
                        {
                            var resitem = new newsitemmatchresult();

                            resitem.Team1Score = res.ChildNodes[1].InnerText;
                            resitem.Team2Score = res.ChildNodes[5].InnerText;
                            resitem.Map = res.QuerySelector(".newsitem-match-result-map-name").InnerText;
                            resitem.mapstatid = res.QuerySelector(".newsitem-match-result-map-name").Attributes["href"].Value.Split('/')[4];


                            mapscorelist.Add(resitem);
                        }

                        



                        var matchresult = new
                        {
                            Type = "newsitem-match-result",
                            EventName = ename,
                            EventId = eurl,
                            Team1Name = team1name,
                            Team1Id = team1id,
                            Team1LogoUrl = team1logourl,
                            Team2Name = team2name,
                            Team2Id = team2id,
                            Team2LogoUrl = team2logourl,
                            MatchId = id,
                            MatchUnixDate = dateunix,
                            Team1Score = team1score,
                            Team2Score = team2score,
                            MapsScores = mapscorelist.ToArray()
                        };

                        ArticleArray.Add(JToken.FromObject(matchresult));
                    }
                    


                }
                else
                {
                    foreach (var item in segment.ChildNodes)
                    {
                        if (item.Name == "a")
                        {
                            var link = new
                            {
                                Type = "Link",
                                LinkUrl = "https://hltv.org" + item.Attributes["href"].Value,
                                LinkText = item.InnerText.Replace("&quot;", qt.ToString())
                        };

                            var jsonlink = JToken.FromObject(link);

                            ArticleArray.Add(jsonlink);
                        }
                        else
                        {
                            var text = new
                            {
                                Type = "Text",
                                Text = item.InnerText.Replace("&quot;", qt.ToString())
                        };

                            var jsontext = JToken.FromObject(text);

                            ArticleArray.Add(jsontext);
                        }
                    }

                }
                var space = new
                {
                    Type = "Space"
                };

                ArticleArray.Add(JToken.FromObject(space));
            }

            Article.ArticleBody = ArticleArray;

            

            


            return Article;
        }
        private class newsitemmatchresult
        {
         public string Map { get; set; }
         public string Team1Score { get; set; }
         public string Team2Score { get; set; }

        public string mapstatid { get; set; }
        }
    }

    
}
