using Fizzler.Systems.HtmlAgilityPack;
using HltvSharp.Models;
using HltvSharp.Models.Enums;
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
        public static Task<List<NewsItem>> GetNews(WebProxy proxy = null)
        {
            return FetchPage("/", GetNewsList, proxy);
        }

        private static List<NewsItem> GetNewsList(Task<HttpResponseMessage> response)
        {
            var content = response.Result.Content;
            string htmlContent = content.ReadAsStringAsync().Result;

            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(htmlContent);

            HtmlNode document = html.DocumentNode;

            var NewsList = new List<NewsItem>();


            foreach (var box in document.QuerySelectorAll(".standard-list"))
            {
                foreach(var article in box.ChildNodes)
                {
                    NewsItem item = new NewsItem();

                    //id
                    var id = article.Attributes["href"].Value.Split('/')[2];
                    item.Id = int.Parse(id);

                    //title
                    item.Title = article.QuerySelector(".newstext").InnerText;

                    //time
                    item.time = article.QuerySelector(".newsrecent").InnerText;

                    //commentcount
                    var cc = article.QuerySelector(".newstc").ChildNodes[3].InnerText;
                    cc = Regex.Replace(cc, "[^0-9]", "");
                    item.CommentCount = int.Parse(cc);



                    NewsList.Add(item);
                }
            }

            return NewsList;
        }
    }
}
