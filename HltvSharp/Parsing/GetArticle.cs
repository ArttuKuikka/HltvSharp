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
    }
}
