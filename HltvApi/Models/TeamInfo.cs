using System;
using System.Collections.Generic;
using System.Text;

namespace HltvApi.Models
{
    public class TeamInfo
    {
        public string Name { get; set; }
        public string Country { get; set; }
        public int Id { get; set; }
        public int WorldRank { get; set; }
        public double AveragePlayerAge { get; set; }
        public TeamInfoCoach Coach { get; set; }

    }
}
