using System;
using System.Collections.Generic;
using System.Text;

namespace HltvApi.Models
{
    public class UpcomingMatchItem
    {
        public string? date { get; set; }
        public DateTime DateTime { get; set; }
        public string? team1name { get; set; }
        public string? team2name { get; set; }

        public int team1id { get; set; }
        public int team2id { get; set; }
        public string? team1iconurl { get; set; }
        public string? team2iconurl { get; set; }

    }
}
