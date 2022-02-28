using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HltvApi
{

    public class TeamSearchItem
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? flagUrl { get; set; }
        public string? teamLogoDarkUrl { get; set; }
        public string? teamLogoLightUrl { get; set; }
        public string? webLocation { get; set; }

        public List<TeamSearchPlayerItem>? PlayerList { get; set; } 
    }

    public class PlayerSearchItem
    {
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? nickName { get; set; }
        public string? flagUrl { get; set; }
        public string? webLocation { get; set; }
        public int? id { get; set; }
        public string? pictureUrl { get; set; }
        public PlayerTeamItem? team { get; set; }

    }

    public class PlayerTeamItem
    {
        public string? name { get; set; }
        public string? teamLogoDarkUrl { get; set; }
        public string? teamLogoLightUrl { get; set; }
        public string? webLocation { get; set; }
    }

    public class TeamSearchPlayerItem
    {
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? nickName { get; set; }
        public string? flagUrl { get; set; }
        public string? webLocation { get; set; }
    }

    public class EventsSearchItem
    {
        public int id { get; set; }
        public string? name { get; set; }
        public string? flagUrl { get; set; }
        public string? eventLogoUrl { get; set; }
        public string? webLocation { get; set; }
        public string? physicalLocation { get; set; }
        public string prizePool { get; set; }
        public string? eventType { get; set; }
        
    }
}
