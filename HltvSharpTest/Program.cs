// See https://aka.ms/new-console-template for more information
var text = "GamerLegion";//Console.ReadLine();

var Search = new HltvSharp.Search();
var res = await Search.Teams(text);

var t = await HltvSharp.Parsing.HltvParser.GetTeam(res[0].Id);
Console.WriteLine(t.Name);
foreach(var match in t.RecentMatches)
{
    Console.WriteLine(match.date);
}

//var m = await HltvSharp.Parsing.HltvParser.GetMatch(t.RecentMatches[0].id);
//Console.WriteLine(m.Id);