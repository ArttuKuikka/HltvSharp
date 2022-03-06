// See https://aka.ms/new-console-template for more information
var text = "ence";//Console.ReadLine();

var Search = new HltvSharp.Search();
var res = await Search.Teams(text);

var t = HltvSharp.Parsing.HltvParser.GetTeam(res[0].Id);
Console.WriteLine(t.Result.Name);
Console.WriteLine(t.Result.Id);
Console.WriteLine(t.Result.WorldRank);
Console.WriteLine(t.Result.Players[0].Id);
var p = HltvSharp.Parsing.HltvParser.GetPlayer(t.Result.Players[0].Id);
Console.WriteLine(p.Result.Name);
var m = HltvSharp.Parsing.HltvParser.GetMatch(2354933);
Console.WriteLine(m.Result.Team1.Name);
