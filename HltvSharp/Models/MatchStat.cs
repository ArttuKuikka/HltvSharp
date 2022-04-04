using System;
using System.Collections.Generic;
using System.Text;

namespace HltvSharp.Models
{
    public class MatchStat
    {
        public string PlayerName { get; set; }
        public string KD { get; set; }
        public int plusminus { get; set; }
        public int KastProcentage { get; set; }
        public int Rating { get; set; }
    }
}
