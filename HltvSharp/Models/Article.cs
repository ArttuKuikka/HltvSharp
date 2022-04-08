using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace HltvSharp.Models
{
    public class Article
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime Date { get; set; }
        public string Header { get; set; }
        public JArray ArticleBody { get; set; }
    }
}
