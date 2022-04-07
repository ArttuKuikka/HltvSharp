using System;
using System.Collections.Generic;
using System.Text;

namespace HltvSharp.Models
{
    public class NewsItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string time { get; set; }
        public int CommentCount { get; set; }
    }
}
