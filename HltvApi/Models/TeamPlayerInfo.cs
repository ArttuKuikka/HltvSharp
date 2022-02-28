using System;
using System.Collections.Generic;
using System.Text;

namespace HltvApi.Models
{
    public class TeamPlayerInfo
    {
        public string name { get; set; }
        public int id { get; set; }
        public string webLocation { get; set; }
        public string playerImgUrl { get; set; }
        public string flagTitle { get; set; }
        public string flagUrl { get; set; }
        public string status { get; set; }
        public string timeOnTeam { get; set; }
        public int mapsPlayed { get; set; }
        public double rating { get; set; }
    }
}
